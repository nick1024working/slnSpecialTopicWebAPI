using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
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

        // ========== 新增、更新、刪除 ==========

        /// <summary>
        /// 新增一筆促銷標籤資料。
        /// </summary>
        public async Task<Result<int>> CreateAsync(CreateSaleTagRequest request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                if (await _bookSaleTagRepository.ExistsByNameAsync(request.Name, ct))
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<int>.Failure("名稱不能重複", ErrorCodes.General.Conflict);
                }

                var entity = _mapper.Map<BookSaleTag>(request);
                entity.DisplayOrder = await _bookSaleTagRepository.HasRecords(ct) ?
                    await _bookSaleTagRepository.GetMaxDisplayOrderAsync(ct) + 1 : 1;
                entity.Slug = request.Name;

                _bookSaleTagRepository.Add(entity);

                await _unitOfWork.CommitAsync(ct);
                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<int>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 依照 ID 刪除促銷標籤資料，此方法具冪等性。
        /// </summary>
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
        // HACK: 需考慮 Name, Slug 可能會重複的情況
        public async Task<Result<Unit>> UpdateByIdAsync(int id, UpdatePartialBookSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = await _bookSaleTagRepository.GetEntityByIdAsync(id, ct);
                if (entity is null)
                    return Result<Unit>.Failure("找不到要更新的促銷標籤", ErrorCodes.General.NotFound);

                entity.Name = request.Name ?? entity.Name;
                entity.IsActive = request.IsActive ?? entity.IsActive;
                entity.Slug = request.Slug ?? entity.Slug;

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 更新所有促銷標籤順序。
        /// </summary>
        public async Task<Result<Unit>> UpdateAllOrderAsync(IReadOnlyList<UpdateOrderByIdRequest> requestList, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entityList = await _bookSaleTagRepository.GetEntityListAsync(ct);

                // 檢查數量一致
                if (entityList.Count != requestList.Count)
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<Unit>.Failure("請求列表與實際促銷標籤數量不符", ErrorCodes.General.Conflict);
                }

                var entityDict = entityList.ToDictionary(x => x.Id);
                var seen = new HashSet<int>();

                int order = 1;
                foreach (var request in requestList)
                {
                    // 檢查存在
                    if (!entityDict.TryGetValue(request.Id, out var entity))
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<Unit>.Failure($"有不合法的促銷標籤 Id: {request.Id}", ErrorCodes.General.BadRequest);
                    }

                    // 檢查重複
                    if (!seen.Add(request.Id))
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<Unit>.Failure($"請求列表中促銷標籤 Id 重複: {request.Id}", ErrorCodes.General.BadRequest);
                    }

                    entity.DisplayOrder = order;
                    ++order;
                }

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
