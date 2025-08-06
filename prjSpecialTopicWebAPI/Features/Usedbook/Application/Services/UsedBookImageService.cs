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
                        ImageIndex = i,
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
        /// <remarks>此處暫不考慮效能。</remarks>
        public async Task<Result<IReadOnlyList<int>>> UpdateAsync(
            Guid bookId, List<UpdateUsedBookImageRequest> requestList, CancellationToken ct = default)
        {
            // 取出當前追蹤實體集合 + 建表
            var entities = await _usedBookImageRepository.GetEntitiesByBookIdAsync(bookId, ct);
            var existingImageIds = entities.Select(e => e.Id).ToHashSet();

            // 基本驗證 - 不為空
            if (requestList.Count == 0)
                return Result<IReadOnlyList<int>>.Failure("書圖片清單不可為空", ErrorCodes.General.BadRequest);

            // 基本驗證 - 封面 + Provider + ID
            byte? coverIndex = null;
            for (byte i = 0; i < requestList.Count; ++i)
            {
                var request = requestList[i];

                // ID 為 null 視作新增
                if (request.Id == null && !Enum.IsDefined(request.StorageProvider))
                    return Result<IReadOnlyList<int>>.Failure("尚未支援的圖片儲存提供者。", ErrorCodes.General.BadRequest);

                // ID 不為 null 視作修改
                if (request.Id != null && !existingImageIds.Contains(request.Id.Value))
                    return Result<IReadOnlyList<int>>.Failure("書圖片 ID 不正確", ErrorCodes.General.BadRequest);

                if (request.IsCover == true)
                {
                    if (coverIndex != null)
                        return Result<IReadOnlyList<int>>.Failure("封面數量必須為一張。", ErrorCodes.General.BadRequest);
                    coverIndex = i;
                }

            }
            if (coverIndex == null)
            {
                _logger.LogInformation("使用者未指定封面，預設設第一張為封面。");
                coverIndex = 0;
            }

            // 準備覆蓋的實體集合
            var toAddEntities = new List<UsedBookImage>();

            var utcNow = DateTime.UtcNow;

            // 遍歷 requestList，更新或沿用實體，把其加入 toAddEntities
            for (byte i = 0; i < requestList.Count; ++i)
            {
                var request = requestList[i];

                // 請求不帶有 imageId 表示新增
                if (request.Id == null)
                {
                    toAddEntities.Add(new UsedBookImage
                    {
                        BookId = bookId,
                        IsCover = i == coverIndex,
                        ImageIndex = i,
                        StorageProvider = (byte)request.StorageProvider,
                        ObjectKey = request.ObjectKey,
                        Sha256 = Convert.FromBase64String("mZpOErm5t5R1P6zEvW+d+ZzXcW8dHZc52S9tfdlVeFY="),
                        UploadedAt = utcNow
                    });
                }
                // 請求帶有 imageId 表示嘗試更新，注意基本驗證時就已經把不存在的情形駁回
                else
                {
                    var entity = entities.Single(e => e.Id == request.Id);
                    toAddEntities.Add(new UsedBookImage
                    {
                        BookId = bookId,
                        IsCover = i == coverIndex,
                        ImageIndex = i,
                        StorageProvider = entity.StorageProvider,
                        ObjectKey = entity.ObjectKey,
                        Sha256 = entity.Sha256,
                        UploadedAt = entity.UploadedAt
                    });
                }
            }

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                _usedBookImageRepository.RemoveRange(entities);
                _usedBookImageRepository.AddRange(toAddEntities);

                await _unitOfWork.CommitAsync(ct);

                // 組裝回應
                var response = toAddEntities
                    .Select(e => e.Id)
                    .ToList()
                    .AsReadOnly();

                return Result<IReadOnlyList<int>>.Success(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<IReadOnlyList<int>>.Map(ex, _logger);
            }
        }

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
        public async Task<Result<IEnumerable<BookImageDto>>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _usedBookImageRepository.GetByBookIdAsync(bookId, ct);

                var dto = _mapper.Map<IEnumerable<BookImageDto>>(queryResult);
                return Result<IEnumerable<BookImageDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IEnumerable<BookImageDto>>.Map(ex, _logger);
            }
        }

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
