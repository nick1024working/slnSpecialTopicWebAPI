using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/linepay")]
    public class LinePayController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;
        private const string _channelId = "2007934205";
        private const string _channelSecret = "d1ded1d6ff3e34383b7d3b4c9d7121ba";
        private readonly ILogger<LinePayController> _logger;

        public LinePayController(IHttpClientFactory factory, ILogger<LinePayController> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPaymentAsync([FromBody] PaymentRequestDto req, CancellationToken ct)
        {
            var client = _factory.CreateClient("LinePay");
            var bodyJson = JsonSerializer.Serialize(req,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


            string apiPath = "/v3/payments/request";
            string nonce = Guid.NewGuid().ToString();
            string message = _channelSecret + apiPath + bodyJson + nonce;
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_channelSecret)))
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));

            // 組裝 HttpRequestMessage
            // Note: BaseAddress 已經在 client 註冊時定義
            using var http = new HttpRequestMessage(HttpMethod.Post, apiPath);
            var content = new StringContent(bodyJson, Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
            http.Content = content;

            // 組裝 Header
            http.Headers.Add("X-LINE-ChannelId", _channelId);
            http.Headers.Add("X-LINE-Authorization", signature);
            http.Headers.Add("X-LINE-Authorization-Nonce", nonce);

            _logger.LogInformation("LinePay request: nonce={Nonce}, apiPath={ApiPath}, bodySha256={BodySha256}, sigBase64={Sig}",
                nonce,
                apiPath,
                Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(bodyJson))),
                signature);
            var response = await client.SendAsync(http, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);
            return StatusCode((int)response.StatusCode, responseBody);
        }
    }
}
