using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Extensions;

namespace prjSpecialTopicWebAPI.Features.Shared.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{key}")]
        public ActionResult<CartItemDto> GetCart([FromRoute] string key)
        {
            var result = HttpContext.Session.GetObject<CartItemDto>(key);
            return result is null ? NotFound() : Ok(result);
        }

        // 此處先不管設計
        [HttpPut("{key}")]
        public ActionResult AddToCart([FromRoute] string key, [FromBody] CartItemDto dto)
        {
            HttpContext.Session.SetObject<CartItemDto>(key, dto);
            return NoContent();
        }

        [HttpDelete("{key}")]
        public ActionResult RemoveFromCart([FromRoute] string key)
        {
            HttpContext.Session.RemoveObject(key);
            return NoContent();
        }
    }
}
