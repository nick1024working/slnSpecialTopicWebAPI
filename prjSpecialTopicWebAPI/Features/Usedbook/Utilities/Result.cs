using System.Diagnostics.CodeAnalysis;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Utilities
{
    /// <summary>
    /// 表示一個操作的結果，包含成功或失敗的狀態、資料值與錯誤訊息。
    /// 常用於服務層（Service Layer）將邏輯結果回傳給控制器（Controller）。
    /// </summary>
    /// <typeparam name="T">回傳的資料型別</typeparam>
    public class Result<T>
    {
        private readonly T? _value;

        // 代表產生此 Result<T> 的行為是否成功
        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsSuccess { get; }

        // 代表行為成功時的資料值，若行為失敗則不允許使用
        public T? Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("不能從失敗結果取得 Value。");

        public string? ErrorMessage { get; }
        public string? ErrorCode { get; }

        /// <summary>
        /// 私有建構函式，僅供靜態方法使用。
        /// </summary>
        private Result(bool isSuccess, T? value, string? errorMessage, string? errorCode)
        {
            IsSuccess = isSuccess;
            _value = value;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 建立一個成功的結果。
        /// </summary>
        /// <param name="value">成功時的資料內容</param>
        /// <returns>表示成功的 Result 實例</returns>
        public static Result<T> Success(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "成功結果不得為 null");
            return new Result<T>(true, value, null, null);
        }

        /// <summary>
        /// 建立一個失敗的結果，包含錯誤訊息與錯誤代碼。
        /// </summary>
        /// <param name="errorMessage">錯誤訊息</param>
        /// <param name="errorCode">錯誤代碼，例如 VALIDATION_FAIL、DB_DUPLICATE_KEY</param>
        /// <returns>表示失敗的 Result 實例</returns>
        public static Result<T> Failure(string errorMessage, string errorCode)
            => new Result<T>(false, default, errorMessage, errorCode);
    }
}