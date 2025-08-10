namespace prjSpecialTopicWebAPI.Features.Usedbook.Utilities
{
    public class FileHelper
    {
        /// <summary>
        /// 根據指定參數，尋找指定資料夾中符合條件的圖片完整路徑(作業系統格式)。
        /// </summary>
        public static string? GetAbsolutePathByImageId(string id, string basePath, string relativePath, string suffix = "")
        {
            var root = Path.Combine(basePath, relativePath);
            if (!Directory.Exists(root))
                return null;

            var files = Directory.GetFiles(root, $"{id}{suffix}.*");
            return files.FirstOrDefault();
        }

        /// <summary>
        /// 根據指定參數，尋找指定資料夾中符合條件的圖片相對路徑(作業系統格式)。
        /// </summary>
        public static string? GetRelativePathByImageId(string id, string basePath, string relativePath, string suffix = "")
        {
            var root = Path.Combine(basePath, relativePath);
            if (!Directory.Exists(root))
                return null;

            var file = Directory.GetFiles(root, $"{id}{suffix}.*").FirstOrDefault();
            if (file == null)
                return null;

            return Path.GetRelativePath(basePath, file);
        }

        /// <summary>
        /// 嘗試刪除指定資料夾中所有符合條件的檔案。
        /// </summary>
        public static void DeleteFile(string id, string basePath, string relativePath, string suffix = "")
        {
            var root = Path.Combine(basePath, relativePath);
            if (!Directory.Exists(root))
                return;

            var files = Directory.GetFiles(root, $"{id}{suffix}.*");
            foreach( var file in files )
                File.Delete(file);
        }

        /// <summary>
        /// 根據副檔名推斷並回傳對應的 MIME Content-Type。
        /// </summary>
        public static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
