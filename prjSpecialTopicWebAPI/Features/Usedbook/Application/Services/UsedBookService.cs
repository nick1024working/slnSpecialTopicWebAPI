using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Models;
using System.Linq.Expressions;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class UsedBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UsedBookRepository _usedBookRepository;
        private readonly UsedBookImageService _usedBookImageService;
        private readonly UsedBookImageRepository _usedBookImageRepository;
        private readonly ImageService _imageService;
        private readonly BookSaleTagRepository _saleTagRepository;
        private readonly ILogger<UsedBookService> _logger;

        public UsedBookService (
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UsedBookRepository usedBookRepository,
            UsedBookImageService usedBookImageService,
            UsedBookImageRepository usedBookImageRepository,
            ImageService imageService,
            BookSaleTagRepository saleTagRepository,
            ILogger<UsedBookService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usedBookRepository = usedBookRepository;
            _usedBookImageService = usedBookImageService;
            _usedBookImageRepository = usedBookImageRepository;
            _imageService = imageService;
            _saleTagRepository = saleTagRepository;
            _logger = logger;
        }

        // ========== 新增、更新 ==========

        public async Task<Result<Guid>> CreateAsync(Guid sellerId, CreateBookRequest request, CancellationToken ct = default)
        {
            Guid usedBookId = Guid.NewGuid();
            DateTime nowTime = DateTime.UtcNow;

            var entity = _mapper.Map<UsedBook>(request);
            entity.Id = usedBookId;
            entity.SellerId = sellerId;
            entity.IsSold = false;
            entity.IsActive = true;
            entity.Slug = usedBookId.ToString();
            entity.CreatedAt = nowTime;
            entity.UpdatedAt = nowTime;

            using var tx = _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                _usedBookRepository.Add(entity);
                var commandResult = await _usedBookImageService.CreateAsync(usedBookId, request.ImageList);
                if (!commandResult.IsSuccess)
                    throw new Exception(commandResult.ErrorMessage);

                await _unitOfWork.CommitAsync(ct);

                return Result<Guid>.Success(usedBookId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Guid>.Map(ex, _logger);
            }
        }

        //public async Task<Result<Unit>> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default)
        //{
        //    try
        //    {
        //        await _usedBookImageService.UpdateByBookIdAsync(id, request.ImageList, ct);
        //        var commandResult = await _usedBookRepository.UpdateAsync(id, request, ct);

        //        if (commandResult == false)
        //            return Result<Unit>.Failure("非此資源擁有者", ErrorCodes.General.NotFound);

        //        await _unitOfWork.CommitAsync(ct);

        //        return Result<Unit>.Success(Unit.Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
        //    }
        //}

        public async Task<Result<Unit>> UpdateActiveStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateActiveStatusAsync(id, isActive, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 查詢 ==========

        public async Task<Result<EditBookDto>> GetForEditByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var entoity = await _usedBookRepository.GetEntityByIdAsync(id, ct);
                if (entoity == null)
                    return Result<EditBookDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageQueryResult = await _usedBookImageRepository.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<EditBookDto>(entoity);
                dto.ImageList = _mapper.Map<IEnumerable<BookImageDto>>(imageQueryResult);

                return Result<EditBookDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<EditBookDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<PublicBookDetailDto>> GetPubicDetailAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var bookQueryResult = await _usedBookRepository.GetTextByIdAsync(id, ct);
                if (bookQueryResult == null)
                    return Result<PublicBookDetailDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageQueryResult = await _usedBookImageRepository.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<PublicBookDetailDto>(bookQueryResult);
                dto.ImageList = _mapper.Map<IEnumerable<BookImageDto>>(imageQueryResult);

                return Result<PublicBookDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PublicBookDetailDto>.Map(ex, _logger);
            }
        }

        // TODO: 需要分頁
        public async Task<Result<IReadOnlyList<PublicBookListItemDto>>> GetPublicListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetPublicBookListAsync(predicate, orderBy, ct);
                var dtoList = new List<PublicBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<PublicBookListItemDto>(res);
                    dto.CoverImageUrl = BuildImageUrl(res.CoverStorageProvider, res.CoverObjectKey);

                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<PublicBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<PublicBookListItemDto>>.Map(ex, _logger);
            }
        }

        // TODO: 需要分頁
        public async Task<Result<IReadOnlyList<UserBookListItemDto>>> GetUserBookListAsync(Guid userId, BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetUserBookListAsync(userId, predicate, orderBy, ct);
                var dtoList = new List<UserBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<UserBookListItemDto>(res);
                    dto.CoverImageUrl = BuildImageUrl(res.CoverStorageProvider, res.CoverObjectKey);

                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<UserBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<UserBookListItemDto>>.Map(ex, _logger);
            }
        }

        // TODO: 需要分頁
        public async Task<Result<IReadOnlyList<AdminBookListItemDto>>> GetAdminBookListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetAdminBookListAsync(predicate, orderBy, ct);
                var dtoList = new List<AdminBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<AdminBookListItemDto>(res);
                    dto.CoverImageUrl = BuildImageUrl(res.CoverStorageProvider, res.CoverObjectKey);

                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<AdminBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<AdminBookListItemDto>>.Map(ex, _logger);
            }
        }

        // ========== 促銷標籤相關 ==========

        public async Task<Result<Unit>> AddBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                // 檢查書籍是否存在
                UsedBook? bookWithTagsEntity = await _usedBookRepository.GetEntityByIdWithSaleTagsAsync(bookId, ct);
                if (bookWithTagsEntity == null)
                    return Result<Unit>.Failure("找不到目標書籍", ErrorCodes.General.NotFound);

                // 檢查重複
                if (bookWithTagsEntity.Tags.Any(t => t.Id == tagId))
                    return Result<Unit>.Failure("書籍已經有此銷售標籤", ErrorCodes.General.Conflict);

                // 檢查銷售標籤是否存在
                BookSaleTag? saleTagsEntity = await _saleTagRepository.GetEntityByIdAsync(tagId, ct);
                if (saleTagsEntity == null)
                    return Result<Unit>.Failure("找不到目標銷售標籤", ErrorCodes.General.NotFound);

                bookWithTagsEntity.Tags.Add(saleTagsEntity);

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> RemoveBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _usedBookRepository.RemoveBookSaleTagAsync(bookId, tagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 主題分類相關 ==========



        // ========== 私有方法 ==========

        private Expression<Func<UsedBook, bool>> BuildPredicate(BookListQuery query)
        {
            return b =>
                (
                    string.IsNullOrWhiteSpace(query.BookStatus) ||
                    query.BookStatus == "all" ||
                    query.BookStatus == "unsold" && !b.IsSold ||
                    query.BookStatus == "onshelf" && b.IsOnShelf
                ) &&
                (string.IsNullOrWhiteSpace(query.Keyword) || b.Title.Contains(query.Keyword)) &&
                (!query.MinPrice.HasValue || b.SalePrice >= query.MinPrice) &&
                (!query.MaxPrice.HasValue || b.SalePrice <= query.MaxPrice);
        }

        private Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> BuildOrderBy(BookListQuery query)
        {
            return q => query switch
            {
                _ when query.SortBy == "updated" && query.SortDir == "asc" => q.OrderBy(b => b.UpdatedAt),
                _ when query.SortBy == "updated" && query.SortDir == "desc" => q.OrderByDescending(b => b.UpdatedAt),
                _ when query.SortBy == "created" && query.SortDir == "asc" => q.OrderBy(b => b.CreatedAt),
                _ when query.SortBy == "created" && query.SortDir == "desc" => q.OrderByDescending(b => b.CreatedAt),
                _ when query.SortBy == "price" && query.SortDir == "asc" => q.OrderBy(b => b.SalePrice),
                _ when query.SortBy == "price" && query.SortDir == "desc" => q.OrderByDescending(b => b.SalePrice),
                _ => q.OrderByDescending(b => b.UpdatedAt)
            };
        }

        private string BuildImageUrl(StorageProvider? provider, string? objectKey)
        {
            string? filePath = null;
            if (provider == StorageProvider.Local && objectKey != null)
            {
                filePath = "/" + _imageService.GetThumbRelativePath(objectKey)?.Replace("\\", "/");
            }
            else if (provider == StorageProvider.Cloudinary)
            {
                // TODO: 這裡放 Cloudinary 的處理方式
            }

            return filePath ?? "/images/fallback-thumb.jpg";
        }

    }
}
