using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace prjSpecialTopicWebAPI.Features.Ebook.Services
{
    public class ECPayService
    {
        public string GenerateCheckMacValue(Dictionary<string, string> parameters, string hashKey, string hashIv)
        {
            var sortedParams = parameters.OrderBy(p => p.Key, StringComparer.Ordinal);

            var sb = new StringBuilder();
            sb.Append($"HashKey={hashKey}");
            foreach (var param in sortedParams)
            {
                sb.Append($"&{param.Key}={param.Value}");
            }
            sb.Append($"&HashIV={hashIv}");

            string rawString = sb.ToString();

            // 使用 HttpUtility.UrlEncode 進行標準編碼
            string encodedString = HttpUtility.UrlEncode(rawString);

            // 根據綠界文件，手動處理特殊字符的轉換，並轉為小寫
            encodedString = encodedString.Replace("%2d", "-")
                                         .Replace("%5f", "_")
                                         .Replace("%2e", ".")
                                         .Replace("%21", "!")
                                         .Replace("%2a", "*")
                                         .Replace("%28", "(")
                                         .Replace("%29", ")")
                                         .ToLower(); // <-- ToLower() 放在最後

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(encodedString));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        /// <summary>
        /// 100% 模擬綠界官方 PHP 範例的 urlencode 編碼行為。
        /// 只有 0-9, a-z, A-Z, -, _, . 會被保留，其餘所有字元都會被編碼。
        /// </summary>
        private string EcpayUrlEncode(string str)
        {
            var result = new StringBuilder();
            var acceptableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";

            foreach (char c in str)
            {
                if (acceptableChars.Contains(c))
                {
                    result.Append(c);
                }
                else
                {
                    // 對字元進行 UTF-8 編碼，然後將每個 byte 轉為 %XX 的格式
                    byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());
                    foreach (byte b in bytes)
                    {
                        result.Append('%' + b.ToString("X2"));
                    }
                }
            }
            return result.ToString();
        }
    }
}

