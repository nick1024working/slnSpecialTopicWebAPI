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
    public class BookCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BookCategoryRepository _bookCategoryRepository;
        private readonly ILogger<BookCategoryService> _logger;

        public BookCategoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            BookCategoryRepository bookCategoryRepository,
            ILogger<BookCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookCategoryRepository = bookCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ========== 新增、更新、刪除 ==========

        /// <summary>
        /// 新增一筆主題分類資料。
        /// </summary>
        public async Task<Result<int>> CreateAsync(CreateBookCategoryRequest request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                if (await _bookCategoryRepository.ExistsByNameAsync(request.Name, ct))
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<int>.Failure("名稱不能重複", ErrorCodes.General.Conflict);
                }

                var entity = _mapper.Map<BookCategory>(request);
                entity.DisplayOrder = await _bookCategoryRepository.HasRecords(ct) ?
                    await _bookCategoryRepository.GetMaxDisplayOrderAsync(ct) + 1 : 1;
                entity.Slug = Guid.NewGuid().ToString();

                _bookCategoryRepository.Add(entity);

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
        /// 依照 ID 刪除主題分類資料，此方法具冪等性。
        /// </summary>
        public async Task<Result<Unit>> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var cmdResult = await _bookCategoryRepository.RemoveByIdAsync(id, ct);
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
        /// 依照 ID 更新主題分類資料，不能更新排序。
        /// </summary>
        // HACK: 需考慮 Name, Slug 可能會重複的情況
        public async Task<Result<Unit>> UpdateByIdAsync(int id, UpdatePartialBookCategoryRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = await _bookCategoryRepository.GetEntityByIdAsync(id, ct);
                if (entity is null)
                    return Result<Unit>.Failure("找不到要更新的主題分類", ErrorCodes.General.NotFound);

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
        /// 更新所有主題分類順序。
        /// </summary>
        public async Task<Result<Unit>> UpdateAllOrderAsync(UpdateOrderByIdRequest request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entityList = await _bookCategoryRepository.GetEntityListAsync(ct);

                // 檢查數量一致
                if (entityList.Count != request.IdList.Count)
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<Unit>.Failure("請求列表與實際主題分類數量不符", ErrorCodes.General.Conflict);
                }

                var entityDict = entityList.ToDictionary(x => x.Id);
                var seen = new HashSet<int>();

                int order = 1;
                foreach (var id in request.IdList)
                {
                    // 檢查存在
                    if (!entityDict.TryGetValue(id, out var entity))
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<Unit>.Failure($"有不合法的主題分類 Id: {id}", ErrorCodes.General.BadRequest);
                    }

                    // 檢查重複
                    if (!seen.Add(id))
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<Unit>.Failure($"請求列表中主題分類 Id 重複: {id}", ErrorCodes.General.BadRequest);
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

        public async Task<Result<BookCategoryDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookCategoryRepository.GetByIdAsync(id, ct);
                if (queryResult == null)
                    return Result<BookCategoryDto>.Failure("查無符合資料", ErrorCodes.General.NotFound);
                var dto = _mapper.Map<BookCategoryDto>(queryResult);

                return Result<BookCategoryDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<BookCategoryDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<BookCategoryDto>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookCategoryRepository.GetAllAsync(ct);
                var dtos = _mapper.Map<IReadOnlyList<BookCategoryDto>>(queryResult);

                return Result<IReadOnlyList<BookCategoryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<BookCategoryDto>>.Map(ex, _logger);
            }
        }

    }
}
