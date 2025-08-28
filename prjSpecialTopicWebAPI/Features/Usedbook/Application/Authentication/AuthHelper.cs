using Microsoft.AspNetCore.DataProtection;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication
{
    public class AuthHelper
    {
        private readonly IDataProtector _protector;

        public AuthHelper(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("UserIdCookie");
        }

        public void SetSeller(Guid userId, HttpContext ctx)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(30) 
            };

            var value = _protector.Protect(userId.ToString());
            ctx.Response.Cookies.Append(".UserId", value, cookieOptions);
        }

        public Guid? GetSeller(HttpContext ctx)
        {
            if (!ctx.Request.Cookies.TryGetValue(".UserId", out string? raw))
                return null;
            try
            {
                var userIdStr = _protector.Unprotect(raw);
                return Guid.TryParse(userIdStr, out var id) ? id : null;
            }
            catch
            {
                return null;
            }
        }

        public void ClearSeller(HttpContext ctx)
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };

            ctx.Response.Cookies.Delete(".UserId", cookieOptions);
        }
    }
}
