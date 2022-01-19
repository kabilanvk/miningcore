using Miningcore.Persistence.Model;

namespace Miningcore.Api.Responses
{
    public class ResetBalanceResponse
    {
        public Balance OldBalance { get; set; }
        public Balance NewBalance { get; set; }
    }
}
