using Microsoft.Extensions.Options;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace prjSpecialTopicWebAPI.Features.Shared.Service
{
    public class LinePayService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IOptionsMonitor<LinePayOptions> _linePayOptions;
        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly ILogger<LinePayService> _logger;

        public LinePayService(
            IHttpClientFactory factory,
            IOptionsMonitor<LinePayOptions> linePayOptions,
            ILogger<LinePayService> logger)
        {
            _factory = factory;
            _linePayOptions = linePayOptions;
            _logger = logger;
        }

        public async Task<LinePayRequestResponseDto> RequestLinePayPaymentAsync(LinePayPaymentRequestDto req, CancellationToken ct = default)
        {
            var client = _factory.CreateClient("LinePay");
            var bodyJson = JsonSerializer.Serialize(req, _jsonOptions);
            string apiPath = "/v3/payments/request";

            // 發送 request
            using var request = BuildSignedPost(apiPath, bodyJson);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            var result = await response.Content.ReadFromJsonAsync<LinePayRequestResponseDto>(_jsonOptions, ct);

            if (result == null)
                throw new InvalidOperationException("LINE Pay 回應無法解析");
            return result;
        }

        public async Task<LinePayPaymentCheckResponse> CheckLinePayPaymentAsync(string transactionId, CancellationToken ct = default)
        {
            var client = _factory.CreateClient("LinePay");
            string apiPath = $"/v3/payments/requests/{transactionId}/check";

            using var request = BuildSignedGet(apiPath, queryString: null);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LinePayPaymentCheckResponse>(_jsonOptions, ct);

            if (result == null)
                throw new InvalidOperationException("LINE Pay 回應無法解析");
            return result;
        }

        public async Task<LinePayPaymentConfirmResponse> ConfirmLinePayPaymentAsync(string transactionId, LinePayPaymentConfirmDto req, CancellationToken ct = default)
        {
            var client = _factory.CreateClient("LinePay");
            var bodyJson = JsonSerializer.Serialize(req, _jsonOptions);
            string apiPath = $"/v3/payments/{transactionId}/confirm";

            using var request = BuildSignedPost(apiPath, bodyJson);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            var result = await response.Content.ReadFromJsonAsync<LinePayPaymentConfirmResponse>(_jsonOptions, ct);

            if (result == null)
                throw new InvalidOperationException("LINE Pay 回應無法解析");
            return result;
        }

        private HttpRequestMessage BuildSignedPost(string apiPath, string bodyJson)
        {
            var opt = _linePayOptions.CurrentValue;
            var nonce = Guid.NewGuid().ToString();

            // 簽章 = HMACSHA256(ChannelSecret, ChannelSecret + apiPath + body + nonce)
            var message = opt.ChannelSecret + apiPath + bodyJson + nonce;
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(opt.ChannelSecret)))
            {
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
            }

            //_logger.LogInformation("[LINEPAY][POST] ChannelId={ChannelId} apiPath={ApiPath} nonce={Nonce} signMsg(len={Len})={SignMsg}",
            //    opt.ChannelId, apiPath, nonce, message.Length, message);

            var req = new HttpRequestMessage(HttpMethod.Post, apiPath)
            {
                Content = new StringContent(bodyJson, Encoding.UTF8, "application/json")
            };

            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req.Headers.Add("X-LINE-ChannelId", opt.ChannelId);
            req.Headers.Add("X-LINE-Authorization-Nonce", nonce);
            req.Headers.Add("X-LINE-Authorization", signature);

            return req;
        }

        private HttpRequestMessage BuildSignedGet(string apiPath, string? queryString)
        {
            var opt = _linePayOptions.CurrentValue;
            var nonce = Guid.NewGuid().ToString();

            var qs = queryString ?? string.Empty;
            var message = opt.ChannelSecret + apiPath + qs + nonce;

            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(opt.ChannelSecret)))
            {
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
            }

            //_logger.LogInformation("[LINEPAY][GET] ChannelId={ChannelId} apiPath={ApiPath} qs='{Qs}' nonce={Nonce} signMsg(len={Len})={SignMsg}",
            //    opt.ChannelId, apiPath, qs, nonce, message.Length, message);

            var url = string.IsNullOrEmpty(qs) ? apiPath : $"{apiPath}?{qs}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req.Headers.Add("X-LINE-ChannelId", opt.ChannelId);
            req.Headers.Add("X-LINE-Authorization-Nonce", nonce);
            req.Headers.Add("X-LINE-Authorization", signature);

            return req;
        }
    }
}
