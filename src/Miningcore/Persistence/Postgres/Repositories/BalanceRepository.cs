/*
Copyright 2017 Coin Foundry (coinfoundry.org)
Authors: Oliver Weichhold (oliver@weichhold.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Miningcore.Extensions;
using Miningcore.Persistence.Model;
using Miningcore.Persistence.Repositories;
using NLog;

namespace Miningcore.Persistence.Postgres.Repositories
{
    public class BalanceRepository : IBalanceRepository
    {
        public BalanceRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        private readonly IMapper mapper;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public async Task<int> AddAmountAsync(IDbConnection con, IDbTransaction tx, string poolId, string address, decimal amount, string usage)
        {
            return await AddAmountAsyncDeductingTxFee(con, tx, poolId, address, amount, usage, 0, 0);
        }

        public async Task<int> AddAmountAsyncDeductingTxFee(IDbConnection con, IDbTransaction tx, string poolId, string address, decimal amount, string usage, decimal deduction, decimal threshold)
        {
            logger.LogInvoke();

            var now = DateTime.UtcNow;

            // update balance
            var query = "SELECT * FROM balances WHERE poolid = @poolId AND address = @address";

            var balance = (await con.QueryAsync<Entities.Balance>(query, new { poolId, address }, tx))
                .FirstOrDefault();

            if(balance == null)
            {
                if(deduction > 0 && deduction < amount)
                {
                    amount -= deduction;
                }


                balance = new Entities.Balance
                {
                    PoolId = poolId,
                    Created = now,
                    Address = address,
                    Amount = amount,
                    Updated = now
                };

                query = "INSERT INTO balances(poolid, address, amount, created, updated) " +
                    "VALUES(@poolid, @address, @amount, @created, @updated)";

                return await con.ExecuteAsync(query, balance, tx);
            }
            else
            {
                if(deduction > 0 && deduction < amount && balance.Amount < threshold)
                {
                    amount -= deduction;
                }

                query = "UPDATE balances SET amount = amount + @amount, updated = now() at time zone 'utc' " +
                    "WHERE poolid = @poolId AND address = @address";

                return await con.ExecuteAsync(query, new
                {
                    poolId,
                    address,
                    amount
                }, tx);
            }
        }

        public async Task<decimal> GetBalanceAsync(IDbConnection con, IDbTransaction tx, string poolId, string address)
        {
            logger.LogInvoke();

            const string query = "SELECT amount FROM balances WHERE poolid = @poolId AND address = @address";

            return await con.QuerySingleOrDefaultAsync<decimal>(query, new { poolId, address }, tx);
        }

        public async Task<decimal> GetBalanceAsync(IDbConnection con, string poolId, string address)
        {
            logger.LogInvoke();

            const string query = "SELECT amount FROM balances WHERE poolid = @poolId AND address = @address";

            return await con.QuerySingleOrDefaultAsync<decimal>(query, new { poolId, address });
        }

        public async Task<Balance> GetMinerBalanceAsync(IDbConnection con, string poolId, string address)
        {
            logger.LogInvoke();

            const string query = "SELECT b.poolid, b.address, b.amount, b.created, b.updated, p.created AS paiddate FROM balances AS b " +
                                 "LEFT JOIN payments AS p ON  p.address = b.address AND p.poolid = b.poolid " +
                                 "WHERE b.poolid = @poolId AND b.address = @address ORDER BY p.created DESC LIMIT 1";

            return (await con.QueryAsync<Entities.Balance>(query, new { poolId, address }))
                .Select(mapper.Map<Balance>)
                .FirstOrDefault();
        }

        public async Task<Balance[]> GetPoolBalancesOverThresholdAsync(IDbConnection con, string poolId, decimal minimum)
        {
            logger.LogInvoke();

            const string query = "SELECT b.poolid, b.address, b.amount, b.created, b.updated, MAX(p.created) AS paiddate FROM balances AS b " +
                                 "LEFT JOIN payments AS p ON  p.address = b.address AND p.poolid = b.poolid " +
                                 "WHERE b.poolid = @poolId AND b.amount >= @minimum " +
                                 "GROUP BY b.poolid, b.address, b.amount, b.created, b.updated";

            return (await con.QueryAsync<Entities.Balance>(query, new { poolId, minimum }))
                .Select(mapper.Map<Balance>)
                .ToArray();
        }

        public async Task<decimal> GetTotalBalanceSum(IDbConnection con, string poolId)
        {
            logger.LogInvoke();

            const string query = "SELECT sum(amount) FROM balances WHERE poolid = @poolId";

            return await con.QuerySingleOrDefaultAsync<decimal>(query, new { poolId });
        }
    }
}
