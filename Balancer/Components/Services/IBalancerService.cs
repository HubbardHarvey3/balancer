
namespace Balancer.Components.Services
{
    internal interface IBalancerService
    {
        public bool BalanceTotal(decimal cash, decimal check, decimal total);
    }
}
