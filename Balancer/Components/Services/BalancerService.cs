using Microsoft.Extensions.Logging;

namespace Balancer.Components.Services
{
    internal class BalancerService
    {
        private readonly ILogger<BalancerService> _logger;

        public BalancerService(ILogger<BalancerService> logger)
        {
            _logger = logger;
        }
        public bool BalanceTotal(decimal cash, decimal check, decimal total)
        {
            if ((cash + check) != total)
            {
                _logger.LogInformation("Total is NOT balanced");
                return false;
            }
            else
            {
                _logger.LogInformation("Total is balanced");
                return true;
            }

        }
    }
}
