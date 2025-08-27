using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
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
        private readonly IMapper _mapper;
        private readonly UsedBookOrderRepository _bookOrderyRepository;
        private readonly Random _random;
        private readonly ILogger<UsedBookOrderService> _logger;

        public UsedBookOrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UsedBookOrderRepository bookOrderRepository,
            Random random,
            ILogger<UsedBookOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookOrderyRepository = bookOrderRepository;
            _mapper = mapper;
            _random = random;
            _logger = logger;
        }

        // ========== 新增、更新、刪除 ==========

        /// <summary>
        /// 新增一筆主題分類資料。
        /// </summary>
        public async Task<Result<string>> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var nowTime = DateTime.UtcNow;
                var entity = _mapper.Map<UsedBookOrder>(request);
                entity.OrderStatus = (byte)OrderStatus.Processing;
                entity.PaymentStatus = (byte)PaymentStatus.Unpaid;
                entity.DeliveryStatus = (byte)DeliveryStatus.Preparing;
                entity.CreatedAt = nowTime;
                entity.UpdatedAt = nowTime;

                _bookOrderyRepository.Add(entity);

                await _unitOfWork.CommitAsync(ct);
                var orderNo = $"{nowTime.ToString("yyyyMMddHHmmssfff")}{_random.Next(0, 1000):D3}";
                return Result<string>.Success(orderNo);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<string>.Map(ex, _logger);
            }
        }



        // ========== 查詢 ==========


    }
}
