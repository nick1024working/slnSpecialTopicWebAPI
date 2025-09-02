using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Service;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class UsedBookPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LinePayService _linepayService;
        private readonly UsedBookOrderRepository _orderRepo;
        private readonly IConfiguration _cfg;

        public UsedBookPaymentService(
            IUnitOfWork unitOfWork,
            UsedBookOrderRepository orderRepo,
            LinePayService linePayService,
            IConfiguration cfg
            )
        {
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
            _linepayService = linePayService;
            _cfg = cfg;
        }

        public async Task<Result<string>> ComfirmPaymentAsync(StateQuery query, CancellationToken ct = default)
        {
            var state = StateToken.Parse(query.State, _cfg["Usedbook:PaymentStateSecret"]);
            if (state == null)
                return Result<string>.Failure("[ComfirmPayment] status 不得為空", ErrorCodes.General.BadRequest);

            var entity = await _orderRepo.GetEntityByNoAsync(state.OrderNo, ct);
            if (entity == null)
                return Result<string>.Failure("[ComfirmPayment] 查無訂單", ErrorCodes.General.NotFound);

            var req = new LinePayPaymentConfirmDto { Amount = (int)entity.GrandTotal };
            var cmdRes = await _linepayService.ConfirmLinePayPaymentAsync(entity.TransactionId, req, ct);
            if (cmdRes.ReturnCode != "0000" && cmdRes.ReturnCode != "0110" && cmdRes.ReturnCode != "0123")
            {
                if (cmdRes.ReturnCode == "0121" || cmdRes.ReturnCode == "1180")
                    entity.PaymentStatus = (byte)PaymentStatus.Expired;
                else if (cmdRes.ReturnCode == "0122")
                    entity.PaymentStatus = (byte)PaymentStatus.Failed;

                entity.OrderStatus = (byte)OrderStatus.Processing;
                await _unitOfWork.CommitAsync(ct);
                return Result<string>.Failure("", ErrorCodes.Payment.Unexpected);
            }
            else
            {
                entity.PaymentStatus = (byte)PaymentStatus.Paid;
                entity.OrderStatus = (byte)OrderStatus.Confirmed;

                await _unitOfWork.CommitAsync(ct);
                return Result<string>.Success(state.OrderNo);
            }
        }

        public async Task<Result<string>> CancelPaymentAsync(StateQuery query, CancellationToken ct = default)
        {
            var state = StateToken.Parse(query.State, _cfg["Usedbook:PaymentStateSecret"]);
            if (state == null)
                return Result<string>.Failure("[CancelPaymentAsync] status 不得為空", ErrorCodes.General.BadRequest);

            var entity = await _orderRepo.GetEntityByNoAsync(state.OrderNo, ct);
            if (entity == null)
                return Result<string>.Failure("[ComfirmPayment] 查無訂單", ErrorCodes.General.NotFound);

            entity.PaymentStatus = (byte)PaymentStatus.Failed;
            entity.OrderStatus = (byte)OrderStatus.Cancelled;

            await _unitOfWork.CommitAsync(ct);
            return Result<string>.Success(state.OrderNo);
        }
    }
}
