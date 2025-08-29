using AutoMapper;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Service;
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
        private readonly LinePayService _linePayService;
        private readonly Random _random;
        private readonly IConfiguration _cfg;
        private readonly IMapper _mapper;
        private readonly ILogger<UsedBookOrderService> _logger;

        public UsedBookOrderService(
            IUnitOfWork unitOfWork,
            UsedBookOrderRepository bookOrderRepository,
            UsedBookRepository bookRepository,
            LinePayService linePayService,
            Random random,
            IConfiguration cfg,
            IMapper mapper,
            ILogger<UsedBookOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookOrderRepository = bookOrderRepository;
            _bookRepository = bookRepository;
            _linePayService = linePayService;
            _cfg = cfg;
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
                orderEntity.GrandTotal = Math.Max(orderEntity.Subtotal - orderEntity.DiscountTotal - orderEntity.DeliveryFee, 0m);
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


                var baseUrl = _cfg["Usedbook:BaseUrl"];
                var state = StateToken.Create(orderEntity.OrderNo, _cfg["Usedbook:PaymentStateSecret"]);

                // 呼叫 LINEPAY
                if (req.PaymentMethod == PaymentMethod.LINEPay)
                {
                    var paymentReq = new LinePayPaymentRequestDto
                    {
                        Amount = (int)orderEntity.GrandTotal,
                        Currency = "TWD",
                        OrderId = orderEntity.OrderNo,
                        Packages = [ new PackageDto {
                            Amount = (int)orderEntity.GrandTotal,
                            Id = "ALL",
                            Products = orderItemEntityList
                                .Select(e => new ProductDto
                                {
                                    Id = e.BookId.ToString(),
                                    Name = e.Title,
                                    Price = (int)e.UnitPrice,
                                    Quantity = e.Quantity,
                                })
                                .ToList(),
                        }],
                        RedirectUrls = new RedirectUrlsDto
                        {
                            ConfirmUrl = $"{baseUrl}/api/usedbooks/payments/linepay/return?state={state}",
                            CancelUrl = $"{baseUrl}/api/usedbooks/payments/linepay/cancel?state={state}",
                        }
                    };
                    _logger.LogWarning("ConfirmUrl={ConfirmUrl}", paymentReq.RedirectUrls.ConfirmUrl);
                    var paymentRes = await _linePayService.RequestLinePayPaymentAsync(paymentReq, ct);
                    if (paymentRes.ReturnCode != "0000")
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<string>.Failure(paymentRes.ReturnMessage, ErrorCodes.General.Unexpected);
                    }
                    orderEntity.TransactionId = paymentRes.Info?.TransactionId;

                    var result = paymentRes.Info?.PaymentUrl?.Web;
                    if (result == null)
                    {
                        await _unitOfWork.RollbackAsync(ct);
                        return Result<string>.Failure("取得 PaymentUrl 失敗", ErrorCodes.General.Unexpected);
                    }

                    await _unitOfWork.CommitAsync(ct);
                    return Result<string>.Success(result);
                }
                return Result<string>.Failure("無對應的結帳功能", ErrorCodes.General.Unexpected);
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
                    if (req.OrderStatus == OrderStatus.Cancelled)
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
                entity.PaymentStatus = req.PaymentStatus == null ? entity.PaymentStatus : (byte)req.PaymentStatus;
                entity.DeliveryStatus = req.DeliveryStatus == null ? entity.DeliveryStatus : (byte)req.DeliveryStatus;
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
