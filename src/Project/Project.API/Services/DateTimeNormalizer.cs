namespace Project.API.Services;

internal static class DateTimeNormalizer
{
    public static DateTime ToUtc(DateTime value) =>
        DateTime.SpecifyKind(value.Date, DateTimeKind.Utc);
}
