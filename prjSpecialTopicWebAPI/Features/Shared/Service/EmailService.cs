using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using prjSpecialTopicWebAPI.Features.Shared.Options;
using prjSpecialTopicWebAPI.Features.Shared.Utilities;

namespace prjSpecialTopicWebAPI.Features.Shared.Service
{
    public class EmailService
    {
        private readonly SmtpOptions _opt;

        public EmailService(IOptions<SmtpOptions> opt)
        {
            _opt = opt.Value;
        }

        public async Task SendBasicEmailAsync(string to, string subject, string title, string body, CancellationToken ct = default)
        {
            var builder = new BodyBuilder
            {
                HtmlBody = EmailTemplates.BasicHtml(title, body),
                TextBody = $"{title}\n\n{body}"
            };
            await SendAsync(to, subject, builder.ToMessageBody(), ct);

        }

        public async Task SendOrderCreatedEmailAsync(string to, string subject, string userName, string orderUrl, CancellationToken ct = default)
        {
            var builder = new BodyBuilder
            {
                HtmlBody = EmailTemplates.OrderCreatedHtml(userName, orderUrl),
                TextBody = $"{userName} 您好，\n您的訂單已建立。\n詳情請見：{orderUrl}"
            };
            await SendAsync(to, subject, builder.ToMessageBody(), ct);

        }

        private async Task SendAsync(string to, string subject, MimeEntity entity, CancellationToken ct = default)
        {
            try
            {
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(_opt.FromName, _opt.FromAddress));
                msg.To.Add(MailboxAddress.Parse(to));
                msg.Subject = subject;
                msg.Body = entity;

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_opt.Host, _opt.Port, SecureSocketOptions.StartTls, ct);
                await smtp.AuthenticateAsync(_opt.UserName, _opt.Password, ct);
                await smtp.SendAsync(msg, ct);
                await smtp.DisconnectAsync(true, ct);
            }
            catch (SmtpCommandException ex)
            {
                throw new InvalidOperationException($"SMTP 命令失敗 ({ex.StatusCode}): {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"寄信失敗: {ex.Message}", ex);
            }

        }
        public async Task SendHtmlAsync(string to,string subject,string html,string?textFallback = null,CancellationToken ct = default)
        {
            var builder = new BodyBuilder
            {
                HtmlBody = html,
              
                TextBody = string.IsNullOrWhiteSpace(textFallback)
                    ? "請在支援 HTML 的郵件客戶端開啟此信，或將內文中的重設連結複製到瀏覽器。"
                    : textFallback
            };

            await SendAsync(to, subject, builder.ToMessageBody(), ct);
        }
    }
}
