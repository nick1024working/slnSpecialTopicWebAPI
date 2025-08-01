namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors
{

    /// <summary>
    /// 封裝錯誤的標準格式，
    /// 包含錯誤訊息與錯誤代碼，用於 API 回傳。
    /// </summary>
    public class ErrorResponse
    {
        public string ErrorMessage { get; set; } = "系統錯誤";
        public string ErrorCode { get; set; } = ErrorCodes.General.Unexpected;
    }
}
