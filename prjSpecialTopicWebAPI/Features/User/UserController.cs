using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prjSpecialTopicWebAPI.Features.Users;
using prjSpecialTopicWebAPI.Models;
using Microsoft.AspNetCore.Http;
using prjSpecialTopicWebAPI.Features.Shared.Options;
using prjSpecialTopicWebAPI.Features.Shared.Service;
using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace prjSpecialTopicWebAPI.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly TeamAProjectContext _db;
    private readonly IConfiguration _config;

    private readonly IOptions<PasswordResetOptions> _resetOpts;
    private readonly EmailService _email;
    public UsersController(
        TeamAProjectContext db,
        IConfiguration config,
        IOptions<PasswordResetOptions> resetOpts,
        EmailService email)
    {
        _db = db;
        _config = config;
        _resetOpts = resetOpts;
        _email = email;
    }

    //重設密碼Email用
    private static bool IsStrongPassword(string p) =>
    !string.IsNullOrWhiteSpace(p) && p.Length >= 8 && p.Any(char.IsLetter) && p.Any(char.IsDigit);
   

    [AllowAnonymous]
    //  驗證 token（前端載入 /member/reset 會先打這支）[AllowAnonymous]
    [HttpGet("reset-password/validate")]
    public IActionResult ValidateResetToken([FromQuery] string token)
    {
        var raw = NormalizeToken(token);
        if (string.IsNullOrWhiteSpace(raw)) return BadRequest("缺少 token");

        var opts = _resetOpts.Value;
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };

        // 先只解析，不驗簽章
        JwtSecurityToken t;
        try { t = handler.ReadJwtToken(raw); }
        catch { return BadRequest("token 格式錯誤"); }

        // 抓出 exp，沒有就當作不合法
        if (!TryGetUnixTime(t, "exp", out var expSec))
            return BadRequest("token 缺少 exp");

        var skew = TimeSpan.FromMinutes(5);
        var expUtc = DateTimeOffset.FromUnixTimeSeconds(expSec).UtcDateTime;
        if (UtcNow() > expUtc + skew) return BadRequest("token 已過期");

        // 類型檢查（purpose/typ = pwdreset）
        bool isPwdReset =
            (t.Payload.TryGetValue("purpose", out var pv) && string.Equals(pv?.ToString(), "pwdreset", StringComparison.OrdinalIgnoreCase)) ||
            (t.Payload.TryGetValue("typ", out var tv) && string.Equals(tv?.ToString(), "pwdreset", StringComparison.OrdinalIgnoreCase));
        if (!isPwdReset) return BadRequest("token 類型錯誤");

        // 驗簽章（壽命我們已手動檢查，所以關掉 ValidateLifetime）
        var p = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Secret)),
            ValidateLifetime = false
        };

        try { handler.ValidateToken(raw, p, out _); }
        catch (SecurityTokenInvalidSignatureException) { return BadRequest("token 簽章無效"); }
        catch { return BadRequest("token 無效"); }

        return Ok(new { ok = true });
    }

    [AllowAnonymous]
    [HttpGet("reset-password/dev-gentoken")]
    public IActionResult DevGenToken()
    {
        var opts = _resetOpts.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var sub = Guid.NewGuid().ToString(); // 測試用 sub

        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = opts.Issuer,
            Audience = opts.Audience,
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, sub),
            new Claim("purpose", "pwdreset"),
            new Claim("typ",     "pwdreset"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
            NotBefore = now.AddMinutes(-2),
            Expires = now.AddMinutes(opts.ExpireMinutes),
            SigningCredentials = creds
        };

        var token = handler.CreateToken(descriptor);
        var jwt = handler.WriteToken(token);
        return Ok(new { token = jwt });
    }
    // 真正重設密碼（表單送出）
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken ct)
    {
        var raw = NormalizeToken(dto.Token);
        if (string.IsNullOrWhiteSpace(raw)) return BadRequest("缺少 token");
        if (dto.NewPassword != dto.ConfirmPassword) return BadRequest("兩次密碼不一致");
        if (!IsStrongPassword(dto.NewPassword)) return BadRequest("新密碼需至少 8 碼，且同時包含英文與數字");

        var opts = _resetOpts.Value;
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };

        JwtSecurityToken t;
        try { t = handler.ReadJwtToken(raw); }
        catch { return BadRequest("token 格式錯誤"); }

        if (!TryGetUnixTime(t, "exp", out var expSec)) return BadRequest("token 缺少 exp");
        var expUtc = DateTimeOffset.FromUnixTimeSeconds(expSec).UtcDateTime;
        if (UtcNow() > expUtc + TimeSpan.FromMinutes(5)) return BadRequest("token 已過期");

        bool isPwdReset =
            (t.Payload.TryGetValue("purpose", out var pv) && string.Equals(pv?.ToString(), "pwdreset", StringComparison.OrdinalIgnoreCase)) ||
            (t.Payload.TryGetValue("typ", out var tv) && string.Equals(tv?.ToString(), "pwdreset", StringComparison.OrdinalIgnoreCase));
        if (!isPwdReset) return BadRequest("token 類型錯誤");

        var p = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Secret)),
            ValidateLifetime = false
        };

        ClaimsPrincipal principal;
        try { principal = handler.ValidateToken(raw, p, out _); }
        catch (SecurityTokenInvalidSignatureException) { return BadRequest("token 簽章無效"); }
        catch { return BadRequest("token 無效"); }

        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? principal.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var uid)) return BadRequest("token 損毀");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Uid == uid, ct);
        if (user is null || user.Status == 0) return NotFound("找不到使用者");

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.AccountOrEmail)) return BadRequest("請輸入帳號或 Email");

        var key = dto.AccountOrEmail.Trim();
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Phone == key || u.Email.ToLower() == key.ToLower(), ct);

        // 不暴露帳號是否存在：找不到也回 204
        if (user == null || user.Status == 0) return NoContent();

        var opts = _resetOpts.Value;
        var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Secret));
        var creds = new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = opts.Issuer,
            Audience = opts.Audience,
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Uid.ToString()),
            new Claim("purpose", "pwdreset"),
            new Claim("typ",     "pwdreset"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }),
            NotBefore = now.AddMinutes(-2),
            Expires = now.AddMinutes(opts.ExpireMinutes),
            SigningCredentials = creds
        };

        var token = handler.CreateToken(descriptor);
        var jwt = handler.WriteToken(token);

        var resetUrl = $"{opts.FrontBaseUrl.TrimEnd('/')}/member/reset?token={Uri.EscapeDataString(jwt)}";

        var html = $@"
