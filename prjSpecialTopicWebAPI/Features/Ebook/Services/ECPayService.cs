using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace prjSpecialTopicWebAPI.Features.Ebook.Services
{
    public class ECPayService
    {
        public string GenerateCheckMacValue(Dictionary<string, string> parameters, string hashKey, string hashIv)
        {
            // 步驟 1: 將傳遞參數依照第一個英文字母，由A到Z的順序來排序
            var sortedParams = parameters.OrderBy(p => p.Key, StringComparer.Ordinal);

            // 步驟 2: 參數最前面加上HashKey、最後面加上HashIV
            var sb = new StringBuilder();
            sb.Append($"HashKey={hashKey}");
            foreach (var param in sortedParams)
            {
                sb.Append($"&{param.Key}={param.Value}");
            }
            sb.Append($"&HashIV={hashIv}");

            string rawString = sb.ToString();

            // 步驟 3: 將整串字串進行URL encode
            string encodedString = HttpUtility.UrlEncode(rawString);

            // 【關鍵修正】根據文件，對特定編碼後的字元進行還原 (decode)
            encodedString = encodedString.Replace("%2d", "-")
                                         .Replace("%5f", "_")
                                         .Replace("%2e", ".")
                                         .Replace("%21", "!")
                                         .Replace("%2a", "*")
                                         .Replace("%28", "(")
                                         .Replace("%29", ")");

            // 步驟 4: 轉為小寫
            encodedString = encodedString.ToLower();

            // 步驟 5: 以SHA256加密方式來產生雜凑值
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(encodedString));

                // 步驟 6: 再轉大寫產生CheckMacValue
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

