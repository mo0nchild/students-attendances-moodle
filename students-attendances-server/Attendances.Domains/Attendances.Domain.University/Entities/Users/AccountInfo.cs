using Attendances.Domain.Core.Models;

namespace Attendances.Domain.University.Entities.Users;

public class AccountInfo : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public AccountRole Role { get; set; } = default;
    public string? RefreshToken { get; set; }
    
    public virtual UserInfo? User { get; set; } = null;
}
public enum AccountRole
{
    Teacher,
    Admin,
}