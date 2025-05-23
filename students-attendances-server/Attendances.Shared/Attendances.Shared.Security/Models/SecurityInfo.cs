namespace Attendances.Shared.Security.Models;

public static class SecurityInfo
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Teacher = "Teacher";
}

public enum SecurityRole
{
    User,
    Teacher,
    Admin,
}