using AutoMapper;
using OfficeOpenXml;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
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
        private readonly ImageService _imageService;
        private readonly ILogger<UsedBookService> _logger;

        public UsedBookService(
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
            _imageService = imageService;
            _logger = logger;
        }

        // ========== 新增、更新 ==========

        /// <summary>
        /// 新增完整書本資源，圖片部分交給 ImageService
        /// </summary>
        public async Task<Result<Guid>> CreateAsync(Guid sellerId, CreateBookRequest request, HttpRequest httpRequest, CancellationToken ct = default)
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
                var saveImageResult = await _imageService.SaveImagesAsync(request.ImageList, httpRequest, ct);
                if (!saveImageResult.IsSuccess)
                    throw new Exception(saveImageResult.ErrorMessage);

                var createRequestList = saveImageResult.Value
                    .Select((image, index) => new CreateUsedBookImageRequest
                    {
                        IsCover = index == 0, // 假設第一張圖片為封面
                        StorageProvider = StorageProvider.Local,
                        ObjectKey = image.Id,
                    }).ToList();

                // 此處呼叫 ImageService 來處理封面圖片
                var createImageResult = await _usedBookImageService.CreateAsync(usedBookId, createRequestList, ct);
                if (!createImageResult.IsSuccess)
                    throw new Exception(createImageResult.ErrorMessage);

                await _unitOfWork.CommitAsync(ct);

                return Result<Guid>.Success(usedBookId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Guid>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 更新指定書本資源，圖片部分交給 ImageService
        /// </summary>
        public async Task<Result<Unit>> UpdateAsync(Guid id, UpdateBookRequest request, HttpRequest httpRequest, CancellationToken ct = default)
        {
            using var tx = _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entity = await _usedBookRepository.GetEntityByIdAsync(id, ct);
                if (entity is null)
                    return Result<Unit>.Failure("找不到要更新的書本", ErrorCodes.General.NotFound);

                entity.SellerDistrictId = request.SellerDistrictId;
                entity.SalePrice = request.SalePrice;
                entity.Title = request.Title;
                entity.Authors = request.Authors;
                entity.CategoryId = request.CategoryId;
                entity.ConditionRatingId = request.ConditionRatingId;
                entity.ConditionDescription = request.ConditionDescription;
                entity.Edition = request.Edition;
                entity.Publisher = request.Publisher;
                entity.PublicationDate = request.PublicationDate;
                entity.Isbn = request.Isbn;
                entity.BindingId = request.BindingId;
                entity.LanguageId = request.LanguageId;
                entity.Pages = request.Pages;
                entity.ContentRatingId = request.ContentRatingId;
                entity.IsOnShelf = request.IsOnShelf;
                entity.UpdatedAt = DateTime.UtcNow;

                // 更新圖片
                var ids = new HashSet<int>();
                var updateRequest = new UpdateOrderByIdRequest();
                foreach (var image in request.ImageList)
                {
                    if (image.Id != null)
                    {
                        ids.Add((int)image.Id);
                        updateRequest.IdList.Add((int)image.Id);
                    }
                    else if (image.Image != null)
                    {
                        var saveResult = await _imageService.SaveImageAsync(image.Image, httpRequest, ct);
                        if (!saveResult.IsSuccess)
                            throw new Exception(saveResult.ErrorMessage);

                        var createRequest = new CreateUsedBookImageRequest
                        {
                            IsCover = false,
                            StorageProvider = StorageProvider.Local,
                            ObjectKey = saveResult.Value.Id,
                        };
                        var createResult = await _usedBookImageService.CreateAsync(id, createRequest, ct);
                        if (!createResult.IsSuccess)
                            throw new Exception(createResult.ErrorMessage);
                        ids.Add(createResult.Value);
                        updateRequest.IdList.Add(createResult.Value);
                    }
                }

                await _usedBookImageService.SetCoverAsync(id, new SetBookCoverRequest { ImageId = updateRequest.IdList[0] }, ct);

                var currentList = await _usedBookImageService.GetByBookIdAsync(id, ct);
                if (!currentList.IsSuccess)
                    throw new Exception(currentList.ErrorMessage);
                foreach (var item in currentList.Value)
                {
                    if (!ids.Contains(item.Id))
                        await _usedBookImageService.DeleteByImageIdAsync(item.Id, ct);
                }

                var updateOrderResult = await _usedBookImageService.UpdateOrderByBookIdAsync(id, updateRequest, ct);
                if (!updateOrderResult.IsSuccess)
                    throw new Exception(updateOrderResult.ErrorMessage);

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 更改狀態 ==========

        public async Task<Result<Unit>> UpdateOnShelfStatusAsync(Guid id, UpdateStatusRequest request, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateOnShelfStatusAsync(id, request.Value, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateActiveStatusAsync(Guid id, UpdateStatusRequest request, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateActiveStatusAsync(id, request.Value, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateSoldStatusAsync(Guid id, UpdateStatusRequest request, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateSoldStatusAsync(id, request.Value, ct);
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

        public async Task<Result<PublicUsedBookDetailDto>> GetPublicDetailByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var bookQueryResult = await _usedBookRepository.GetDetailByIdAsync(id, ct);
                if (bookQueryResult == null)
                    return Result<PublicUsedBookDetailDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageResult = await _usedBookImageService.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<PublicUsedBookDetailDto>(bookQueryResult);
                dto.ImageList = imageResult?.Value?.ToList() ?? [];

                return Result<PublicUsedBookDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PublicUsedBookDetailDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<AdminUsedBookDetailDto>> GetAdminDetailByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var bookQueryResult = await _usedBookRepository.GetDetailByIdAsync(id, ct);
                if (bookQueryResult == null)
                    return Result<AdminUsedBookDetailDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageResult = await _usedBookImageService.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<AdminUsedBookDetailDto>(bookQueryResult);
                dto.ImageList = imageResult?.Value?.ToList() ?? [];

                return Result<AdminUsedBookDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<AdminUsedBookDetailDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<UpdateBookPayloadDto>> GetUpdatePayloadByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var entity = await _usedBookRepository.GetEntityByIdWithCountyIdAsync(id, ct);
                if (entity == null)
                    return Result<UpdateBookPayloadDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageResult = await _usedBookImageService.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<UpdateBookPayloadDto>(entity);
                dto.ImageList = imageResult?.Value?.ToList() ?? [];
                dto.SellerCountyId = entity.SellerDistrict.CountyId;

                return Result<UpdateBookPayloadDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<UpdateBookPayloadDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<PagedResult<PublicBookListItemDto>>> GetPublicListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetPublicBookListAsync(predicate, orderBy, query.Paging, ct);
                var itemList = new List<PublicBookListItemDto>();
                foreach (var res in queryResult.Items)
                {
                    var item = _mapper.Map<PublicBookListItemDto>(res);
                    item.CoverImageUrl = _usedBookImageService.GetThumbUrlWithFallback(res.CoverStorageProvider, res.CoverObjectKey);
                    itemList.Add(item);
                }

                var dto = new PagedResult<PublicBookListItemDto>
                {
                    Items = itemList,
                    PageIndex = query.Paging.PageIndex,
                    PageSize = query.Paging.PageSize,
                    TotalRows = queryResult.TotalRows
                };

                return Result<PagedResult<PublicBookListItemDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PagedResult<PublicBookListItemDto>>.Map(ex, _logger);
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
                    dto.CoverImageUrl = _usedBookImageService.GetThumbUrlWithFallback(res.CoverStorageProvider, res.CoverObjectKey);
                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<UserBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<UserBookListItemDto>>.Map(ex, _logger);
            }
        }

        public async Task<Result<PagedResult<AdminBookListItemDto>>> GetAdminBookListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetAdminBookListAsync(predicate, orderBy, query.Paging, ct);
                var itemList = new List<AdminBookListItemDto>();
                foreach (var res in queryResult.Items)
                {
                    var item = _mapper.Map<AdminBookListItemDto>(res);
                    item.CoverImageUrl = _usedBookImageService.GetThumbUrlWithFallback(res.CoverStorageProvider, res.CoverObjectKey);
                    itemList.Add(item);
                }

                var dto = new PagedResult<AdminBookListItemDto>
                {
                    Items = itemList,
                    PageIndex = query.Paging.PageIndex,
                    PageSize = query.Paging.PageSize,
                    TotalRows = queryResult.TotalRows
                };

                return Result<PagedResult<AdminBookListItemDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PagedResult<AdminBookListItemDto>>.Map(ex, _logger);
            }
        }

        // ========== 促銷標籤相關 ==========

        public async Task<Result<Unit>> ApplyBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _usedBookRepository.AddSaleTagAsync(bookId, tagId, ct);
                if (commandResult)
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
                var commandResult = await _usedBookRepository.RemoveSaleTagAsync(bookId, tagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateBookSaleTagBatchAsync(UpdateBookSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var commandResult = request.IsApply ?
                    await _usedBookRepository.AddSaleTagBatchAsync(request.BookIdList, request.TagId, ct) :
                    await _usedBookRepository.RemoveSaleTagBatchAsync(request.BookIdList, request.TagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== Excel ==========

        public IReadOnlyList<BookSaleTag> ImportBooks(Stream stream)
        {
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.FirstOrDefault();
            if (ws == null || ws.Dimension == null)
                return Array.Empty<BookSaleTag>();

            var startRow = 2; // 第1列是標題
            var endRow = ws.Dimension.End.Row;

            var list = new List<BookSaleTag>(capacity: endRow - startRow + 1);

            for (int row = startRow; row <= endRow; row++)
            {
                // 判斷整列是否都是空白
                bool isRowEmpty =
                    string.IsNullOrWhiteSpace(ws.Cells[row, 1].Text) &&
                    string.IsNullOrWhiteSpace(ws.Cells[row, 2].Text) &&
                    string.IsNullOrWhiteSpace(ws.Cells[row, 3].Text) &&
                    string.IsNullOrWhiteSpace(ws.Cells[row, 4].Text) &&
                    string.IsNullOrWhiteSpace(ws.Cells[row, 5].Text);

                if (isRowEmpty) continue;

                // 轉型：盡量用 GetValue<T>()
                int id = ws.Cells[row, 1].GetValue<int>();
                string name = ws.Cells[row, 2].GetValue<string>()?.Trim() ?? string.Empty;
                bool isActive = ParseBool(ws.Cells[row, 3]);        // 支援 1/0, TRUE/FALSE, 是/否
                int displayOrd = ws.Cells[row, 4].GetValue<int>();
                string slug = ws.Cells[row, 5].GetValue<string>()?.Trim() ?? string.Empty;

                list.Add(new BookSaleTag
                {
                    Id = id,
                    Name = name,
                    IsActive = isActive,
                    DisplayOrder = displayOrd,
                    Slug = slug
                });
            }

            return list;
        }


        // ========== 私有方法 ==========

        private Expression<Func<UsedBook, bool>> BuildPredicate(BookListQuery query)
        {
            // 預處理
            var saleTagIds = query.SaleTagIds ?? Array.Empty<int>();
            var keyword = query.Keyword?.Trim();
            var status = query.BookStatus?.Trim().ToLowerInvariant();

            return b =>
                // 主題分類 N:1
                (!query.CategoryId.HasValue || b.CategoryId == query.CategoryId) &&
                // 促標標籤 N:M
                (saleTagIds.Count == 0 || b.Tags.Any(t => saleTagIds.Contains(t.Id))) &&
                // 狀態
                (string.IsNullOrWhiteSpace(status) ||
                    status == "all" ||
                    (status == "inactive" && !b.IsActive) ||
                    (status == "unsold" && !b.IsSold) ||
                    (status == "onshelf" && b.IsOnShelf)) &&
                // 關鍵字
                (string.IsNullOrWhiteSpace(query.Keyword)
                    || b.Title.Contains(query.Keyword)) &&
                // 價格區間
                (!query.MinPrice.HasValue || b.SalePrice >= query.MinPrice) &&
                (!query.MaxPrice.HasValue || b.SalePrice <= query.MaxPrice);
        }

        private Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> BuildOrderBy(BookListQuery query)
        {
            return q => query switch
            {
                _ when query.Paging.SortBy == "updated" && query.Paging.SortDir == "asc" => q.OrderBy(b => b.UpdatedAt),
                _ when query.Paging.SortBy == "updated" && query.Paging.SortDir == "desc" => q.OrderByDescending(b => b.UpdatedAt),
                _ when query.Paging.SortBy == "created" && query.Paging.SortDir == "asc" => q.OrderBy(b => b.CreatedAt),
                _ when query.Paging.SortBy == "created" && query.Paging.SortDir == "desc" => q.OrderByDescending(b => b.CreatedAt),
                _ when query.Paging.SortBy == "price" && query.Paging.SortDir == "asc" => q.OrderBy(b => b.SalePrice),
                _ when query.Paging.SortBy == "price" && query.Paging.SortDir == "desc" => q.OrderByDescending(b => b.SalePrice),
                _ => q.OrderByDescending(b => b.UpdatedAt)
            };
        }

        // 小工具：更彈性的布林轉換
        private static bool ParseBool(ExcelRange cell)
        {
            // 先試著用 GetValue<bool>()
            if (bool.TryParse(cell.Text, out var b)) return b;

            var t = (cell.GetValue<string>() ?? string.Empty).Trim().ToLowerInvariant();
            return t switch
            {
                "1" or "true" or "yes" or "y" or "是" => true,
                "0" or "false" or "no" or "n" or "否" => false,
                _ => cell.GetValue<bool>() // 有時 Value 就是 bool
            };
        }
    }
}
