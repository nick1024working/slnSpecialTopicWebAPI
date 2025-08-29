using System.Security.Cryptography;
using System.Text;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Utilities
{
    public sealed class StateToken
    {
        public string OrderNo { get; set; } = string.Empty;
        public long Ts { get; set; }

        /// <summary>
        /// 建立 token
        /// </summary>
        public static string Create(string orderNo, string secret, int expireSeconds = 900)
        {
            long ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string payload = $"{orderNo}|{ts}";
            string hmac = Sign(payload, secret);

            string raw = $"{orderNo}|{ts}|{hmac}";
            return Base64UrlEncode(Encoding.UTF8.GetBytes(raw));
        }

        /// <summary>
        /// 解析並驗證 token
        /// 驗簽/檢查時效失敗會回傳 null
        /// </summary>
        public static StateToken? Parse(string token, string secret, int expireSeconds = 900)
        {
            try
            {
                string raw = Encoding.UTF8.GetString(Base64UrlDecode(token));
                var parts = raw.Split('|');
                if (parts.Length != 3) return null;

                string orderNo = parts[0];
                if (!long.TryParse(parts[1], out long ts)) return null;
                string givenHmac = parts[2];

                string expected = Sign($"{orderNo}|{ts}", secret);

                if (!CryptographicOperations.FixedTimeEquals(
                        Convert.FromBase64String(expected),
                        Convert.FromBase64String(givenHmac)))
                    return null;

                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (now - ts > expireSeconds) return null;

                return new StateToken { OrderNo = orderNo, Ts = ts };
            }
            catch
            {
                return null;
            }
        }

        private static string Sign(string data, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

        private static byte[] Base64UrlDecode(string s)
        {
            s = s.Replace("-", "+").Replace("_", "/");
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }
    }
}
