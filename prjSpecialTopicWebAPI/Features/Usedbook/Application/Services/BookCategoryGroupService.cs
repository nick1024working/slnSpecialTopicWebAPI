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
    public class BookCategoryGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BookCategoryGroupRepository _bookCategoryGroupRepository;
        private readonly ILogger<BookCategoryGroupService> _logger;

        public BookCategoryGroupService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            BookCategoryGroupRepository bookCategoryGroupRepository,
            ILogger<BookCategoryGroupService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookCategoryGroupRepository = bookCategoryGroupRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ========== 新增、更新、刪除 ==========

        /// <summary>
        /// 新增一筆主題分類群資料。
        /// </summary>
        public async Task<Result<int>> CreateAsync(CreateBookCategoryGroupRequest request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                if (await _bookCategoryGroupRepository.ExistsByNameAsync(request.Name, ct))
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<int>.Failure("名稱不能重複", ErrorCodes.General.Conflict);
                }

                var entity = _mapper.Map<BookCategoryGroup>(request);
                entity.DisplayOrder = await _bookCategoryGroupRepository.HasRecords(ct) ?
                    await _bookCategoryGroupRepository.GetMaxDisplayOrderAsync(ct) + 1 : 1;
                entity.Slug = Guid.NewGuid().ToString();

                _bookCategoryGroupRepository.Add(entity);

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
        /// 依照 ID 刪除主題分類群資料，此方法具冪等性。
        /// </summary>
        public async Task<Result<Unit>> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var cmdResult = await _bookCategoryGroupRepository.RemoveByIdAsync(id, ct);
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
        /// 依照 ID 更新主題分類群資料，不能更新排序。
        /// </summary>
        // HACK: 需考慮 Name, Slug 可能會重複的情況
        public async Task<Result<Unit>> UpdateByIdAsync(int id, UpdatePartialBookCategoryGroupRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = await _bookCategoryGroupRepository.GetEntityByIdAsync(id, ct);
                if (entity is null)
                    return Result<Unit>.Failure("找不到要更新的主題分類群", ErrorCodes.General.NotFound);

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
        /// 更新所有主題分類群順序。
        /// </summary>
        public async Task<Result<Unit>> UpdateAllOrderAsync(IReadOnlyList<UpdateOrderByIdRequest> requestList, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entityList = await _bookCategoryGroupRepository.GetEntityListAsync(ct);

                // 檢查數量一致
                if (entityList.Count != requestList.Count)
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<Unit>.Failure("請求列表與實際主題分類群數量不符", ErrorCodes.General.Conflict);
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
                        return Result<Unit>.Failure($"有不合法的主題分類群 Id: {request.Id}", ErrorCodes.General.BadRequest);
                    }

                    // 檢查重複
                    if (!seen.Add(request.Id))
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<Unit>.Failure($"請求列表中主題分類群 Id 重複: {request.Id}", ErrorCodes.General.BadRequest);
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

        public async Task<Result<BookCategoryGroupDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookCategoryGroupRepository.GetByIdAsync(id, ct);
                if (queryResult == null)
                    return Result<BookCategoryGroupDto>.Failure("查無符合資料", ErrorCodes.General.NotFound);
                var dto = _mapper.Map<BookCategoryGroupDto>(queryResult);

                return Result<BookCategoryGroupDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<BookCategoryGroupDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<BookCategoryGroupDto>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookCategoryGroupRepository.GetAllAsync(ct);
                var dtos = _mapper.Map<IReadOnlyList<BookCategoryGroupDto>>(queryResult);

                return Result<IReadOnlyList<BookCategoryGroupDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<BookCategoryGroupDto>>.Map(ex, _logger);
            }
        }

    }
}
