using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/test")]
    public class ATestController: ControllerBase
    {

        [HttpGet("")]
        public IActionResult SetNormalCookie()
        {
            Response.Cookies.Append("NORMAL", "some ordinary data", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            Response.Cookies.Append("user_id", "U12345", new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(3),
            });

            Response.Cookies.Append("authorize", "Bearer abc.def.ghi", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(5),
            });


            return Ok(new { message = "Multiple cookies set" });
        }

        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            return NoContent();
        }
    }
}
