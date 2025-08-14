using Microsoft.AspNetCore.Mvc;
using static prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors.ErrorCodes;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors
{
    /// <summary>
    /// 依 <see cref="ErrorCodes"/> 將錯誤碼映射為 <see cref="IActionResult"/>。
    /// Controller 只需傳入 ErrorCode，即可得到標準化 HTTP 回應。
    /// </summary>
    public static class ErrorCodeToHttpResponseMapper
    {
        /// <summary>
        /// 根據錯誤代碼產生對應的 <see cref="ActionResult"/>。
        /// </summary>
        /// <param name="errorCode">自訂錯誤代碼；若為 <c>null</c> 或空白則回傳 500。</param>
        public static ActionResult Map(string? errorCode)
        {
            var response = new ErrorResponse
            {
                ErrorCode = errorCode ?? General.Unexpected,
                ErrorMessage = GetMessage(errorCode)
            };

            return errorCode switch
            {
                // ---------- Auth ----------
                Auth.NotMatch or Auth.AlreadyLoggedIn
                    => new BadRequestObjectResult(response),

                Auth.Unauthorized or Auth.InvalidUserContext or Auth.SessionExpired or Auth.AccountLocked
                    => new UnauthorizedObjectResult(response),

                Auth.EmailAlreadyExists
                    => new ConflictObjectResult(response),

                // ---------- Validation ----------
                Validation.InvalidFormat
                    => new BadRequestObjectResult(response),

                // ---------- General ----------
                General.BadRequest
                    => new BadRequestObjectResult(response),

                General.NotFound
                    => new NotFoundObjectResult(response),

                General.Forbidden
                    => new ObjectResult(response) { StatusCode = StatusCodes.Status403Forbidden },

                General.Conflict
                    => new ConflictObjectResult(response),

                // ---------- EF ----------
                Ef.ConcurrencyConflict
                    => new ConflictObjectResult(response),

                Ef.InvalidEntityState
                    => new ObjectResult(response) { StatusCode = StatusCodes.Status422UnprocessableEntity },

                // ---------- SQL ----------
                Sql.DuplicateKey or Sql.UniqueConstraint or Sql.ForeignKeyConstraint
                    => new ConflictObjectResult(response),

                Sql.Timeout
                    => new ObjectResult(response) { StatusCode = StatusCodes.Status504GatewayTimeout },

                Sql.Deadlock
                    => new ObjectResult(response) { StatusCode = StatusCodes.Status503ServiceUnavailable },

                Sql.SqlError
                    => new BadRequestObjectResult(response),

                // ---------- Fallback ----------
                _ => new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError }
            };
        }

        /// <summary>
        /// 依 ErrorCode 取得預設訊息文字（後續可改成多語系資源）。
        /// </summary>
        private static string GetMessage(string? errorCode) => errorCode switch
        {
            // Auth
            Auth.NotMatch => "密碼錯誤",
            Auth.Unauthorized => "未授權行為",
            Auth.InvalidUserContext => "登入但Claim錯誤",
            Auth.SessionExpired => "工作階段已過期",
            Auth.AccountLocked => "帳戶已被鎖定",
            Auth.AlreadyLoggedIn => "重複登入",
            Auth.EmailAlreadyExists => "Email已被使用",

            // Validation
            Validation.InvalidFormat => "欄位格式不正確",

            // General
            General.NotFound => "找不到資源",
            General.Conflict => "資料衝突",
            General.Forbidden => "禁止存取",
            General.BadRequest => "請求格式錯誤",

            // EF
            Ef.ConcurrencyConflict => "資料已被其他使用者修改",
            Ef.InvalidEntityState => "模型狀態錯誤",

            // SQL
            Sql.DuplicateKey => "資料已存在",
            Sql.UniqueConstraint => "資料違反唯一性限制",
            Sql.ForeignKeyConstraint => "資料關聯錯誤",
            Sql.Timeout => "資料庫逾時，請稍後再試",
            Sql.Deadlock => "資料庫資源競爭導致死結",
            Sql.SqlError => "資料庫執行錯誤",

            // Fallback
            _ => "系統錯誤，請稍後再試"
        };
    }
}
