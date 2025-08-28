using AutoMapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using prjSpecialTopicWebAPI.Models;
using System.Drawing;
using System.Linq.Expressions;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class UsedBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UsedBookRepository _usedBookRepository;
        private readonly UsedBookImageService _usedBookImageService;
        private readonly LookupService _lookupService;
        private readonly ImageService _imageService;
        private readonly ILogger<UsedBookService> _logger;

        public UsedBookService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UsedBookRepository usedBookRepository,
            UsedBookImageService usedBookImageService,
            UsedBookImageRepository usedBookImageRepository,
            LookupService lookupService,
            ImageService imageService,
            BookSaleTagRepository saleTagRepository,
            ILogger<UsedBookService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usedBookRepository = usedBookRepository;
            _usedBookImageService = usedBookImageService;
            _lookupService = lookupService;
            _imageService = imageService;
            _logger = logger;
        }

        // ========== 新增、更新 ==========

        /// <summary>
        /// 新增完整書本資源，圖片部分交給 ImageService
        /// </summary>
        public async Task<Result<Guid>> CreateAsync(Guid sellerId, CreateBookRequest request, HttpRequest httpRequest, CancellationToken ct = default)
        {
            Guid usedBookId = Guid.NewGuid();
            DateTime nowTime = DateTime.UtcNow;

            var entity = _mapper.Map<UsedBook>(request);
            entity.Id = usedBookId;
            entity.SellerId = sellerId;
            entity.IsSold = false;
            entity.IsActive = true;
            entity.Slug = usedBookId.ToString();
            entity.CreatedAt = nowTime;
            entity.UpdatedAt = nowTime;

            using var tx = _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                _usedBookRepository.Add(entity);
                var saveImageResult = await _imageService.SaveImagesAsync(request.ImageList, httpRequest, ct);
                if (!saveImageResult.IsSuccess)
                    throw new Exception(saveImageResult.ErrorMessage);

                var createRequestList = saveImageResult.Value
                    .Select((image, index) => new CreateUsedBookImageRequest
                    {
                        IsCover = index == 0, // 假設第一張圖片為封面
                        StorageProvider = StorageProvider.Local,
                        ObjectKey = image.Id,
                    }).ToList();

                // 此處呼叫 ImageService 來處理封面圖片
                var createImageResult = await _usedBookImageService.CreateAsync(usedBookId, createRequestList, ct);
                if (!createImageResult.IsSuccess)
                    throw new Exception(createImageResult.ErrorMessage);

                await _unitOfWork.CommitAsync(ct);

                return Result<Guid>.Success(usedBookId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Guid>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 更新指定書本資源，圖片部分交給 ImageService
        /// </summary>
        public async Task<Result<Unit>> UpdateAsync(Guid id, UpdateBookRequest request, HttpRequest httpRequest, CancellationToken ct = default)
        {
            using var tx = _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var entity = await _usedBookRepository.GetEntityByIdAsync(id, ct);
                if (entity is null)
                    return Result<Unit>.Failure("找不到要更新的書本", ErrorCodes.General.NotFound);

                entity.SellerDistrictId = request.SellerDistrictId;
                entity.SalePrice = request.SalePrice;
                entity.Title = request.Title;
                entity.Authors = request.Authors;
                entity.CategoryId = request.CategoryId;
                entity.ConditionRatingId = request.ConditionRatingId;
                entity.ConditionDescription = request.ConditionDescription;
                entity.Edition = request.Edition;
                entity.Publisher = request.Publisher;
                entity.PublicationDate = request.PublicationDate;
                entity.Isbn = request.Isbn;
                entity.BindingId = request.BindingId;
                entity.LanguageId = request.LanguageId;
                entity.Pages = request.Pages;
                entity.ContentRatingId = request.ContentRatingId;
                entity.IsOnShelf = request.IsOnShelf;
                entity.UpdatedAt = DateTime.UtcNow;

                // 更新圖片
                var ids = new HashSet<int>();
                var updateRequest = new UpdateOrderByIdRequest();
                foreach (var image in request.ImageList)
                {
                    if (image.Id != null)
                    {
                        ids.Add((int)image.Id);
                        updateRequest.IdList.Add((int)image.Id);
                    }
                    else if (image.Image != null)
                    {
                        var saveResult = await _imageService.SaveImageAsync(image.Image, httpRequest, ct);
                        if (!saveResult.IsSuccess)
                            throw new Exception(saveResult.ErrorMessage);

                        var createRequest = new CreateUsedBookImageRequest
                        {
                            IsCover = false,
                            StorageProvider = StorageProvider.Local,
                            ObjectKey = saveResult.Value.Id,
                        };
                        var createResult = await _usedBookImageService.CreateAsync(id, createRequest, ct);
                        if (!createResult.IsSuccess)
                            throw new Exception(createResult.ErrorMessage);
                        ids.Add(createResult.Value);
                        updateRequest.IdList.Add(createResult.Value);
                    }
                }

                await _usedBookImageService.SetCoverAsync(id, new SetBookCoverRequest { ImageId = updateRequest.IdList[0] }, ct);

                var currentList = await _usedBookImageService.GetByBookIdAsync(id, ct);
                if (!currentList.IsSuccess)
                    throw new Exception(currentList.ErrorMessage);
                foreach (var item in currentList.Value)
                {
                    if (!ids.Contains(item.Id))
                        await _usedBookImageService.DeleteByImageIdAsync(item.Id, ct);
                }

                var updateOrderResult = await _usedBookImageService.UpdateOrderByBookIdAsync(id, updateRequest, ct);
                if (!updateOrderResult.IsSuccess)
                    throw new Exception(updateOrderResult.ErrorMessage);

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 更改狀態 ==========

        public async Task<Result<Unit>> UpdateOnShelfStatusAsync(Guid id, UpdateBooleanStatusRequest request, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateOnShelfStatusAsync(id, request.Value, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateActiveStatusAsync(Guid id, UpdateBooleanStatusRequest request, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateActiveStatusAsync(id, request.Value, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateSoldStatusAsync(Guid id, UpdateBooleanStatusRequest request, CancellationToken ct = default)
        {
            try
            {
                bool commandResult = await _usedBookRepository.UpdateSoldStatusAsync(id, request.Value, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== 查詢 ==========

        public async Task<Result<PublicUsedBookDetailDto>> GetPublicDetailByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var bookQueryResult = await _usedBookRepository.GetDetailByIdAsync(id, ct);
                if (bookQueryResult == null)
                    return Result<PublicUsedBookDetailDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageResult = await _usedBookImageService.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<PublicUsedBookDetailDto>(bookQueryResult);
                dto.ImageList = imageResult?.Value?.ToList() ?? [];

                return Result<PublicUsedBookDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PublicUsedBookDetailDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<AdminUsedBookDetailDto>> GetAdminDetailByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var bookQueryResult = await _usedBookRepository.GetDetailByIdAsync(id, ct);
                if (bookQueryResult == null)
                    return Result<AdminUsedBookDetailDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageResult = await _usedBookImageService.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<AdminUsedBookDetailDto>(bookQueryResult);
                dto.ImageList = imageResult?.Value?.ToList() ?? [];

                return Result<AdminUsedBookDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<AdminUsedBookDetailDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<UpdateBookPayloadDto>> GetUpdatePayloadByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var entity = await _usedBookRepository.GetEntityByIdWithCountyIdAsync(id, ct);
                if (entity == null)
                    return Result<UpdateBookPayloadDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageResult = await _usedBookImageService.GetByBookIdAsync(id, ct);

                var dto = _mapper.Map<UpdateBookPayloadDto>(entity);
                dto.ImageList = imageResult?.Value?.ToList() ?? [];
                dto.SellerCountyId = entity.SellerDistrict.CountyId;

                return Result<UpdateBookPayloadDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<UpdateBookPayloadDto>.Map(ex, _logger);
            }
        }

        public async Task<Result<PagedResult<PublicBookListItemDto>>> GetPublicListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetPublicBookListAsync(predicate, orderBy, query.Paging, ct);
                var itemList = new List<PublicBookListItemDto>();
                foreach (var res in queryResult.Items)
                {
                    var item = _mapper.Map<PublicBookListItemDto>(res);
                    item.CoverImageUrl = _usedBookImageService.GetThumbUrlWithFallback(res.CoverStorageProvider, res.CoverObjectKey);
                    itemList.Add(item);
                }

                var dto = new PagedResult<PublicBookListItemDto>
                {
                    Items = itemList,
                    PageIndex = queryResult.PageIndex,
                    PageSize = queryResult.PageSize,
                    TotalRows = queryResult.TotalRows
                };

                return Result<PagedResult<PublicBookListItemDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PagedResult<PublicBookListItemDto>>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<UserBookListItemDto>>> GetUserBookListAsync(Guid userId, BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetUserBookListAsync(userId, predicate, orderBy, ct);
                var dtoList = new List<UserBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<UserBookListItemDto>(res);
                    dto.CoverImageUrl = _usedBookImageService.GetThumbUrlWithFallback(res.CoverStorageProvider, res.CoverObjectKey);
                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<UserBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<UserBookListItemDto>>.Map(ex, _logger);
            }
        }

        public async Task<Result<PagedResult<AdminBookListItemDto>>> GetAdminBookListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件 + 排序 的轉換與組裝
                Expression<Func<UsedBook, bool>> predicate = BuildPredicate(query);
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = BuildOrderBy(query);

                var queryResult = await _usedBookRepository.GetAdminBookListAsync(predicate, orderBy, query.Paging, ct);
                var itemList = new List<AdminBookListItemDto>();
                foreach (var res in queryResult.Items)
                {
                    var item = _mapper.Map<AdminBookListItemDto>(res);
                    item.CoverImageUrl = _usedBookImageService.GetThumbUrlWithFallback(res.CoverStorageProvider, res.CoverObjectKey);
                    itemList.Add(item);
                }

                var dto = new PagedResult<AdminBookListItemDto>
                {
                    Items = itemList,
                    PageIndex = queryResult.PageIndex,
                    PageSize = queryResult.PageSize,
                    TotalRows = queryResult.TotalRows
                };

                return Result<PagedResult<AdminBookListItemDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PagedResult<AdminBookListItemDto>>.Map(ex, _logger);
            }
        }

        // ========== 促銷標籤相關 ==========

        public async Task<Result<Unit>> ApplyBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _usedBookRepository.AddSaleTagAsync(bookId, tagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> RemoveBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _usedBookRepository.RemoveSaleTagAsync(bookId, tagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        public async Task<Result<Unit>> UpdateBookSaleTagBatchAsync(UpdateBookSaleTagRequest request, CancellationToken ct = default)
        {
            try
            {
                var commandResult = request.IsApply ?
                    await _usedBookRepository.AddSaleTagBatchAsync(request.BookIdList, request.TagId, ct) :
                    await _usedBookRepository.RemoveSaleTagBatchAsync(request.BookIdList, request.TagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // ========== Excel ==========

        public async Task<Result<byte[]>> ExportUploadExampleAsync(CancellationToken ct = default)
        {
            try
            {
                var lookupResult = await _lookupService.GetAllUsedBookUILookupsList(ct);
                if (!lookupResult.IsSuccess)
                    throw new Exception(lookupResult.ErrorMessage);
                var lookups = lookupResult.Value;

                var countyDictResult = await _lookupService.GetCountyDistrictDictionaryAsync(ct);
                if (!countyDictResult.IsSuccess)
                    throw new Exception(countyDictResult.ErrorMessage);
                var countyDict = countyDictResult.Value;

                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("上傳資料表");
                var rws = package.Workbook.Worksheets.Add("選項表");

                // ========== 建立選項表 ========== 

                // 工具函數
                void FillRef(ExcelWorksheet s, int col, string title, IEnumerable<string> list)
                {
                    s.Cells[1, col].Value = title;
                    int r = 2;
                    foreach (var v in list)
                        s.Cells[r++, col].Value = v;

                    // 定義具名範圍，供資料驗證使用
                    int endRow = r - 1;
                    if (endRow >= 2)
                    {
                        var addr = s.Cells[2, col, endRow, col];
                        // 若同名已存在，先移除再新增以避免重複命名錯誤
                        var wbNames = package.Workbook.Names;
                        if (wbNames.ContainsKey(title))
                            wbNames.Remove(title);

                        wbNames.Add(title, addr);
                    }
                }

                // 填入選項表
                int nowCol = 1;
                FillRef(rws, nowCol++, "裝訂方式", lookups.BookBindings.Select(i => i.Name));
                FillRef(rws, nowCol++, "主題分類", lookups.BookCategories.Select(i => i.Name));
                FillRef(rws, nowCol++, "書況評等", lookups.BookConditionRatings.Select(i => i.Name));
                FillRef(rws, nowCol++, "內容分級", lookups.ContentRatings.Select(i => i.Name));
                FillRef(rws, nowCol++, "語言", lookups.Languages.Select(i => i.Name));
                FillRef(rws, nowCol++, "縣市", lookups.Counties.Select(i => i.Name));
                foreach (var county in lookups.Counties)
                {
                    FillRef(rws, nowCol++, "CITY_" + county.Name, countyDict[county.Id]);
                }

                rws.Hidden = eWorkSheetHidden.VeryHidden;


                // ========== 建立資料表 ========== 
                // 第一列: 說明
                // 第二列: headers
                // 第三列: examples
                // 第四列開始填寫
                int maxRow = 100 + 4 - 1;

                string description = "紅色*欄位為必填，第3列為範例，資料請在第4列~103列填寫。上傳後須補上圖片才能於站內正常顯示。";

                string[] headers = {
                    "所在縣市*", "所在鄉鎮市區*", "售價*", "書名*", "作者*",
                    "主題分類*", "書況評等*", "書況描述",
                    "版次／刷次", "出版社", "出版日期", "ISBN",
                    "裝訂方式*", "語言*", "頁數", "內容分級*",
                    "是否上架*"
                };
                string[] examples = {
                    "台北市", "大安區", "299", "C++從入門到放棄（第3版）", "明日科技",
                    "電腦與資訊科學", "可接受", "封面與封底稍有破損與磨損，內頁有些許筆記但不影響閱讀。",
                    "3版2刷", "", "2024/06/01", "7302652090",
                    "平裝", "簡體中文", "393", "普遍級",
                    "Y"
                };
                int[] colWidth = {
                    10, 10, 10, 35, 35,
                    15, 10, 60,
                    10, 10, 10, 15,
                    10, 10, 10, 10,
                    10
                };

                // 填入說明 (r1)
                ws.Cells[1, 1].Value = description;

                // 填入表頭 (r2)
                for (int c = 0; c < headers.Length; ++c)
                {
                    var cell = ws.Cells[2, c + 1];
                    cell.Value = headers[c];
                    cell.Style.Font.Bold = true;
                    if (headers[c].EndsWith("*"))
                        cell.Style.Font.Color.SetColor(Color.Red);
                }

                // 填入範例 (r3)
                for (int c = 0; c < headers.Length; ++c)
                {
                    ws.Cells[3, c + 1].Value = examples[c];
                }

                nowCol = 1;

                // 所在縣市 (c1)
                var dvCounty = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvCounty.Formula.ExcelFormula = "=縣市";
                dvCounty.AllowBlank = false;
                dvCounty.ShowErrorMessage = true;
                dvCounty.ErrorTitle = "所在縣市錯誤";
                dvCounty.Error = "請從下拉選單選擇有效的『所在縣市』";
                ++nowCol;

                // 所在鄉鎮市區 (c2)
                var cityColLetter = OfficeOpenXml.ExcelCellAddress.GetColumnLetter(1);      // 所在縣市在第1欄
                var dvDistrict = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvDistrict.Formula.ExcelFormula = $"=INDIRECT(\"CITY_\" & ${cityColLetter}4)";
                dvDistrict.AllowBlank = false;
                dvDistrict.ShowErrorMessage = true;
                dvDistrict.ErrorTitle = "所在鄉鎮市區錯誤";
                dvDistrict.Error = "請從下拉選單選擇有效的『所在鄉鎮市區』";
                ++nowCol;

                // (c3)
                var dvSalePrice = ws.DataValidations.AddIntegerValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvSalePrice.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.greaterThan;
                dvSalePrice.Formula.Value = 0;
                dvSalePrice.AllowBlank = false;
                dvSalePrice.ShowErrorMessage = true;
                dvSalePrice.ErrorTitle = "數值錯誤";
                dvSalePrice.Error = "請輸入大於 0 的整數";
                ++nowCol;

                // (c4)
                var dvTitle = ws.DataValidations.AddTextLengthValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvTitle.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.lessThanOrEqual;
                dvTitle.Formula.Value = 50;
                dvTitle.ShowErrorMessage = true;
                dvTitle.ErrorTitle = "書名錯誤";
                dvTitle.Error = "最多 50 字";
                ++nowCol;

                // (c5)
                var dvAuthor = ws.DataValidations.AddTextLengthValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvAuthor.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.lessThanOrEqual;
                dvAuthor.Formula.Value = 100;
                dvAuthor.ShowErrorMessage = true;
                dvAuthor.ErrorTitle = "作者錯誤";
                dvAuthor.Error = "最多 100 字";
                ++nowCol;

                // 主題分類 (c6)
                var dvCategory = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvCategory.Formula.ExcelFormula = "=主題分類";
                dvCategory.AllowBlank = false;
                dvCategory.ShowErrorMessage = true;
                dvCategory.ErrorTitle = "主題分類錯誤";
                dvCategory.Error = "請從下拉選單選擇有效的『主題分類』";
                ++nowCol;

                // (c7)
                var dvCondition = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvCondition.Formula.ExcelFormula = "=書況評等";
                dvCondition.AllowBlank = false;
                dvCondition.ShowErrorMessage = true;
                dvCondition.ErrorTitle = "書況評等錯誤";
                dvCondition.Error = "請從下拉選單選擇有效的『書況評等』";
                ++nowCol;

                // (c8)
                var dvCondDesc = ws.DataValidations.AddTextLengthValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvCondDesc.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.lessThanOrEqual;
                dvCondDesc.Formula.Value = 100;
                dvCondDesc.ShowErrorMessage = true;
                dvCondDesc.ErrorTitle = "書況描述錯誤";
                dvCondDesc.Error = "最多 100 字";
                ++nowCol;

                var dvEdition = ws.DataValidations.AddTextLengthValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvEdition.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.lessThanOrEqual;
                dvEdition.Formula.Value = 10;
                dvEdition.ShowErrorMessage = true;
                dvEdition.ErrorTitle = "版次／刷次錯誤";
                dvEdition.Error = "最多 10 字";
                ++nowCol;

                var dvPublisher = ws.DataValidations.AddTextLengthValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvPublisher.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.lessThanOrEqual;
                dvPublisher.Formula.Value = 50;
                dvPublisher.ShowErrorMessage = true;
                dvPublisher.ErrorTitle = "出版社錯誤";
                dvPublisher.Error = "最多 50 字";
                ++nowCol;

                ws.Column(nowCol).Style.Numberformat.Format = "yyyy/mm/dd";
                var dvPubDate = ws.DataValidations.AddDateTimeValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvPubDate.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.lessThanOrEqual;
                dvPubDate.Formula.Value = DateTime.Today;
                dvPubDate.ShowErrorMessage = true;
                dvPubDate.ErrorTitle = "出版日錯誤";
                dvPubDate.Error = "須為合法日期";
                ++nowCol;

                var colLetter = OfficeOpenXml.ExcelCellAddress.GetColumnLetter(nowCol);
                var dvISBN = ws.DataValidations.AddCustomValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvISBN.Formula.ExcelFormula = $"=AND(ISNUMBER(VALUE(${colLetter}4)),OR(LEN(${colLetter}4)=10,LEN(${colLetter}4)=13))";
                dvISBN.ShowErrorMessage = true;
                dvISBN.ErrorTitle = "ISBN錯誤";
                dvISBN.Error = "ISBN 必須為 10 碼或 13 碼";
                ++nowCol;

                // 裝訂方式 (c13)
                var dvBinding = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvBinding.Formula.ExcelFormula = "=裝訂方式";
                dvBinding.AllowBlank = false;
                dvBinding.ShowErrorMessage = true;
                dvBinding.ErrorTitle = "裝訂方式錯誤";
                dvBinding.Error = "請從下拉選單選擇有效的『裝訂方式』";
                ++nowCol;

                var dvLanguage = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvLanguage.Formula.ExcelFormula = "=語言";
                dvLanguage.AllowBlank = false;
                dvLanguage.ShowErrorMessage = true;
                dvLanguage.ErrorTitle = "語言錯誤";
                dvLanguage.Error = "請從下拉選單選擇有效的『語言』";
                ++nowCol;

                var dvPages = ws.DataValidations.AddIntegerValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvPages.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.greaterThan;
                dvPages.Formula.Value = 0;
                dvPages.AllowBlank = false;
                dvPages.ShowErrorMessage = true;
                dvPages.ErrorTitle = "數值錯誤";
                dvPages.Error = "請輸入大於 0 的整數";
                ++nowCol;

                var dvContentRating = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvContentRating.Formula.ExcelFormula = "=內容分級";
                dvContentRating.AllowBlank = false;
                dvContentRating.ShowErrorMessage = true;
                dvContentRating.ErrorTitle = "內容分級錯誤";
                dvContentRating.Error = "請從下拉選單選擇有效的『內容分級』";
                ++nowCol;

                // 主題分類 (c17)
                var dvOnShelf = ws.DataValidations.AddListValidation(ws.Cells[4, nowCol, maxRow, nowCol].Address);
                dvOnShelf.Formula.Values.Add("Y");
                dvOnShelf.Formula.Values.Add("N");
                dvOnShelf.AllowBlank = false;
                dvOnShelf.ShowErrorMessage = true;
                dvOnShelf.ErrorTitle = "輸入錯誤";
                dvOnShelf.Error = "請從 Y / N 中選擇";


                // UX：凍結首列、調整欄寬
                ws.View.FreezePanes(4, 1);
                for (int c = 0; c < colWidth.Length; ++c)
                {
                    ws.Column(c + 1).Width = colWidth[c];
                }

                // UI
                ws.Cells[1, 1, 1, headers.Length].Merge = true;
                var desc = ws.Cells[1, 1];
                desc.Style.Fill.PatternType = ExcelFillStyle.Solid;
                desc.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));
                desc.Style.WrapText = true;

                using (var target = ws.Cells[3, 1, 3, headers.Length])
                {
                    target.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    target.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                return Result<byte[]>.Success(package.GetAsByteArray());
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<byte[]>.Map(ex, _logger);
            }
        }

        public async Task<Result<IReadOnlyList<Guid>>> ImportBooks(Guid sellerId, Stream stream, CancellationToken ct = default)
        {
            try
            {
                using var package = new ExcelPackage(stream);
                var ws = package.Workbook.Worksheets["上傳資料表"];
                if (ws == null || ws.Dimension == null)
                    return Result<IReadOnlyList<Guid>>.Failure("資料表不能為空", ErrorCodes.General.BadRequest);

                // ========== 整理 Dictionary ==========

                var lookupResult = await _lookupService.GetAllUsedBookUILookupsList(ct);
                if (!lookupResult.IsSuccess)
                    throw new Exception(lookupResult.ErrorMessage);
                var lookups = lookupResult.Value;

                var countyDistrictNameToIdResult = await _lookupService.GetCountyDistrictNameToDistrictIdAsync(ct);
                if (!countyDistrictNameToIdResult.IsSuccess)
                    throw new Exception(countyDistrictNameToIdResult.ErrorMessage);

                var countyDistrictDict = countyDistrictNameToIdResult.Value;
                var categoryDict = lookupResult.Value.BookCategories.ToDictionary(i => i.Name, i => i.Id);
                var conditionDict = lookupResult.Value.BookConditionRatings.ToDictionary(i => i.Name, i => i.Id);
                var bindingDict = lookupResult.Value.BookBindings.ToDictionary(i => i.Name, i => i.Id);
                var languageDict = lookupResult.Value.Languages.ToDictionary(i => i.Name, i => i.Id);
                var contentRatingDict = lookupResult.Value.ContentRatings.ToDictionary(i => i.Name, i => i.Id);

                // ========== 讀取 ==========

                int startRow = 4;
                int endRowCap = 100 + startRow - 1;

                int endRowByDim = ws.Dimension.End.Row;
                int endRow = Math.Min(endRowByDim, endRowCap);

                var entityList = new List<UsedBook>(capacity: endRow - startRow + 1);

                for (int row = startRow; row <= endRow; row++)
                {
                    string countytName = ws.Cells[row, 1].GetValue<string>()?.Trim() ?? string.Empty;           // 所在縣市
                    string districtName = ws.Cells[row, 2].GetValue<string>()?.Trim() ?? string.Empty;          // 所在鄉鎮市區
                    int salePrice = ws.Cells[row, 3].GetValue<int>();                                           // 售價
                    string title = ws.Cells[row, 4].GetValue<string>()?.Trim() ?? string.Empty;                 // 書名
                    string authors = ws.Cells[row, 5].GetValue<string>()?.Trim() ?? string.Empty;               // 作者

                    string categoryName = ws.Cells[row, 6].GetValue<string>()?.Trim() ?? string.Empty;          // 主題分類
                    string conditionName = ws.Cells[row, 7].GetValue<string>()?.Trim() ?? string.Empty;         // 書況評等
                    string? conditionDesc = ws.Cells[row, 8].GetValue<string?>()?.Trim() ?? string.Empty;

                    string? edition = ws.Cells[row, 9].GetValue<string?>()?.Trim();
                    string? publisher = ws.Cells[row, 10].GetValue<string?>()?.Trim();
                    DateTime? publishDateRaw = ws.Cells[row, 11].GetValue<DateTime?>();
                    string? isbn = ws.Cells[row, 12].GetValue<string?>()?.Trim();

                    string bindingName = ws.Cells[row, 13].GetValue<string>()?.Trim() ?? string.Empty;          // 裝訂方式
                    string languageName = ws.Cells[row, 14].GetValue<string>()?.Trim() ?? string.Empty;         // 語言
                    int? pages = ws.Cells[row, 15].GetValue<int?>();                                            // 頁數
                    string contentRatingName = ws.Cells[row, 16].GetValue<string>()?.Trim() ?? string.Empty;    // 內容分級

                    bool isOnShelf = ParseBool(ws.Cells[row, 17]);                                              // 是否上架

                    // 驗證並轉成合法輸入值
                    Guid id = Guid.NewGuid();
                    if (!countyDistrictDict.TryGetValue((countytName, districtName), out int sellerDistrictId))
                        throw new Exception($"row={row} 處 countytName, districtName 無效");
                    if (salePrice <= 0)
                        throw new Exception($"row={row} 處 salePrice 無效");
                    if (title == string.Empty)
                        throw new Exception($"row={row} 處 title 無效");
                    if (authors == string.Empty)
                        throw new Exception($"row={row} 處 authors 無效");
                    if (!categoryDict.TryGetValue(categoryName, out int categoryId))
                        throw new Exception($"row={row} 處 categoryName 無效");
                    if (!conditionDict.TryGetValue(conditionName, out int conditionRatingId))
                        throw new Exception($"row={row} 處 conditionName 無效");

                    DateOnly? publishDate = null;
                    if (publishDateRaw.HasValue)
                    {
                        if (publishDateRaw.Value.Date > DateTime.Today)
                            throw new Exception($"row={row} 處 publishDate 無效");
                        else
                            publishDate = DateOnly.FromDateTime(publishDateRaw.Value.Date);
                    }
                    if (isbn != null && isbn.Length != 10 && isbn.Length != 13)
                        throw new Exception($"row={row} 處 isbn 無效");

                    if (!bindingDict.TryGetValue(bindingName, out int bindingId))
                        throw new Exception($"row={row} 處 bindingName 無效");
                    if (!languageDict.TryGetValue(languageName, out int languageId))
                        throw new Exception($"row={row} 處 languageName 無效");
                    if (pages != null && pages <= 0)
                        throw new Exception($"row={row} 處 pages 無效");
                    if (!contentRatingDict.TryGetValue(contentRatingName, out int contentRatingId))
                        throw new Exception($"row={row} 處 contentRatingName 無效");

                    // 組裝實體
                    entityList.Add(new UsedBook
                    {
                        Id = id,
                        SellerId = sellerId,
                        SellerDistrictId = sellerDistrictId,

                        SalePrice = salePrice,
                        Title = title,
                        Authors = authors,
                        CategoryId = categoryId,
                        ConditionRatingId = conditionRatingId,
                        ConditionDescription = conditionDesc,

                        Edition = edition,
                        Publisher = publisher,
                        PublicationDate = publishDate,
                        Isbn = isbn,
                        BindingId = bindingId,
                        LanguageId = languageId,
                        Pages = pages,
                        ContentRatingId = contentRatingId,

                        IsOnShelf = isOnShelf,
                        IsSold = false,
                        IsActive = true,

                        Slug = id.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    });
                }

                _usedBookRepository.AddRange(entityList);
                await _unitOfWork.CommitAsync(ct);

                var result = entityList.Select(e => e.Id).ToList();
                return Result<IReadOnlyList<Guid>>.Success(result);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<Guid>>.Map(ex, _logger);
            }
        }

        // ========== 私有方法 ==========

        private Expression<Func<UsedBook, bool>> BuildPredicate(BookListQuery query)
        {
            // 預處理
            var saleTagIds = query.SaleTagIds ?? Array.Empty<int>();
            var keyword = query.Keyword?.Trim();
            var status = query.BookStatus?.Trim().ToLowerInvariant();

            return b =>
                // 主題分類 N:1
                (!query.CategoryId.HasValue || b.CategoryId == query.CategoryId) &&
                // 促標標籤 N:M
                (saleTagIds.Count == 0 || b.Tags.Any(t => saleTagIds.Contains(t.Id))) &&
                // 狀態
                (string.IsNullOrWhiteSpace(status) ||
                    status == "all" ||
                    (status == "inactive" && !b.IsActive) ||
                    (status == "unsold" && !b.IsSold) ||
                    (status == "onshelf" && b.IsOnShelf)) &&
                // 關鍵字
                (string.IsNullOrWhiteSpace(query.Keyword)
                    || b.Title.Contains(query.Keyword)) &&
                // 價格區間
                (!query.MinPrice.HasValue || b.SalePrice >= query.MinPrice) &&
                (!query.MaxPrice.HasValue || b.SalePrice <= query.MaxPrice);
        }

        private Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> BuildOrderBy(BookListQuery query)
        {
            return q => query switch
            {
                _ when query.Paging.SortBy == "updated" && query.Paging.SortDir == "asc" => q.OrderBy(b => b.UpdatedAt),
                _ when query.Paging.SortBy == "updated" && query.Paging.SortDir == "desc" => q.OrderByDescending(b => b.UpdatedAt),
                _ when query.Paging.SortBy == "created" && query.Paging.SortDir == "asc" => q.OrderBy(b => b.CreatedAt),
                _ when query.Paging.SortBy == "created" && query.Paging.SortDir == "desc" => q.OrderByDescending(b => b.CreatedAt),
                _ when query.Paging.SortBy == "price" && query.Paging.SortDir == "asc" => q.OrderBy(b => b.SalePrice),
                _ when query.Paging.SortBy == "price" && query.Paging.SortDir == "desc" => q.OrderByDescending(b => b.SalePrice),
                _ => q.OrderByDescending(b => b.UpdatedAt)
            };
        }

        // 小工具：更彈性的布林轉換
        private static bool ParseBool(ExcelRange cell)
        {
            // 先試著用 GetValue<bool>()
            if (bool.TryParse(cell.Text, out var b)) return b;

            var t = (cell.GetValue<string>() ?? string.Empty).Trim().ToLowerInvariant();
            return t switch
            {
                "1" or "true" or "yes" or "y" or "是" => true,
                "0" or "false" or "no" or "n" or "否" => false,
                _ => cell.GetValue<bool>() // 有時 Value 就是 bool
            };
        }
    }
}
