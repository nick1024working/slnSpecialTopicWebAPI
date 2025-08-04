using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class LookupService
    {
        private readonly BookBindingRepository _bookBindingRepository;
        private readonly BookConditionRatingRepository _bookConditionRatingRepository;
        private readonly ContentRatingRepository _contentRatingRepository;
        private readonly CountyRepository _countyRepository;
        private readonly DistrictRepository _districtRepository;
        private readonly LanguageRepository _languageRepository;
        private readonly ILogger<LookupService> _logger;

        public LookupService(
            BookBindingRepository bookBindingRepository,
            BookConditionRatingRepository bookConditionRatingRepository,
            ContentRatingRepository contentRatingRepository,
            CountyRepository countyRepository,
            DistrictRepository districtRepository,
            LanguageRepository languageRepository,
            ILogger<LookupService> logger)
        {
            _bookBindingRepository = bookBindingRepository;
            _bookConditionRatingRepository = bookConditionRatingRepository;
            _contentRatingRepository = contentRatingRepository;
            _countyRepository = countyRepository;
            _districtRepository = districtRepository;
            _languageRepository = languageRepository;
            _logger = logger;
        }

        // TODO: 考慮平行處理的可能性。
        /// <summary>
        /// 讀取所有 UsedBook需要的下拉選單資料。
        /// </summary>
        public async Task<Result<AllUsedBookLookupListsDto>> GetUsedBookUILookupsList(CancellationToken ct = default)
        {
            try
            {
                var bookBindingResult = await GetBookBindingListAsync(ct);
                if (!bookBindingResult.IsSuccess)
                    return Result<AllUsedBookLookupListsDto>.Failure("Lookup.GetBookBindingListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var bookConditionResult = await GetBookConditionRatingListAsync(ct);
                if (!bookConditionResult.IsSuccess)
                    return Result<AllUsedBookLookupListsDto>.Failure("Lookup.GetBookConditionRatingListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var contentRatingResult = await GetContentRatingListAsync(ct);
                if (!contentRatingResult.IsSuccess)
                    return Result<AllUsedBookLookupListsDto>.Failure("Lookup.GetContentRatingListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var countyResult = await GetCountyListAsync(ct);
                if (!countyResult.IsSuccess)
                    return Result<AllUsedBookLookupListsDto>.Failure("Lookup.GetCountyListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var languageResult = await GetLanguageListAsync(ct);
                if (!languageResult.IsSuccess)
                    return Result<AllUsedBookLookupListsDto>.Failure("Lookup.GetLanguageListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var dto = new AllUsedBookLookupListsDto
                {
                    BookBindings = bookBindingResult.Value,
                    BookConditionRatings = bookConditionResult.Value,
                    ContentRatings = contentRatingResult.Value,
                    Counties = countyResult.Value,
                    Languages = languageResult.Value
                };

                return Result<AllUsedBookLookupListsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<AllUsedBookLookupListsDto>.Map(ex, _logger, nameof(GetBookConditionRatingListAsync));
            }
        }

        /// <summary>
        /// 讀取所有 BookBinding，並轉換為 <see cref="IdNameDto"/> 物件列表。
        /// </summary>
        public async Task<Result<IEnumerable<IdNameDto>>> GetBookBindingListAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _bookBindingRepository.GetAllAsync(ct);
                var dtoList = result.Select(x => new IdNameDto { Id = x.Id, Name = x.Name }).ToList();
                return Result<IEnumerable<IdNameDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IEnumerable<IdNameDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 BookConditionRating，並轉換為 <see cref="IdNameDto"/> 物件列表。
        /// </summary>
        public async Task<Result<IEnumerable<IdNameDto>>> GetBookConditionRatingListAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _bookConditionRatingRepository.GetAllAsync(ct);
                var dtoList = result.Select(x => new IdNameDto { Id = x.Id, Name = x.Name }).ToList();
                return Result<IEnumerable<IdNameDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IEnumerable<IdNameDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 ContentRating，並轉換為 <see cref="IdNameDto"/> 物件列表。
        /// </summary>
        public async Task<Result<IEnumerable<IdNameDto>>> GetContentRatingListAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _contentRatingRepository.GetAllAsync(ct);
                var dtoList = result.Select(x => new IdNameDto { Id = x.Id, Name = x.Name }).ToList();
                return Result<IEnumerable<IdNameDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IEnumerable<IdNameDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 County，並轉換為 <see cref="IdNameDto"/> 物件列表。
        /// </summary>
        public async Task<Result<IEnumerable<IdNameDto>>> GetCountyListAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _countyRepository.GetAllAsync(ct);
                var dtoList = result.Select(x => new IdNameDto { Id = x.Id, Name = x.Name }).ToList();
                return Result<IEnumerable<IdNameDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IEnumerable<IdNameDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 District，並轉換為 <see cref="IdNameDto"/> 物件列表。
        /// </summary>
        public async Task<Result<IEnumerable<IdNameDto>>> GetDistrictListByCountyIdAsync(int countyId, CancellationToken ct = default)
    {
        try
        {
            var result = await _districtRepository.GetByCountyIdAsync(countyId, ct);
            var dtoList = result.Select(x => new IdNameDto { Id = x.Id, Name = x.Name }).ToList();
            return Result<IEnumerable<IdNameDto>>.Success(dtoList);
        }
        catch (Exception ex)
        {
            return ExceptionToErrorResultMapper<IEnumerable<IdNameDto>>.Map(ex, _logger);
        }
    }

        /// <summary>
        /// 讀取所有 Language，並轉換為 <see cref="IdNameDto"/> 物件列表。
        /// </summary>
        public async Task<Result<IEnumerable<IdNameDto>>> GetLanguageListAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _languageRepository.GetAllAsync(ct);
                var dtoList = result.Select(x => new IdNameDto { Id = x.Id, Name = x.Name }).ToList();
                return Result<IEnumerable<IdNameDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IEnumerable<IdNameDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取 BookConditionRatingDescription。
        /// </summary>
        public async Task<Result<BookConditionRatingDescriptionDto>> GetBookConditionRatingDescriptionById(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await _bookConditionRatingRepository.GetDescriptionByIdAsync(id, ct);
                if (result == null)
                    return Result<BookConditionRatingDescriptionDto>.Failure("BookRatingDesc 不存在", ErrorCodes.General.NotFound);
                var dto = new BookConditionRatingDescriptionDto { Description = result };
                return Result<BookConditionRatingDescriptionDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<BookConditionRatingDescriptionDto>.Map(ex, _logger);
            }
        }
    }
}
