namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors
{
    /// <summary>
    /// 本類別統一定義系統中的錯誤代碼常數，
    /// 用於 Result<T>.Failure(...) 的錯誤分類與前端/Log 判斷依據。
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>登入/驗證/授權相關錯誤</summary>
        public static class Auth
        {
            public const string NotMatch = "Auth.NotMatch";
            public const string AccountLocked = "Auth.AccountLocked";
            public const string Unauthorized = "Auth.Unauthorized";
            public const string InvalidUserContext = "Auth.InvalidUserContext";
            public const string SessionExpired = "Auth.SessionExpired";
            public const string AlreadyLoggedIn = "Auth.AlreadyLoggedIn";
            public const string EmailAlreadyExists = "Auth.EmailAlreadyExists";
        }

        /// <summary>資料驗證錯誤</summary>
        public static class Validation
        {
            public const string InvalidFormat = "Validation.InvalidFormat";
            public const string InvalidValue = "Validation.InvalidValuie";
        }

        /// <summary>一般錯誤（系統邏輯、找不到資源等）</summary>
        public static class General
        {
            public const string Unexpected = "General.Unexpected";                  // 未預期的例外
            public const string NotFound = "General.NotFound";                      // 查無資料
            public const string Conflict = "General.Conflict";                      // 狀態衝突（重複、操作不一致等）
            public const string Forbidden = "General.Forbidden";
            public const string BadRequest = "General.BadRequest";
        }

        /// <summary>SQL Server 錯誤（資料庫層級的限制與失敗）</summary>
        public static class Sql
        {
            public const string DuplicateKey = "Sql.DuplicateKey";                  // 2627
            public const string UniqueConstraint = "Sql.UniqueConstraint";          // 2601
            public const string ForeignKeyConstraint = "Sql.ForeignKeyConstraint";  // 547
            public const string Timeout = "Sql.Timeout";                            // -2
            public const string Deadlock = "Sql.Deadlock";                          // 1205
            public const string SqlError = "Sql.SqlError";                          // fallback
        }

        /// <summary>EF Core 應用層錯誤（追蹤狀態、模型錯誤等）</summary>
        public static class Ef
        {
            public const string ConcurrencyConflict = "Ef.ConcurrencyConflict";     // SaveChanges 遇到併發
            public const string InvalidEntityState = "Ef.InvalidEntityState";       // Entity 沒有 Key 或無法追蹤
            //public const string ChangeTrackingError = "Ef.ChangeTrackingError";     // Entity 狀態錯誤
            //public const string NavigationError = "Ef.NavigationError";             // Include 或 Navigation 使用錯誤
            //public const string QueryError = "Ef.QueryError";                       // 不合法的查詢條件
        }
    }

}