<p>您好{(string.IsNullOrEmpty(user.Name) ? "" : $"，{WebUtility.HtmlEncode(user.Name)}")}：</p>
<p>請在 <b>{opts.ExpireMinutes}</b> 分鐘內點擊下方按鈕重設密碼：</p>
<p>
  <a href=""{resetUrl}""
     style=""display:inline-block;padding:10px 14px;background:#2563eb;color:#fff;text-decoration:none;border-radius:6px"">
     重設密碼
  </a>
</p>
<p>若按鈕無法點擊，請複製以下完整連結到瀏覽器開啟：</p>
<p style=""word-break:break-all"">{WebUtility.HtmlEncode(resetUrl)}</p>
<p>若非本人操作可忽略此信。</p>";

        await _email.SendHtmlAsync(user.Email, "ProBookLand 重設密碼連結", html, ct: ct);
        return NoContent();
    }
    [AllowAnonymous]
    [HttpGet("reset-password/peek")]
    public IActionResult PeekToken([FromQuery] string token)
    {
        var raw = NormalizeToken(token);
        if (string.IsNullOrWhiteSpace(raw)) return BadRequest("缺少 token");

        var t = new JwtSecurityTokenHandler().ReadJwtToken(raw);
        var claims = t.Claims.Select(c => new { c.Type, c.Value });
        long? exp = TryGetUnixTime(t, "exp", out var v) ? v : null;

        return Ok(new
        {
            header = t.Header,
            claims,
            validFromUtc = t.ValidFrom,
            validToUtc = t.ValidTo,
            payloadExp = exp,
            payloadNbf = t.Payload.Nbf,
            payloadIat = t.Payload.Iat
        });
    }
    // 讀取列表（受 JWT 保護）
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> GetList([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.Users.Where(u => u.Status != 0);
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(u => u.Phone.Contains(q) || u.Email.Contains(q) || u.Name.Contains(q));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.RegisterDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemDto(u.Uid, u.Phone, u.Name, u.Email, u.Gender, u.Birthday, u.AvatarUrl, u.Status, u.Level))
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    // 讀取單筆（受 JWT 保護）
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDetailDto>> Get(Guid id)
    {
        var u = await _db.Users.FindAsync(id);
        if (u is null || u.Status == 0) return NotFound();
        return Ok(new UserDetailDto(u.Uid, u.Phone, u.Name, u.Email, u.Gender, u.Birthday, u.Address, u.RegisterDate, u.LastLoginDate, u.AvatarUrl, u.Status, u.Level));
    }

    // 註冊（密碼雜湊）
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDetailDto>> Register([FromBody] RegisterDto dto)
    {
        // 基本檢查
        if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest("缺少密碼");
        bool hasLetter = dto.Password.Any(char.IsLetter);
        bool hasDigit = dto.Password.Any(char.IsDigit);
        if (dto.Password.Length < 8 || !hasLetter || !hasDigit)
            return BadRequest("密碼需至少 8 碼，且同時包含英文與數字");

        var phone = dto.Phone.Trim();
        var email = dto.Email.Trim();

        // Email 不分大小寫檢查；Phone 依原樣
        var exists = await _db.Users.AnyAsync(x =>
            x.Phone == phone || x.Email.ToLower() == email.ToLower());
        if (exists) return Conflict("Phone 或 Email 已存在");

        var user = new User
        {
            Uid = Guid.NewGuid(),
            Phone = phone,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Name = dto.Name.Trim(),
            Email = email,
            Gender = dto.Gender,
            Birthday = dto.Birthday,
            Address = dto.Address?.Trim(),
            RegisterDate = DateTime.UtcNow,
            Status = 1,
            Level = 0
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new UserDetailDto(user.Uid, user.Phone, user.Name, user.Email, user.Gender,
            user.Birthday, user.Address, user.RegisterDate, user.LastLoginDate, user.AvatarUrl,
            user.Status, user.Level));
    }

    // 登入（驗證密碼 + 回 JWT）
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        var account = (dto?.Account ?? string.Empty).Trim();
        var plain = (dto?.Password ?? string.Empty);

        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(plain))
            return BadRequest("缺少帳號或密碼");

        // Email 不分大小寫；Phone 依原樣比對
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Phone == account || u.Email.ToLower() == account.ToLower());

        if (user is null || user.Status == 0)
            return Unauthorized("帳號不存在或已停用");

        var hashed = user.Password ?? string.Empty;
        bool isBcrypt = hashed.StartsWith("$2a$") || hashed.StartsWith("$2b$") || hashed.StartsWith("$2y$");
        bool ok = false;
        bool upgraded = false;

        if (isBcrypt)
        {
            try
            {
                ok = BCrypt.Net.BCrypt.Verify(plain, hashed);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // 雜湊字串可能被截斷/損毀 視為驗證失敗
                ok = false;
            }
        }
        else
        {
            // 舊資料（明碼）相容
            if (hashed == plain)
            {
                ok = true;
                // 只升級，不在這裡 Save；統一在結尾 Save 一次
                user.Password = BCrypt.Net.BCrypt.HashPassword(plain);
                upgraded = true;
            }
        }

        if (!ok) return Unauthorized("帳號或密碼錯誤");

        // 在這裡更新最後登入時間與升級後的密碼
        user.LastLoginDate = DateTime.UtcNow;
        if (upgraded)
        {
            // 這裡可以寫一筆 LoginLogs
        }
        await _db.SaveChangesAsync();

        var token = GenerateJwt(user);

        // 寫入 Cookie
        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,                  // JS 無法讀取，防 XSS
            Secure = true,                    // 只在 HTTPS 傳
            SameSite = SameSiteMode.None,     // 跨站 withCredentials 必須
            Path = "/",                       // 全站可帶
        });

        var detail = new UserDetailDto(
            user.Uid, user.Phone, user.Name, user.Email, user.Gender, user.Birthday,
            user.Address, user.RegisterDate, user.LastLoginDate, user.AvatarUrl, user.Status, user.Level
        );

        return Ok(new { user = detail });
    }

    // 登出（清除 Cookie）
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Append("auth_token", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UnixEpoch // 立即過期
        });
        return Ok();
    }
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        // 確認是不是本人（JWT 內的 Sub 要等於 id）
        var uidStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (uidStr != user.Uid.ToString()) return Forbid();

        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Address))
            user.Address = dto.Address.Trim();
        if (dto.Birthday.HasValue)
            user.Birthday = dto.Birthday.Value;

        await _db.SaveChangesAsync();
        return Ok(new UserDetailDto(user.Uid, user.Phone, user.Name, user.Email,
            user.Gender, user.Birthday, user.Address, user.RegisterDate,
            user.LastLoginDate, user.AvatarUrl, user.Status, user.Level));
    }

    //  修改密碼（受 JWT 保護）
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return BadRequest("新密碼與確認密碼不一致");

        bool hasLetter = dto.NewPassword.Any(char.IsLetter);
        bool hasDigit = dto.NewPassword.Any(char.IsDigit);
        if (dto.NewPassword.Length < 8 || !hasLetter || !hasDigit)
            return BadRequest("新密碼需至少 8 碼，且同時包含英文與數字");

        var uidStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(uidStr, out var uid)) return Unauthorized();

        var user = await _db.Users.FindAsync(uid);
        if (user is null || user.Status == 0) return NotFound();

        var hashed = user.Password ?? "";
        bool isBcrypt = hashed.StartsWith("$2a$") || hashed.StartsWith("$2b$") || hashed.StartsWith("$2y$");
        bool okOld = false;

        if (isBcrypt)
        {
            try { okOld = BCrypt.Net.BCrypt.Verify(dto.OldPassword, hashed); }
            catch (BCrypt.Net.SaltParseException) { okOld = false; }
        }
        else
        {
            // 舊資料（明碼）相容
            okOld = hashed == dto.OldPassword;
        }

        if (!okOld) return Unauthorized("舊密碼錯誤");

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();
        return Ok(new { message = "密碼已更新成功" });
    }

    //  刪除會員（受 JWT 保護）
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var uidStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (uidStr != user.Uid.ToString()) return Forbid();

        // 軟刪除 → 改 Status = 0
        user.Status = 0;
        await _db.SaveChangesAsync();

        return Ok(new { message = "會員已停用" });
    }
    // 目前使用者（需授權；JwtBearer 會從 Header or Cookie 取 token）
    [Authorize]
    [HttpGet("me")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Me()
    {
        var uidStr =
     User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value   //  關映射後用這個
     ?? User.FindFirst("uid")?.Value;                     // 兼容之前的備援
        if (string.IsNullOrEmpty(uidStr)) return Unauthorized();

        if (!Guid.TryParse(uidStr, out var uid)) return Unauthorized();

        var me = await _db.Users
       .Where(x => x.Uid == uid && x.Status != 0)
       .Select(x => new
       {
           x.Uid,
           x.Name,
           x.Email,
           x.Phone,
           x.Address,
           x.Birthday,
           x.AvatarUrl,
           x.Status,
           x.Level
       })
       .FirstOrDefaultAsync();

        return Ok(me);
    }
    private string GenerateJwt(User user)
    {
        // 把金鑰放到 appsettings.json: "Jwt": { "Key": "隨機字串" }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Uid.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Phone),
                new("name", user.Name),
                new("level", (user.Level ?? 0).ToString())
            };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public class UploadAvatarForm
    {
        public IFormFile File { get; set; } = default!;
    }

    [HttpPost("upload-avatar")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarForm form)
    {
        var file = form.File;
        if (file == null || file.Length == 0)
            return BadRequest("請選擇圖片");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("檔案大小不可超過 5MB");

        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
        if (!allowedExts.Contains(ext))
            return BadRequest("僅支援 jpg / jpeg / png 格式");

        var uidStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(uidStr, out var uid)) return Unauthorized();

        var user = await _db.Users.FindAsync(uid);
        if (user is null) return NotFound();

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{uid}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.AvatarUrl = $"/uploads/avatars/{fileName}";
        await _db.SaveChangesAsync();

        return Ok(new { avatarUrl = user.AvatarUrl });
    }
       
    private static string ExtractToken(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // 如果是完整 URL，就把 querystring 裡的 token 拿出來
        if (input.Contains("://", StringComparison.Ordinal))
        {
            try
            {
                var uri = new Uri(input);
                var q = QueryHelpers.ParseQuery(uri.Query);
                if (q.TryGetValue("token", out var v)) return v.ToString();
            }
            catch { /* 忽略，當作不是網址處理 */ }
        }
        // 不是網址就當作已經是 token
        return input.Trim();
    }
    private static string NormalizeToken(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

        var t = raw.Trim().Trim('"', '\'', '<', '>');
        // 移除常見的零寬 / bidi 控制字元
        var bad = new HashSet<char>(new[] {
        '\u200B','\u200C','\u200D','\u200E','\u200F',
        '\u202A','\u202B','\u202C','\u202D','\u202E',
        '\u2066','\u2067','\u2068','\u2069'
    });
        t = new string(t.Where(c => !bad.Contains(c)).ToArray());

        // 若貼的是 token=xxx
        const string prefix = "token=";
        var idx = t.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (idx >= 0) t = t[(idx + prefix.Length)..];

        // 若貼的是完整 URL，從 query 取 token
        if (t.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            Uri.TryCreate(t, UriKind.Absolute, out var uri))
        {
            var q = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            if (q.TryGetValue("token", out var v) && !string.IsNullOrWhiteSpace(v))
                t = v.ToString();
        }
               
        var parts = t.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 3) t = string.Join('.', parts.Take(3));

        return t;
    }
    //  看目前載入的 Reset 設定，確認 ExpireMinutes & Secret 長度
    [AllowAnonymous]
    [HttpGet("reset-password/debug-opts")]
    public IActionResult DebugOpts()
    {
        var o = _resetOpts.Value;
        return Ok(new { o.Issuer, o.Audience, o.ExpireMinutes, SecretLen = (o.Secret ?? "").Length });
    }

    // 驗 token 時把真正的失敗原因吐出來
    [AllowAnonymous]
    [HttpPost("reset-password/debug-validate")]
    public IActionResult DebugValidate([FromBody] ResetPasswordDto dto)
    {
        var raw = NormalizeToken(dto.Token);
        if (string.IsNullOrWhiteSpace(raw)) return BadRequest("缺少 token");

        var opts = _resetOpts.Value;
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };

        JwtSecurityToken t;
        try { t = handler.ReadJwtToken(raw); }
        catch (Exception ex) { return BadRequest("ReadJwtToken: " + ex.Message); }

        if (!TryGetUnixTime(t, "exp", out var expSec))
            return BadRequest("Parse: 缺少 exp");

        var expUtc = DateTimeOffset.FromUnixTimeSeconds(expSec).UtcDateTime;
        var now = UtcNow();
        var skew = TimeSpan.FromMinutes(5);
        var expired = now > expUtc + skew;

        var p = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Secret)),
            ValidateLifetime = false
        };

        try
        {
            var principal = handler.ValidateToken(raw, p, out _);
            return Ok(new
            {
                ok = !expired,
                nowUtc = now,
                expUtc,
                expired,
                purpose = t.Payload.TryGetValue("purpose", out var pv) ? pv : null,
                typ = t.Payload.TryGetValue("typ", out var tv) ? tv : null
            });
        }
        catch (SecurityTokenInvalidSignatureException ex) { return BadRequest("BadSignature: " + ex.Message); }
        catch (Exception ex) { return BadRequest(ex.GetType().Name + ": " + ex.Message); }
    }

    private static bool TryGetUnixTime(JwtSecurityToken jwt, string name, out long seconds)
    {
        seconds = 0;

        // 1) 先從 JwtSecurityToken.Payload 嘗試
        if (jwt.Payload.TryGetValue(name, out var v))
        {
            switch (v)
            {
                case long l: seconds = l; return true;
                case int i: seconds = i; return true;
                case double d: seconds = (long)d; return true;
                case decimal m: seconds = (long)m; return true;
                case string s when long.TryParse(s, out var l2): seconds = l2; return true;
                case JsonElement je:
                    if (je.ValueKind == JsonValueKind.Number)
                    {
                        if (je.TryGetInt64(out var l3)) { seconds = l3; return true; }
                        if (je.TryGetDouble(out var d2)) { seconds = (long)d2; return true; }
                    }
                    else if (je.ValueKind == JsonValueKind.String)
                    {
                        var str = je.GetString();
                        if (long.TryParse(str, out var l4)) { seconds = l4; return true; }
                    }
                    break;
            }
        }

        // 2) 取不到就直接 Base64Url 解 payload 再用 JSON 讀
        try
        {
            var raw = jwt.RawData;                      // 原始 JWT 字串
            if (!string.IsNullOrEmpty(raw))
            {
                var parts = raw.Split('.');
                if (parts.Length >= 2)
                {
                    var bytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(parts[1]);
                    using var doc = JsonDocument.Parse(bytes);
                    if (doc.RootElement.TryGetProperty(name, out var node))
                    {
                        if (node.ValueKind == JsonValueKind.Number)
                        {
                            if (node.TryGetInt64(out var l)) { seconds = l; return true; }
                            if (node.TryGetDouble(out var d)) { seconds = (long)d; return true; }
                        }
                        else if (node.ValueKind == JsonValueKind.String)
                        {
                            var s = node.GetString();
                            if (long.TryParse(s, out var l)) { seconds = l; return true; }
                        }
                    }
                }
            }
        }
        catch
        {
            // 忽略，當沒有 exp 看待
        }

        return false;
    }

    private static DateTime UtcNow() => DateTime.UtcNow;
}