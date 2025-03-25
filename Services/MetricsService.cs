using System.Collections.Concurrent;
using TapMangoChallenge.Models;

namespace TapMangoChallenge.Services
{
    public class MetricsService
    {
        private readonly ConcurrentQueue<MetricEvent> _events = new();
        private readonly TimeSpan _retention = TimeSpan.FromMinutes(5);

        public void AddEvent(string phoneNumber, DateTime timestamp)
        {
            _events.Enqueue(new MetricEvent { PhoneNumber = phoneNumber, Timestamp = timestamp });
            CleanupOldEvents(timestamp);
        }

        private void CleanupOldEvents(DateTime now)
        {
            while (_events.TryPeek(out var evt) && (now - evt.Timestamp) > _retention)
            {
                _events.TryDequeue(out _);
            }
        }

        public IEnumerable<MetricEvent> GetEvents(DateTime? start = null, DateTime? end = null, string phoneNumber = null)
        {
            var now = DateTime.UtcNow;
            start ??= now - _retention;
            end ??= now;

            return _events.Where(evt =>
                evt.Timestamp >= start && evt.Timestamp <= end &&
                (phoneNumber == null || evt.PhoneNumber == phoneNumber)
            );
        }

        public double GetGlobalRate(DateTime? start = null, DateTime? end = null)
        {
            var events = GetEvents(start, end);
            var count = events.Count();
            var seconds = ((end ?? DateTime.UtcNow) - (start ?? DateTime.UtcNow.AddMinutes(-5))).TotalSeconds;
            return seconds > 0 ? count / seconds : 0;
        }

        public Dictionary<string, double> GetPerNumberRate(DateTime? start = null, DateTime? end = null)
        {
            var events = GetEvents(start, end);
            var grouped = events.GroupBy(evt => evt.PhoneNumber)
                .ToDictionary(g => g.Key, g =>
                {
                    var seconds = ((end ?? DateTime.UtcNow) - (start ?? DateTime.UtcNow.AddMinutes(-5))).TotalSeconds;
                    return seconds > 0 ? g.Count() / seconds : 0;
                });
            return grouped;
        }
    }
}
