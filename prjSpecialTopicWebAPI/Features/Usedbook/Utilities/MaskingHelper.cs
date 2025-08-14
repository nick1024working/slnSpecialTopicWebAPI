namespace prjSpecialTopicWebAPI.Features.Usedbook.Utilities
{
    public class MaskingHelper
    {
        public static string MaskGuid(Guid guid)
        {
            string str = guid.ToString();
            return $"{str[..4]}****-****-****-****-{str[^4..]}";
        }

        public static string MaskFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;

            var chars = fullName.ToCharArray();
            bool[] isCJK = chars.Select(IsChinese).ToArray();

            // 中文優先處理：若前面是中文姓氏（CJK），保留其餘遮蔽
            if (isCJK[0])
            {
                return chars[0] + new string('*', chars.Length - 1);
            }

            // 英文開頭，保留第一個字母與最後一個中文姓氏（若有）
            var visible = new List<char> { chars[0] };
            int lastCJKIndex = Array.FindLastIndex(isCJK, c => c);
            for (int i = 1; i < chars.Length; i++)
            {
                if (i == lastCJKIndex)
                    visible.Add(chars[i]);
                else
                    visible.Add('*');
            }

            return new string(visible.ToArray());
        }

        private static bool IsChinese(char ch)
        {
            return ch >= 0x4e00 && ch <= 0x9fff; // 常見 CJK 統一漢字範圍
        }
    }
}
