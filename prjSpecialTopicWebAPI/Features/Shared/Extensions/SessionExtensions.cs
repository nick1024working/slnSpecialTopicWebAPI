using System.Text.Json;

namespace prjSpecialTopicWebAPI.Features.Shared.Extensions
{
    /// </summary>
    public static class SessionExtensions
    {
        // 自定義 JSON 序列化設定，此處設定忽略屬性名稱大小寫差異
        static readonly JsonSerializerOptions JsonOpt = new() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// 將指定物件以 JSON 序列化後存入 Session。
        /// </summary>
        public static void SetObject<T>(this ISession session, string key, T value) =>
            session.SetString(key, JsonSerializer.Serialize(value, JsonOpt));

        /// <summary>
        /// 從 Session 取出指定鍵的 JSON，並反序列化為強型別物件。
        /// </summary>
        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json is null ? default : JsonSerializer.Deserialize<T>(json, JsonOpt);
        }

        /// <summary>
        /// 從 Session 刪除指定鍵。
        /// </summary>
        public static void RemoveObject(this ISession session, string key) =>
            session.Remove(key);
    }
}
