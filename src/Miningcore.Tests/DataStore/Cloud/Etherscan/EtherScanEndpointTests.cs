using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Metadata;
using Miningcore.Configuration;
using Miningcore.DataStore.Cloud.EtherScan;
using Miningcore.Payments;
using Xunit;

namespace Miningcore.Tests.DataStore.Cloud.EtherScan
{
    public class EtherScanEndpointTests : TestBase
    {
        private EtherScanEndpoint etherScanEndpoint;
        private readonly ClusterConfig clusterConfig;
        private readonly PoolConfig poolConfig;

        public EtherScanEndpointTests()
        {
            var ctx = ModuleInitializer.Container;
            var handlerImpl = ctx.Resolve<IEnumerable<Meta<Lazy<IPayoutHandler, CoinFamilyAttribute>>>>()
                .First(x => x.Value.Metadata.SupportedFamilies.Contains(CoinFamily.Ethereum)).Value;

            clusterConfig = PoolCore.Pool.clusterConfig;
            poolConfig = clusterConfig.Pools.First(c => c.Coin.Equals("ethereum", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetDailyAverageBlockTime_Successful()
        {
            clusterConfig.Pools[0].EtherScan.ApiKey = "";
            clusterConfig.Pools[0].EtherScan.ApiUrl = "https://api.etherscan.io/api";
            etherScanEndpoint = new EtherScanEndpoint(clusterConfig);

            var res = await etherScanEndpoint.GetDailyAverageBlockTime(10, "0xbd5CDD2B3D04Ce42605AAEa7E8355ac0e0a12710");
            Assert.NotNull(res);
            Assert.True(res > 0);
        }

    }
}
