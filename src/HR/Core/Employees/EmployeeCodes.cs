namespace HR.Core.Employees;

public static class EmployeeStatuses
{
    public const string Active = "ACTIVE";
    public const string Locked = "LOCKED";
    public const string Resigned = "RESIGNED";
}

public static class Genders
{
    public const string Male = "Male";
    public const string Female = "Female";
    public const string Other = "Other";

    public static bool TryNormalize(string? value, out string gender)
    {
        gender = string.Empty;
        var text = (value ?? string.Empty).Trim();
        if (text.Equals(Male, StringComparison.OrdinalIgnoreCase) || text.Equals("Nam", StringComparison.OrdinalIgnoreCase))
        {
            gender = Male;
            return true;
        }

        if (text.Equals(Female, StringComparison.OrdinalIgnoreCase)
            || text.Equals("Nữ", StringComparison.OrdinalIgnoreCase)
            || text.Equals("Nu", StringComparison.OrdinalIgnoreCase))
        {
            gender = Female;
            return true;
        }

        if (text.Equals(Other, StringComparison.OrdinalIgnoreCase)
            || text.Equals("Khác", StringComparison.OrdinalIgnoreCase)
            || text.Equals("Khac", StringComparison.OrdinalIgnoreCase))
        {
            gender = Other;
            return true;
        }

        return false;
    }
}
