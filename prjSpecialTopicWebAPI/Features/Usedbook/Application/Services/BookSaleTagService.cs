using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Usedbook.Application.Services
{
    public class BookSaleTagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BookSaleTagRepository _bookSaleTagRepository;
        private readonly ILogger<BookSaleTagService> _logger;

        public BookSaleTagService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            BookSaleTagRepository bookSaleTagRepository,
            ILogger<BookSaleTagService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookSaleTagRepository = bookSaleTagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 新增一筆促銷標籤資料。
        /// </summary>
        // TODO: 需考慮交易完整性、名稱 UNIQUE
        public async Task<Result<int>> CreateAsync(CreateSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = _mapper.Map<BookSaleTag>(request);
                entity.DisplayOrder = await _bookSaleTagRepository.CountAsync(ct) + 1;
                _bookSaleTagRepository.Add(entity);

                await _unitOfWork.CommitAsync(ct);
                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<int>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 依照 ID 刪除促銷標籤資料，此方法具冪等性。
        /// </summary>
        // TODO: 須考慮書本關聯性
        public async Task<Result<Unit>> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var cmdResult = await _bookSaleTagRepository.RemoveByIdAsync(id, ct);
                if (cmdResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 依照 ID 更新促銷標籤資料，不能更新排序。
        /// </summary>
        public async Task<Result<Unit>> UpdateByIdAsync(int id, UpdatePartialBookSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = await _bookSaleTagRepository.GetEntityByIdAsync(id, ct);
                if (entity is null)
                    return Result<Unit>.Failure("找不到要更新的促銷標籤", ErrorCodes.General.NotFound);

                entity.Name = request.Name ?? entity.Name;
                entity.IsActive = request.IsActive ?? entity.IsActive;

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 批次更新所有促銷標籤資料，與當前所有促銷標籤資料進行比較。
        /// </summary>
        public async Task<Result<Unit>> UpdateAllAsync(IEnumerable<UpdateBookSaleTagRequest> requestList, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entityList = await _bookSaleTagRepository.GetEntityListAsync(ct);
                var entityDict = entityList.ToDictionary(x => x.Id);
                var idSet = entityList.Select(x => x.Id).ToHashSet();
                var removeList = new List<BookSaleTag>();
                var addList = new List<BookSaleTag>();

                // 遍歷 requestList，將新增和更新的項目分別加入到 addList 和 entityList 中
                int order = 1;
                foreach (var request in requestList)
                {
                    // 無 Id 代表要新增
                    if (request.Id is not int id)
                    {
                        addList.Add(new BookSaleTag
                        {
                            Name = request.Name,
                            IsActive = true,
                            DisplayOrder = order,
                        });
                    }
                    // 有 Id 代表要更新
                    else if (entityDict.TryGetValue(id, out var entity))
                    {
                        entity.Name = request.Name;
                        entity.IsActive = request.IsActive;
                        entity.DisplayOrder = order;
                    }
                    // 非法 Id 整批駁回
                    else
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<Unit>.Failure("有不合法的促銷標籤 Id，請確認後再試", ErrorCodes.General.BadRequest);
                    }
                    ++order;
                }

                // 處理 entityList 中不在 requestList 中的項目
                idSet = requestList.Where(r => r.Id.HasValue).Select(r => r.Id.Value).ToHashSet();
                removeList = entityList.Where(e => !idSet.Contains(e.Id)).ToList();

                // 執行
                _bookSaleTagRepository.AddRange(addList);
                _bookSaleTagRepository.RemoveRange(removeList);
                await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 查詢 ==========

        public async Task<Result<BookSaleTagDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookSaleTagRepository.GetByIdAsync(id, ct);
                if (queryResult == null)
                    return Result<BookSaleTagDto>.Failure("查無符合資料", ErrorCodes.General.NotFound);
                var dto = _mapper.Map<BookSaleTagDto>(queryResult);

                return Result<BookSaleTagDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<BookSaleTagDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<BookSaleTagDto>>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookSaleTagRepository.GetByBookIdAsync(bookId, ct);
                if (queryResult == null)
                    return Result<IReadOnlyList<BookSaleTagDto>>.Failure("查無符合資料", ErrorCodes.General.NotFound);
                var dto = _mapper.Map<IReadOnlyList<BookSaleTagDto>>(queryResult);

                return Result<IReadOnlyList<BookSaleTagDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<BookSaleTagDto>>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<BookSaleTagDto>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookSaleTagRepository.GetAllAsync(ct);
                var dtos = _mapper.Map<IReadOnlyList<BookSaleTagDto>>(queryResult);

                return Result<IReadOnlyList<BookSaleTagDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<BookSaleTagDto>>.Map(ex, _logger);
            }
        }

    }
}
