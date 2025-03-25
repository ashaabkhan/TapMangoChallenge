using System.Collections.Concurrent;

namespace TapMangoChallenge.Services
{
    public class RateLimiterService
    {
        private readonly int _maxPerNumber;
        private readonly int _maxGlobal;
        private readonly TimeSpan _window = TimeSpan.FromSeconds(1);
        private readonly ConcurrentDictionary<string, Queue<DateTime>> _perNumberTracker = new();
        private readonly Queue<DateTime> _globalTracker = new();
        private readonly MetricsService _metricsService;

        public RateLimiterService(int maxPerNumber, int maxGlobal, MetricsService metricsService)
        {
            _maxPerNumber = maxPerNumber;
            _maxGlobal = maxGlobal;
            _metricsService = metricsService;

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    CleanupOldEntries();
                    await Task.Delay(30_000);
                }
            });
        }

        /// <summary>
        /// Checks if a message can be sent from the given phone number.
        /// If allowed, records the event.
        /// </summary>
        public bool CanSend(string phoneNumber)
        {
            var now = DateTime.UtcNow;

            CleanupOldTimestamps(_globalTracker, now);
            if (_globalTracker.Count >= _maxGlobal) return false;

            var numberQueue = _perNumberTracker.GetOrAdd(phoneNumber, _ => new Queue<DateTime>());
            lock (numberQueue)
            {
                CleanupOldTimestamps(numberQueue, now);
                if (numberQueue.Count >= _maxPerNumber) return false;

                numberQueue.Enqueue(now);
                _globalTracker.Enqueue(now);
            }

            _metricsService.AddEvent(phoneNumber, now);

            return true;
        }

        private void CleanupOldTimestamps(Queue<DateTime> queue, DateTime now)
        {
            while (queue.Count > 0 && (now - queue.Peek()) > _window)
                queue.Dequeue();
        }

        private void CleanupOldEntries()
        {
            var now = DateTime.UtcNow;

            foreach (var key in _perNumberTracker.Keys)
            {
                if (!_perNumberTracker.TryGetValue(key, out var queue)) continue;
                lock (queue)
                {
                    CleanupOldTimestamps(queue, now);
                    if (queue.Count == 0)
                        _perNumberTracker.TryRemove(key, out _);
                }
            }

            CleanupOldTimestamps(_globalTracker, now);
        }
    }
}
