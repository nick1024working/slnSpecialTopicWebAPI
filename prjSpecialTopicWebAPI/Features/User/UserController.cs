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

namespace prjSpecialTopicWebAPI.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly TeamAProjectContext _db;
    private readonly IConfiguration _config;
    public UsersController(TeamAProjectContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // 讀取列表（受 JWT 保護）
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> GetList([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.Users.AsQueryable();
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
        if (u is null) return NotFound();
        return Ok(new UserDetailDto(u.Uid, u.Phone, u.Name, u.Email, u.Gender, u.Birthday, u.Address, u.RegisterDate, u.LastLoginDate, u.AvatarUrl, u.Status, u.Level));
    }

    // 註冊（密碼雜湊）
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDetailDto>> Register([FromBody] RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(x => x.Phone == dto.Phone || x.Email == dto.Email))
            return Conflict("Phone 或 Email 已存在");

        var user = new User
        {
            Uid = Guid.NewGuid(),
            Phone = dto.Phone.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // 雜湊存入
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim(),
            Gender = dto.Gender,
            Birthday = dto.Birthday,
            RegisterDate = DateTime.UtcNow,
            Status = 1,
            Level = 0
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new UserDetailDto(user.Uid, user.Phone, user.Name, user.Email, user.Gender, user.Birthday, user.Address, user.RegisterDate, user.LastLoginDate, user.AvatarUrl, user.Status, user.Level));
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

        if (user is null) return Unauthorized("帳號或密碼錯誤");

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
            .Where(x => x.Uid == uid)
            .Select(x => new
            {
                x.Uid,
                x.Name,
                x.Email,
                x.Phone,
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
}