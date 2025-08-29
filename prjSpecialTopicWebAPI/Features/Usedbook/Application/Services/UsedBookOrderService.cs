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
        private readonly UsedBookOrderRepository _bookOrderRepository;
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
            _bookOrderRepository = bookOrderRepository;
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
                // 基本驗證
                if (req.BookIdList.Count <= 0)
                    return Result<string>.Failure("書本清單不能為空", ErrorCodes.General.BadRequest);

                var nowTime = DateTime.UtcNow;
                var orderNo = $"{nowTime.ToString("yyyyMMddHHmmssfff")}{_random.Next(0, 1000):D3}";
                var orderEntity = _mapper.Map<UsedBookOrder>(req);

                // 去重
                var bookIds = req.BookIdList.Distinct().ToList();

                // 驗證 + 取出書本 entity (後續要改狀態)
                var bookEntityList = new List<UsedBook>();
                foreach (var bookId in req.BookIdList)
                {
                    var bookEntity = await _bookRepository.GetEntityByIdAsync(bookId, ct);
                    if (bookEntity?.SellerId != req.SellerId)
                        return Result<string>.Failure("書本賣家不符", ErrorCodes.General.BadRequest);
                    if (bookEntity.IsSold || !bookEntity.IsActive || !bookEntity.IsOnShelf)
                        return Result<string>.Failure("書本狀態不可售", ErrorCodes.General.BadRequest);

                    bookEntityList.Add(bookEntity);
                    orderEntity.Subtotal += bookEntity.SalePrice;
                }

                // 組裝訂單本體 entity
                orderEntity.OrderNo = orderNo;
                orderEntity.BuyerId = buyerId;
                orderEntity.SellerId = req.SellerId;
                orderEntity.OrderStatus = (byte)OrderStatus.Pending;
                orderEntity.PaymentStatus = (byte)PaymentStatus.Unpaid;
                orderEntity.DeliveryStatus = (byte)DeliveryStatus.Preparing;
                orderEntity.PaymentMethod = (byte)req.PaymentMethod;
                orderEntity.DeliveryMethod = (byte)req.DeliveryMethod;
                orderEntity.DiscountTotal = 0m;
                orderEntity.DeliveryFee = DeliveryMapper.ToFee[req.DeliveryMethod];
                orderEntity.GrandTotal = Math.Min(orderEntity.Subtotal - orderEntity.DiscountTotal - orderEntity.DeliveryFee, 0m);
                orderEntity.CreatedAt = nowTime;
                orderEntity.UpdatedAt = nowTime;

                _bookOrderRepository.AddOrder(orderEntity);

                // 組裝訂單商品 entity
                var orderItemEntityList = new List<UsedBookOrderItem>();
                foreach (var bookEntity in bookEntityList)
                {
                    var orderItemEntity = new UsedBookOrderItem
                    {
                        Order = orderEntity,
                        BookId = bookEntity.Id,
                        Title = bookEntity.Title,
                        UnitPrice = bookEntity.SalePrice,
                        Quantity = 1
                    };

                    bookEntity.IsOnShelf = false;
                    bookEntity.IsSold = true;

                    orderItemEntityList.Add(orderItemEntity);
                }
                _bookOrderRepository.AddRangeOrderItems(orderItemEntityList);
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
                var entity = await _bookOrderRepository.GetEntityByNoAsync(orderNo, ct);
                if (entity == null)
                    return Result<Unit>.Failure("找不到該訂單資源", ErrorCodes.General.NotFound);

                if (req.OrderStatus != null)
                {
                    if (req.OrderStatus == (byte)OrderStatus.Cancelled)
                    {
                        foreach (var item in entity.UsedBookOrderItems)
                        {
                            var bookEntity = await _bookRepository.GetEntityByIdAsync(item.BookId, ct);
                            if (bookEntity == null)
                                return Result<Unit>.Failure("訂單查無書本實體", ErrorCodes.General.Conflict);
                            bookEntity.IsSold = false;
                            bookEntity.IsOnShelf = true;
                        }
                    }
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
                var queryResult = await _bookOrderRepository.GetSellerOrderListAsync(userId, ct);
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
                var queryResult = await _bookOrderRepository.GetBuyerOrderListAsync(userId, ct);
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
