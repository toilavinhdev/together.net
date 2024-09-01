namespace Infrastructure.SharedKernel.Extensions;

public static class DateTimeExtensions
{
    public static DateTimeOffset StartOfDayUtc(this DateTimeOffset input)
    {
        input = input.ToUniversalTime();
        return new DateTimeOffset(input.Year, input.Month, input.Day, 0, 0, 0, TimeSpan.Zero);
    }

    public static DateTimeOffset EndOfDayUtc(this DateTimeOffset input)
    {
        return input.StartOfDayUtc().AddDays(1).AddTicks(-1);
    }
    
    public static IEnumerable<DateTimeOffset> GetDaysInRange(DateTimeOffset start, DateTimeOffset end)
    {
        var range = (int)(end.Date - start.Date).TotalDays + 1;

        return Enumerable.Range(0, range).Select(next => start.AddDays(next));
    }
}