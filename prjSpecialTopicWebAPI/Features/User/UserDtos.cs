namespace prjSpecialTopicWebAPI.Features.Users;

public record UserListItemDto(Guid Uid, string Phone, string Name, string Email, bool Gender, DateOnly Birthday, string? AvatarUrl, byte? Status, byte? Level);
public record UserDetailDto(Guid Uid, string Phone, string Name, string Email, bool Gender, DateOnly Birthday, string? Address, DateTime? RegisterDate, DateTime? LastLoginDate, string? AvatarUrl, byte? Status, byte? Level);
public record RegisterDto(string Phone, string Password, string Name, string Email, bool Gender, DateOnly Birthday);
public record LoginDto(string Account, string Password); // Account 可塞 Phone 或 Email
