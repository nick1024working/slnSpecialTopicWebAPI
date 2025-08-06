using AutoMapper;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Models;
using System.Data;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class UsedBookImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UsedBookImageRepository _usedBookImageRepository;
        private readonly ILogger<UsedBookImageService> _logger;

        public UsedBookImageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UsedBookImageRepository usedBookImageRepository,
            ILogger<UsedBookImageService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _usedBookImageRepository = usedBookImageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ========== 新增、更新 ==========

        /// <summary>
        /// 使用者建立指定書本的所有書圖片
        /// </summary>
        public async Task<Result<IReadOnlyList<int>>> CreateAsync(
            Guid bookId, List<CreateUsedBookImageRequest> requestList, CancellationToken ct = default)
        {
            // 基本驗證 - 不為空
            if (requestList.Count == 0)
                return Result<IReadOnlyList<int>>.Failure("書圖片清單不可為空", ErrorCodes.General.BadRequest);

            // 基本驗證 - 封面 + Provider + ID
            byte? coverIndex = null;
            for (byte i = 0; i < requestList.Count; ++i)
            {
                var request = requestList[i];

                if (request.IsCover == true)
                {
                    if (coverIndex != null)
                        return Result<IReadOnlyList<int>>.Failure("封面數量必須為一張。", ErrorCodes.General.BadRequest);
                    coverIndex = i;
                }

                if (!Enum.IsDefined(request.StorageProvider))
                    return Result<IReadOnlyList<int>>.Failure("尚未支援的圖片儲存提供者。", ErrorCodes.General.BadRequest);
            }
            if (coverIndex == null)
                return Result<IReadOnlyList<int>>.Failure("封面未指定。", ErrorCodes.General.BadRequest);

            try
            {
                // 準備新增的實體集合
                var entities = new List<UsedBookImage>();

                var utcNow = DateTime.UtcNow;

                for (byte i = 0; i < requestList.Count; ++i)
                {
                    var request = requestList[i];

                    entities.Add(new UsedBookImage
                    { 
                        BookId = bookId,
                        IsCover = i == coverIndex,
                        DisplayOrder = i,
                        StorageProvider = (byte)request.StorageProvider,
                        ObjectKey = request.ObjectKey,
                        Sha256 = Convert.FromBase64String("mZpOErm5t5R1P6zEvW+d+ZzXcW8dHZc52S9tfdlVeFY="),
                        UploadedAt = utcNow
                    });
                }

                _usedBookImageRepository.AddRange(entities);
                await _unitOfWork.CommitAsync(ct);

                var response = entities.Select(e => e.Id).ToList().AsReadOnly();

                return Result<IReadOnlyList<int>>.Success(response);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<int>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 使用者更新指定書本的所有書圖片。
        /// </summary>
        public async Task<Result<Unit>> UpdateOrderByBookIdAsync(
            Guid bookId, IReadOnlyList<UpdateOrderByIdRequest> requestList, CancellationToken ct = default)
        {
            var entityList = await _usedBookImageRepository.GetEntitiesByBookIdAsync(bookId, ct);

            // 檢查數量一致
            if (entityList.Count != requestList.Count)
            {
                await _unitOfWork.RollbackAsync(ct);
                return Result<Unit>.Failure("請求列表與實際二手書圖列表數量不符", ErrorCodes.General.Conflict);
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
                    return Result<Unit>.Failure($"有不合法的二手書圖 Id: {request.Id}", ErrorCodes.General.BadRequest);
                }

                // 檢查重複
                if (!seen.Add(request.Id))
                {
                    await _unitOfWork.RollbackAsync(ct);
                    return Result<Unit>.Failure($"請求列表中二手書圖 Id 重複: {request.Id}", ErrorCodes.General.BadRequest);
                }

                entity.DisplayOrder = order;
                ++order;
            }

            await _unitOfWork.CommitAsync(ct);
            return Result<Unit>.Success(Unit.Value);

        }

        // 上面更改
        /// <summary>
        /// 根據 ImageId 刪除指定書圖片，此操作具備冪等性。
        /// </summary>
        /// <remarks>
        /// 此操作具備冪等性：若資料已存在，將不進行任何變更；若尚未存在，則新增追蹤紀錄。
        /// </remarks>
        public async Task<Result<Unit>> DeleteByImageIdAsync(int imageId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _usedBookImageRepository.RemoveByImageIdAsync(imageId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 查詢 ==========

        /// <summary>
        /// 根據 ImageId 查詢指定書圖片
        /// </summary>
        public async Task<Result<BookImageDto>> GetByImageIdAsync(int imageId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _usedBookImageRepository.GetByImageIdAsync(imageId, ct);
                if (queryResult == null)
                    return Result<BookImageDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var dto = _mapper.Map<BookImageDto>(queryResult);
                return Result<BookImageDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<BookImageDto>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 根據 BookId 查詢所有書圖片，將依照 ImageIndex 升序排序。
        /// </summary>
        public async Task<Result<IReadOnlyList<BookImageDto>>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _usedBookImageRepository.GetByBookIdAsync(bookId, ct);

                var dto = _mapper.Map<IReadOnlyList<BookImageDto>>(queryResult);
                return Result<IReadOnlyList<BookImageDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<BookImageDto>>.Map(ex, _logger);
            }
        }

        // ========== 封面相關 ==========

        /// <summary>
        /// 查詢指定書本的封面圖片
        /// </summary>
        public async Task<Result<BookImageDto>> GetCoverByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _usedBookImageRepository.GetCoverByBookIdAsync(bookId, ct);
                if (queryResult == null)
                    return Result<BookImageDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var dto = _mapper.Map<BookImageDto>(queryResult);
                return Result<BookImageDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<BookImageDto>.Map(ex, _logger);
            }
        }

        // NOTE: 可以考慮不指定書本 ID
        /// <summary>
        /// 設定指定書本的封面圖片
        /// </summary>
        public async Task<Result<Unit>> SetCoverAsync(Guid bookId, SetBookCoverRequest request, CancellationToken ct = default)
        {
            try
            {
                var entities = await _usedBookImageRepository.GetEntitiesByBookIdAsync(bookId, ct);
                if (entities == null ||  !entities.Any(q => q.Id == request.ImageId))
                    return Result<Unit>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                // 更改 entity 設定封面
                foreach (var entity in entities)
                {
                    entity.IsCover = entity.Id == request.ImageId;
                }

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }
    }
}
