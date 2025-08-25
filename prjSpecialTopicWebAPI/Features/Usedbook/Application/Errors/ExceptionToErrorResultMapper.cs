using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using System.Runtime.CompilerServices;

/// <summary>
/// 提供 Service Layer 的例外處理映射邏輯，
/// 將例外轉換為 <see cref="Result{T}"/> 物件，封裝錯誤代碼與訊息。
/// </summary>
/// <typeparam name="T">要包裝的結果資料型別</typeparam>
public static class ExceptionToErrorResultMapper<T>
{
    /// <summary>
    /// 根據例外類型轉換為 <see cref="Result{T}"/>，並記錄錯誤日誌。
    /// </summary>
    /// <param name="ex">捕獲的例外物件</param>
    /// <param name="logger">日誌紀錄器</param>
    /// <param name="caller">呼叫此方法的成員名稱（由編譯器自動提供）</param>
    /// <returns>封裝錯誤訊息與錯誤代碼的 <see cref="Result{T}"/> 物件</returns>
    public static Result<T> Map(Exception ex, ILogger logger, [CallerMemberName] string? caller = null)
    {
        var dataType = typeof(T).Name;
        var callerName = caller ?? "Unknown";

        logger.LogError(ex,
            "[{Caller} → {DataType}] 發生例外：{ExceptionType} - {Message}",
            callerName,
            dataType,
            ex.GetType().Name,
            ex.Message
        );

        return ex switch
        {
            // EF Core 在執行資料異動時，發生的 SQL Server 例外（主鍵/外鍵/逾時等）
            DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx =>
                MapSqlException(sqlEx),

            // SaveChanges 發生併發衝突（例如 RowVersion 檢查失敗）
            DbUpdateConcurrencyException =>
                Result<T>.Failure("資料已被其他使用者修改", ErrorCodes.Ef.ConcurrencyConflict),

            // Entity 被追蹤但無法辨識主鍵（模型設計錯誤）
            InvalidCastException =>
                Result<T>.Failure("資料格式不正確，可能為模型錯誤", ErrorCodes.Ef.InvalidEntityState),

            // LINQ 查詢返回多筆但預期單筆，例如 .Single() 找到兩筆資料
            InvalidOperationException =>
                Result<T>.Failure("資料庫操作錯誤，查詢結果不唯一", ErrorCodes.General.Conflict),

            // 查詢結果為 null 卻仍執行後續操作（通常為 null 檢查漏掉）
            ArgumentNullException =>
                Result<T>.Failure("找不到目標資料", ErrorCodes.General.NotFound),

            // fallback：未預期例外
            _ =>
                Result<T>.Failure("系統錯誤，請稍後再試", ErrorCodes.General.Unexpected)
        };
    }

    /// <summary>
    /// 將 <see cref="SqlException"/> 轉換為對應的 <see cref="Result{T}"/>。
    /// 根據 SQL Server 的錯誤編號決定錯誤訊息與錯誤代碼。
    /// </summary>
    /// <param name="sqlEx">SQL Server 拋出的例外</param>
    /// <returns>封裝錯誤訊息與錯誤代碼的 <see cref="Result{T}"/> 物件</returns>
    private static Result<T> MapSqlException(SqlException sqlEx)
    {
        return sqlEx.Number switch
        {
            2627 => Result<T>.Failure("資料重複，無法新增", ErrorCodes.Sql.DuplicateKey),                    // 主鍵違反
            2601 => Result<T>.Failure("資料違反唯一性限制", ErrorCodes.Sql.UniqueConstraint),                // UNIQUE 約束
            547 => Result<T>.Failure("資料有關聯，無法刪除或更新", ErrorCodes.Sql.ForeignKeyConstraint),      // 外鍵違反
            -2 => Result<T>.Failure("資料庫逾時，請稍後再試", ErrorCodes.Sql.Timeout),                        // 執行逾時
            1205 => Result<T>.Failure("資料庫資源競爭導致死結，請重試操作", ErrorCodes.Sql.Deadlock),          // 死結
            _ => Result<T>.Failure("資料庫執行錯誤", ErrorCodes.Sql.SqlError)                                // fallback
        };
    }
}
