using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Usedbook.Application.Services
{
    public class UsedBookOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UsedBookOrderRepository _bookOrderyRepository;
        private readonly UsedBookRepository _bookRepository;
        private readonly Random _random;
        private readonly IMapper _mapper;
        private readonly ILogger<UsedBookOrderService> _logger;

        public UsedBookOrderService(
            IUnitOfWork unitOfWork,
            UsedBookOrderRepository bookOrderRepository,
            UsedBookRepository bookRepository,
            Random random,
            IMapper mapper,
            ILogger<UsedBookOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookOrderyRepository = bookOrderRepository;
            _bookRepository = bookRepository;
            _random = random;
            _mapper = mapper;
            _logger = logger;
        }

        // ========== 新增、更新、刪除 ==========

        public async Task<Result<string>> CreateAsync(Guid buyerId, CreateOrderRequest req, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var bookEntity = await _bookRepository.GetEntityByIdAsync(req.BookId, ct);
                if (bookEntity == null)
                    return Result<string>.Failure("錯誤的書本 ID", ErrorCodes.General.BadRequest);
                var nowTime = DateTime.UtcNow;
                var entity = _mapper.Map<UsedBookOrder>(req);
                var orderNo = $"{nowTime.ToString("yyyyMMddHHmmssfff")}{_random.Next(0, 1000):D3}";
                entity.OrderNo = orderNo;
                entity.OrderStatus = (byte)OrderStatus.Processing;
                entity.PaymentStatus = (byte)PaymentStatus.Unpaid;
                entity.DeliveryStatus = (byte)DeliveryStatus.Preparing;
                entity.PaymentMethod = (byte)req.PaymentMethod;
                entity.DeliveryMethod = (byte)req.DeliveryMethod;
                entity.BuyerId = buyerId;
                entity.SellerId = bookEntity.SellerId;
                entity.BookId = req.BookId;
                entity.Title = bookEntity.Title;
                entity.CreatedAt = nowTime;
                entity.UpdatedAt = nowTime;

                _bookOrderyRepository.Add(entity);
                bookEntity.IsOnShelf = false;
                bookEntity.IsSold = true;

                await _unitOfWork.CommitAsync(ct);
                return Result<string>.Success(orderNo);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<string>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateOrderStatusAsync(string orderNo, UpdateOrderStatusRequest req, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entity = await _bookOrderyRepository.GetEntityByNoAsync(orderNo, ct);
                if (entity == null)
                    return Result<Unit>.Failure("找不到該訂單資源", ErrorCodes.General.NotFound);

                var bookEntity = await _bookRepository.GetEntityByIdAsync(entity.BookId, ct);
                if (bookEntity == null)
                    return Result<Unit>.Failure("找不到該書本資源", ErrorCodes.General.NotFound);

                if (req.OrderStatus != null)
                {
                    if (req.OrderStatus == (byte)OrderStatus.Cancelled)
                        bookEntity.IsSold = false;
                    entity.OrderStatus = (byte)req.OrderStatus;
                }
                entity.PaymentStatus = req.PaymentStatus ?? entity.PaymentStatus;
                entity.DeliveryStatus = req.DeliveryStatus ?? entity.DeliveryStatus;
                entity.UpdatedAt = DateTime.UtcNow;

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

        public async Task<Result<IReadOnlyList<UserOrderListItemDto>>> GetSellerOrderListAsync(Guid userId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookOrderyRepository.GetSellerOrderListAsync(userId, ct);
                var dtoList = _mapper.Map<IReadOnlyList<UserOrderListItemDto>>(queryResult);
                return Result<IReadOnlyList<UserOrderListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<UserOrderListItemDto>>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<UserOrderListItemDto>>> GetBuyerOrderListAsync(Guid userId, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookOrderyRepository.GetBuyerOrderListAsync(userId, ct);
                var dtoList = _mapper.Map<IReadOnlyList<UserOrderListItemDto>>(queryResult);
                return Result<IReadOnlyList<UserOrderListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<UserOrderListItemDto>>.Map(ex, _logger);
            }
        }

    }
}
