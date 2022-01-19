namespace Miningcore.Api.Requests
{
    public class ResetBalanceRequest
    {
        public string PoolId { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
    }
}
