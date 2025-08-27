using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class ExternalDomainService
    {
        private readonly ExternalDomainRepository _externalDomainRepository;
        private readonly ILogger<ExternalDomainService> _logger;

        public ExternalDomainService(ExternalDomainRepository externalDomainRepository, ILogger<ExternalDomainService> logger)
        {
            _externalDomainRepository = externalDomainRepository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<Guid>>> GetSellerListAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _externalDomainRepository.GetSellerListAsync(ct);
                return Result<IReadOnlyList<Guid>>.Success(result);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<Guid>>.Map(ex, _logger);
            }
        }

    }
}
