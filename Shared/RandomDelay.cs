namespace Shared;

public static class RandomDelay
{
    private static readonly Random _random = new Random(DateTime.Now.Nanosecond);

    public static TimeSpan DelayInSeconds(double minSeconds, double maxSeconds)
    {
        if (minSeconds > maxSeconds)
        {
            throw new ArgumentException($"Min {minSeconds} can't be larger than max {maxSeconds}");
        }

        var randomDelay = (maxSeconds - minSeconds) * _random.NextDouble();
        return TimeSpan.FromSeconds(minSeconds + randomDelay);
    }
}