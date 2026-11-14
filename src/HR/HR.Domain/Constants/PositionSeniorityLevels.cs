namespace HR.Domain.Constants;

public static class PositionSeniorityLevels
{
    public const string Staff = "STAFF";
    public const string Manager = "MANAGER";
    public const string Director = "DIRECTOR";
    public const string Executive = "EXECUTIVE";

    public static readonly string[] All = [Staff, Manager, Director, Executive];

    public static bool IsValid(string? value) => All.Contains(value, StringComparer.Ordinal);

    public static bool IsEligibleForPartTime(string? value) => value == Staff;
}
