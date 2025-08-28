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
        private const string CheckoutKey = "CHECKOUTDRAFT";

        public CartController() { }

        // ========== 購物車 ==========

        [HttpGet]
        public ActionResult<AllCartsDto> GetCart()
        {
            var sessionId = HttpContext.Session.Id;
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

        //[HttpPut]
        //public IActionResult ReplaceCart([FromBody] AllCartsDto allCarts)
        //{
        //    allCarts.UpdatedAt = DateTime.UtcNow;
        //    Recalculate(allCarts);

        //    HttpContext.Session.SetObject(CartKey, allCarts);
        //    return NoContent();
        //}

        [HttpPatch("items")]
        public IActionResult UpsertItem([FromBody] UpsertCartItemRequest req)
        {
            var sessionId = HttpContext.Session.Id;
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

        [HttpPatch("delivery")]
        public IActionResult UpdateDelivery([FromBody] UpdateDeliveryRequest req)
        {
            if (req.DeliveryFee < 0)
                return BadRequest("運費金額錯誤");

            var sid = HttpContext.Session.Id;
            var allCarts = HttpContext.Session.GetObject<AllCartsDto>(CartKey) ?? new AllCartsDto();
            if (!allCarts.Carts.ContainsKey(req.ProductProvider))
                return BadRequest("該購物車並不存在");
            var nowCart = allCarts.Carts[req.ProductProvider];

            nowCart.DeliveryFee = req.DeliveryFee;
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

        // ========== 結帳草稿 ==========

        [HttpGet("checkout-draft")]
        public IActionResult GetCheckoutDraft()
        {
            var checkoutDraft = HttpContext.Session.GetObject<CheckoutDraftDto>(CheckoutKey);
            if (checkoutDraft == null)
                return NotFound();
            return Ok(checkoutDraft);
        }

        [HttpPut("checkout-draft")]
        public IActionResult UpsertCheckoutDraft([FromBody] CheckoutDraftDto req)
        {
            var checkoutDraft = HttpContext.Session.GetObject<CheckoutDraftDto>(CheckoutKey) ?? new CheckoutDraftDto();
            checkoutDraft.ProductProvider = req.ProductProvider;
            checkoutDraft.DeliveryOption = req.DeliveryOption;
            checkoutDraft.PaymentOption = req.PaymentOption;

            checkoutDraft.BuyerName = req.BuyerName;
            checkoutDraft.BuyerEmail = req.BuyerEmail;
            checkoutDraft.BuyerPhone = req.BuyerPhone;

            checkoutDraft.ReceiverName = req.ReceiverName;
            checkoutDraft.ReceiverPhone = req.ReceiverPhone;

            checkoutDraft.CountyId = req.CountyId;
            checkoutDraft.DistrictId = req.DistrictId;
            checkoutDraft.Address = req.Address;


            HttpContext.Session.SetObject(CheckoutKey, checkoutDraft);
            return NoContent();
        }

        // ========== 私有方法 ==========

        /// <summary>
        /// 重新計算 AllCartsDto 中的所有計算欄位
        /// </summary>
        private static void Recalculate(AllCartsDto allCarts)
        {
            allCarts.GrandTotal = 0;
            foreach (var (_, cart) in allCarts.Carts)
            {
                cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
                cart.GrandTotal = cart.Subtotal - cart.DiscountTotal + cart.DeliveryFee;
                allCarts.GrandTotal += cart.GrandTotal;
            }
        }
    }
}
