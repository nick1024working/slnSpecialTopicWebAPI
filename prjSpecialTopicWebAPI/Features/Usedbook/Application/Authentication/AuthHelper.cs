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
            => ctx.Response.Cookies.Append(".UserId", _protector.Protect(userId.ToString()));

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
            => ctx.Response.Cookies.Delete(".UserId");
    }
}
