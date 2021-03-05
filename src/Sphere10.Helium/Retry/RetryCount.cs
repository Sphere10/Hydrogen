using System;

namespace Sphere10.Helium.Retry
{
    public record RetryCount
    {
        public int Count { get; }
        public TimeSpan DurationBetweenCount { get; }

        public RetryCount(int count, TimeSpan durationBetweenCount) => (Count, DurationBetweenCount) = (count, durationBetweenCount);
    }
}