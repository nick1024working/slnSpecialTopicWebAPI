using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Options;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace prjSpecialTopicWebAPI.Features.Shared.Service
{
    public class PaymentService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IOptionsMonitor<LinePayOptions> _linePayOptions;
        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };


        public PaymentService(
            IHttpClientFactory factory,
            IOptionsMonitor<LinePayOptions> linePayOptions)
        {
            _factory = factory;
            _linePayOptions = linePayOptions;
        }

        public async Task<PaymentRawResult> RequestLinePayPayment([FromBody] PaymentRequestDto req, CancellationToken ct = default)
        {
            var client = _factory.CreateClient("LinePay");
            var bodyJson = JsonSerializer.Serialize(req, _jsonOptions);


            string apiPath = "/v3/payments/request";
            string nonce = Guid.NewGuid().ToString();
            string message = _linePayOptions.CurrentValue.ChannelSecret + apiPath + bodyJson + nonce;
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_linePayOptions.CurrentValue.ChannelSecret)))
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));

            // 組裝 HttpRequestMessage
            // Note: BaseAddress 已經在 client 註冊時定義
            using var http = new HttpRequestMessage(HttpMethod.Post, apiPath);
            var content = new StringContent(bodyJson, Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
            http.Content = content;

            // 組裝 Header
            http.Headers.Add("X-LINE-ChannelId", _linePayOptions.CurrentValue.ChannelId);
            http.Headers.Add("X-LINE-Authorization", signature);
            http.Headers.Add("X-LINE-Authorization-Nonce", nonce);

            // 發送 request
            using var response = await client.SendAsync(http, ct);
            string responseBody = await response.Content.ReadAsStringAsync(ct);

            return new PaymentRawResult
            {
                StatusCode = (int)response.StatusCode,
                RawBody = responseBody
            };
        }
    }
}
