namespace prjSpecialTopicWebAPI.Features.Users;

public record UserListItemDto(Guid Uid, string Phone, string Name, string Email, bool Gender, DateOnly Birthday, string? AvatarUrl, byte? Status, byte? Level);
public record UserDetailDto(Guid Uid, string Phone, string Name, string Email, bool Gender, DateOnly Birthday, string? Address, DateTime? RegisterDate, DateTime? LastLoginDate, string? AvatarUrl, byte? Status, byte? Level);

public record RegisterDto(string Phone, string Password, string Name, string Email, bool Gender, DateOnly Birthday, string? Address);
public record LoginDto(string Account, string Password); // Account 可塞 Phone 或 Email
public record UserUpdateDto(string? Name, string? Address, DateOnly? Birthday);

public record ChangePasswordDto(string OldPassword, string NewPassword, string ConfirmPassword);

/// <summary>送出忘記密碼請求（可填手機或 Email 任一）</summary>
public record ForgotPasswordDto(string AccountOrEmail);

/// <summary>重設密碼（前端帶回 token + 新密碼）</summary>
public record ResetPasswordDto(string Token, string NewPassword, string ConfirmPassword);
