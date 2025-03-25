using Microsoft.VisualStudio.TestTools.UnitTesting;
using TapMangoChallenge.Services;

namespace TapMangoChallenge.Tests
{
    [TestClass]
    public class RateLimiterServiceTests
    {
        [TestMethod]
        public void AllowsMessagesWithinPerNumberLimit()
        {
            var metrics = new MetricsService();
            var service = new RateLimiterService(maxPerNumber: 3, maxGlobal: 10, metricsService: metrics);
            string phone = "123456";

            // First three messages should be allowed.
            Assert.IsTrue(service.CanSend(phone));
            Assert.IsTrue(service.CanSend(phone));
            Assert.IsTrue(service.CanSend(phone));
        }

        [TestMethod]
        public void BlocksMessageWhenPerNumberLimitExceeded()
        {
            var metrics = new MetricsService();
            var service = new RateLimiterService(maxPerNumber: 2, maxGlobal: 10, metricsService: metrics);
            string phone = "123456";

            // First two messages allowed, third should be blocked.
            Assert.IsTrue(service.CanSend(phone));
            Assert.IsTrue(service.CanSend(phone));
            Assert.IsFalse(service.CanSend(phone));
        }

        [TestMethod]
        public void BlocksMessageWhenGlobalLimitExceeded()
        {
            var metrics = new MetricsService();
            var service = new RateLimiterService(maxPerNumber: 10, maxGlobal: 3, metricsService: metrics);
            
            // Using different phone numbers, global limit is 3 messages per second.
            Assert.IsTrue(service.CanSend("111"));
            Assert.IsTrue(service.CanSend("222"));
            Assert.IsTrue(service.CanSend("333"));
            Assert.IsFalse(service.CanSend("444"));
        }

        [TestMethod]
        public void ResetsAfterWindowExpires()
        {
            var metrics = new MetricsService();
            var service = new RateLimiterService(maxPerNumber: 2, maxGlobal: 10, metricsService: metrics);
            string phone = "123456";

            // Use up the per-number limit.
            Assert.IsTrue(service.CanSend(phone));
            Assert.IsTrue(service.CanSend(phone));
            Assert.IsFalse(service.CanSend(phone));

            // Wait for longer than the 1-second sliding window.
            Thread.Sleep(1100);

            // Should allow again after the window expires.
            Assert.IsTrue(service.CanSend(phone));
        }
    }
}
