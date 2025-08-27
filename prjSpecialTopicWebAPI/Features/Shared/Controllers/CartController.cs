using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Enums;
using prjSpecialTopicWebAPI.Features.Shared.Extensions;

namespace prjSpecialTopicWebAPI.Features.Shared.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Route("api/carts")]
    public class CartController : ControllerBase
    {
        private const string CartKey = "CART";

        public CartController() { }

        [HttpGet]
        public ActionResult<AllCartsDto> GetCart()
        {
            var sid = HttpContext.Session.Id;
            var allCarts = HttpContext.Session.GetObject<AllCartsDto>(CartKey) ?? new AllCartsDto();

            return Ok(allCarts);
        }

        [HttpGet("items/{provider}")]
        public ActionResult<CartDto> GetCartByProvider([FromRoute] ProductProvider provider)
        {
            var allCarts = HttpContext.Session.GetObject<AllCartsDto>(CartKey) ?? new AllCartsDto();
            if (!allCarts.Carts.ContainsKey(provider))
                return NoContent();
            var nowCart = allCarts.Carts[provider];

            return Ok(nowCart);
        }

        [HttpPut]
        public IActionResult ReplaceCart([FromBody] AllCartsDto allCarts)
        {
            allCarts.UpdatedAt = DateTime.UtcNow;
            Recalculate(allCarts);

            HttpContext.Session.SetObject(CartKey, allCarts);
            return NoContent();
        }

        [HttpPatch("items")]
        public IActionResult UpsertItem([FromBody] UpsertCartItemRequest req)
        {
            var sid = HttpContext.Session.Id;
            var allCarts = HttpContext.Session.GetObject<AllCartsDto>(CartKey) ?? new AllCartsDto();
            allCarts.Carts.TryAdd(req.ProductProvider, new CartDto());
            var nowCart = allCarts.Carts[req.ProductProvider];
            int index = nowCart.Items.FindIndex(i => i.Id == req.Id);

            // 找不到就新增，找到就覆蓋
            if (index == -1)
            {
                if (string.IsNullOrWhiteSpace(req.Name))
                    return BadRequest("新增的商品欄位不全 (缺少 Name)");
                if (string.IsNullOrWhiteSpace(req.ImageUrl))
                    return BadRequest("新增的商品欄位不全 (缺少 ImageUrl)");
                if (req.UnitPrice == null || req.UnitPrice < 0)
                    return BadRequest("新增的商品欄位不全 (缺少 UnitPrice)");

                nowCart.Items.Add(new CartItemDto
                {
                    Id = req.Id,
                    Name = req.Name.Trim(),
                    ImageUrl = req.ImageUrl.Trim(),
                    UnitPrice = (decimal)req.UnitPrice,
                    Quantity = req.Quantity
                });
            }
            else
            {
                if (req.Quantity > 0)
                {
                    nowCart.Items[index].Name = req.Name ?? nowCart.Items[index].Name;
                    nowCart.Items[index].ImageUrl = req.ImageUrl ?? nowCart.Items[index].ImageUrl;
                    nowCart.Items[index].UnitPrice = req.UnitPrice ?? nowCart.Items[index].UnitPrice;
                    nowCart.Items[index].Quantity = req.Quantity;
                }
                else
                    nowCart.Items.RemoveAt(index);
            }

            nowCart.UpdatedAt = DateTime.UtcNow;
            allCarts.UpdatedAt = DateTime.UtcNow;
            Recalculate(allCarts);

            HttpContext.Session.SetObject(CartKey, allCarts);
            return NoContent();
        }

        [HttpDelete("items/{provider}/{id}")]
        public IActionResult RemoveItemFromCart([FromRoute] ProductProvider provider, [FromRoute] string id)
        {
            var allCarts = HttpContext.Session.GetObject<AllCartsDto>(CartKey) ?? new AllCartsDto();
            if (!allCarts.Carts.ContainsKey(provider))
                return NoContent();
            var nowCart = allCarts.Carts[provider];

            nowCart.Items.RemoveAll(i => i.Id == id);

            nowCart.UpdatedAt = DateTime.UtcNow;
            allCarts.UpdatedAt = DateTime.UtcNow;
            Recalculate(allCarts);

            HttpContext.Session.SetObject(CartKey, allCarts);
            return NoContent();
        }

        [HttpDelete]
        public IActionResult ClearCart()
        {
            HttpContext.Session.RemoveObject(CartKey);
            return NoContent();
        }

        /// <summary>
        /// 重新計算 AllCartsDto 中的所有計算欄位
        /// </summary>
        private static void Recalculate(AllCartsDto allCarts)
        {
            allCarts.GrandTotal = 0;
            foreach (var (_, cart) in allCarts.Carts)
            {
                cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
                cart.GrandTotal = cart.Subtotal - cart.DiscountTotal + cart.ShippingFee;
                allCarts.GrandTotal += cart.GrandTotal;
            }
        }
    }
}
