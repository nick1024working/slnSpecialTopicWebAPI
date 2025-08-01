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
        /// 新增促銷標籤。
        /// </summary>
        public async Task<Result<int>> CreateAsync(CreateSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = new BookSaleTag
                {
                    Name = request.SaleTagName,
                };

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
        /// 更新所有促銷標籤。
        /// </summary>
        public async Task<Result<IReadOnlyList<int>>> UpdateAllAsync(IEnumerable<UpdateBookSaleTagRequest> requestList, CancellationToken ct = default)
        {
            var entityList = requestList
                .Select(x => new BookSaleTag
                {
                    Name = x.SaleTagName,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = true
                })
                .ToList();

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await _bookSaleTagRepository.UpdateAllAsync(entityList, ct);
                await _unitOfWork.CommitAsync(ct);

                var result = entityList.Select(x => x.Id).ToList();

                return Result<IReadOnlyList<int>>.Success(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<IReadOnlyList<int>>.Map(ex, _logger);
            }
        }


        /// <summary>
        /// 跟新指定促銷標籤啟用狀態，此操作具備冪等性。
        /// </summary>
        public async Task<Result<Unit>> UpdateActiveStatusAsync(int saleTagId, bool isActive, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _bookSaleTagRepository.UpdateActiveStatusAsync(saleTagId, isActive, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 依照 Id 更新促銷標籤。
        /// </summary>
        public async Task<Result<Unit>> UpdateSaleTagNameAsync(
            int saleTagId, UpdateBookSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = new BookSaleTag
                {
                    Id = saleTagId,
                    Name = request.SaleTagName,
                };

                var commandResult = await _bookSaleTagRepository.UpdateAsync(entity, ct);
                if (commandResult == false)
                    return Result<Unit>.Failure("找不到要更新的促銷標籤", ErrorCodes.General.NotFound);

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 根據 Id 查詢銷售標籤。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
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

        /// <summary>
        /// 查詢所有促銷標籤。
        /// </summary>
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
