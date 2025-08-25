using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Extensions;

namespace prjSpecialTopicWebAPI.Features.Shared.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Route("api/carts")]
    public class CartController : ControllerBase
    {
        private const string CartKey = "CART";
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<CartDto> GetCart()
        {
            var result = HttpContext.Session.GetObject<CartDto>(CartKey) ?? new CartDto();

            Recalculate(result);

            HttpContext.Session.SetObject(CartKey, result);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult ReplaceCart([FromBody] CartDto dto)
        {
            dto.UpdatedAt = DateTime.UtcNow;
            Recalculate(dto);

            HttpContext.Session.SetObject(CartKey, dto);
            return NoContent();
        }

        // TODO: 待處理
        // NOTE: 此處未把邏輯分到 BLL
        [HttpPatch("items")]
        public IActionResult UpsertItem([FromBody] PatchItemRequest req)
        {
            return NoContent();
        }

        [HttpDelete("items/{id}")]
        public IActionResult RemoveItemFromCart([FromRoute] string id)
        {
            var result = HttpContext.Session.GetObject<CartDto>(CartKey) ?? new CartDto();

            result.Items.RemoveAll(i => i.Id == id);
            result.UpdatedAt = DateTime.UtcNow;
            Recalculate(result);

            HttpContext.Session.SetObject(CartKey, result);
            return NoContent();
        }

        [HttpDelete]
        public IActionResult ClearCart()
        {
            HttpContext.Session.RemoveObject(CartKey);
            return NoContent();
        }

        /// <summary>
        /// 重新計算 cart 的計算欄位
        /// </summary>
        private static void Recalculate(CartDto cart)
        {
            cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
            cart.GrandTotal = cart.Subtotal - cart.DiscountTotal + cart.ShippingFee;
        }
    }
}
