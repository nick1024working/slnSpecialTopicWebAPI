namespace prjSpecialTopicWebAPI.Features.Shared.Utilities
{
    public static class EmailTemplates
    {
        private const string DefaultBannerUrl = "https://i.meee.com.tw/zqvFSB2.jpg";

        private static string RenderLayout(string innerHtml, string bannerUrl)
        {
            return $@"
<table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background:#f6f7fb;'>
  <tr>
    <td align='center' style='padding:24px;'>
      <table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0' 
             style='width:600px; max-width:100%; background:#ffffff; border-radius:12px; overflow:hidden;'>
        <tr>
          <td align='center'>
            <img src='{bannerUrl}' width='100%' 
                 alt='ProBookLand Banner'
                 style='display:block; margin:0 auto; max-width:100%; height:auto; border:0; outline:none; text-decoration:none;' />
          </td>
        </tr>
        {innerHtml}
      </table>
    </td>
  </tr>
</table>";
        }

        public static string BasicHtml(string title, string body, string bannerUrl = DefaultBannerUrl)
        {
            var safeTitle = System.Net.WebUtility.HtmlEncode(title).Replace("\n", "<br>");
            var safeBody = System.Net.WebUtility.HtmlEncode(body).Replace("\n", "<br>");

            var inner = $@"
<tr>
  <td align='center' style='padding:28px 28px 8px 28px; font-family:Arial, Helvetica, sans-serif;'>
    <div style='font-size:22px; line-height:28px; font-weight:bold; color:#222;'>{safeTitle}</div>
  </td>
</tr>
<tr>
  <td align='center' style='padding:16px 28px; font-family:Arial, Helvetica, sans-serif;'>
    <div style='font-size:15px; line-height:22px; color:#444;'>{safeBody}</div>
  </td>
</tr>";

            return RenderLayout(inner, bannerUrl);
        }

        public static string OrderCreatedHtml(string userName, string orderUrl, string bannerUrl = DefaultBannerUrl)
        {
            var safeName = System.Net.WebUtility.HtmlEncode(userName);
            var safeUrl = System.Net.WebUtility.HtmlEncode(orderUrl);

            var inner = $@"
<tr>
  <td align='center' style='padding:28px 28px 8px 28px; font-family:Arial, Helvetica, sans-serif;'>
    <div style='font-size:22px; line-height:28px; font-weight:bold; color:#222;'>ProBookLand 通知</div>
  </td>
</tr>
<tr>
  <td align='center' style='padding:16px 28px; font-family:Arial, Helvetica, sans-serif;'>
    <div style='font-size:15px; line-height:22px; color:#444;'>
      {safeName} 您好，<br>
      您的訂單已建立，我們正在為您處理。<br>
      點擊下方按鈕即可查看詳情。
    </div>
  </td>
</tr>
<tr>
  <td align='center' style='padding:12px 28px 28px 28px;'>
    <a href='{safeUrl}' 
       style='display:inline-block; background:#3b82f6; padding:12px 22px; font-family:Arial, Helvetica, sans-serif; 
              font-size:14px; line-height:18px; color:#ffffff; text-decoration:none; border-radius:999px;'>
      查看訂單
    </a>
  </td>
</tr>
<tr>
  <td align='center' style='padding:0 28px 24px 28px; font-family:Arial, Helvetica, sans-serif;'>
    <div style='font-size:12px; line-height:18px; color:#8b8b8b;'>
      若按鈕無法點擊，請開啟：<br>
      <span style='word-break:break-all; color:#555;'>{safeUrl}</span>
    </div>
  </td>
</tr>";

            return RenderLayout(inner, bannerUrl);
        }
    }
}
