using Balancer.Components.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Balancer.Tests
{
    public class BalancerServiceTests
    {
        private readonly BalancerService _balancerService;
        private readonly Mock<ILogger<BalancerService>> _mockLogger;

        public BalancerServiceTests() 
        {
            _mockLogger = new Mock<ILogger<BalancerService>>();
            _balancerService = new BalancerService(_mockLogger.Object);
        }

        [Fact]
        public void BalanceTotal_TotalShouldMatch()
        {
            decimal cash = 111m;
            decimal check = 222m;
            decimal total = 333m;

            bool testResult = _balancerService.BalanceTotal(cash, check, total);

            Assert.True(testResult);
        }


        [Fact]
        public void BalanceTotal_TotalShouldNotMatch()
        {
            decimal cash = 111m;
            decimal check = 222m;
            decimal total = 444m;
            
            bool testResult = _balancerService.BalanceTotal(cash, check, total);

            Assert.False(testResult);
        }
    }
}
