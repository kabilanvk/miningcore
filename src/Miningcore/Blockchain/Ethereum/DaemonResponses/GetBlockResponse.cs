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

using System.Numerics;
using Miningcore.Serialization;
using Newtonsoft.Json;

namespace Miningcore.Blockchain.Ethereum.DaemonResponses
{
    public class Transaction
    {
        /// <summary>
        /// 32 Bytes - hash of the transaction.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// the number of transactions made by the sender prior to this one.
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong>))]
        public ulong Nonce { get; set; }

        /// <summary>
        /// 32 Bytes - hash of the block where this transaction was in. null when its pending.
        /// </summary>
        public string BlockHash { get; set; }

        /// <summary>
        /// block number where this transaction was in. null when its pending.
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong?>))]
        public ulong? BlockNumber { get; set; }

        /// <summary>
        /// integer of the transactions index position in the block. null when its pending.
        /// </summary>
        [JsonProperty("transactionIndex")]
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong?>))]
        public ulong? Index { get; set; }

        /// <summary>
        /// address of the sender.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// address of the receiver. null when its a contract creation transaction.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Value transferred in Wei
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<BigInteger>))]
        public BigInteger Value { get; set; }

        /// <summary>
        /// gas price provided by the sender in Wei.
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<BigInteger>))]
        public BigInteger GasPrice { get; set; }

        /// <summary>
        /// gas provided by the sender
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<BigInteger>))]
        public BigInteger Gas { get; set; }

        /// <summary>
        /// the data send along with the transaction.
        /// </summary>
        public string Input { get; set; }
    }

    public class Block
    {
        /// <summary>
        /// The block number. null when its pending block.
        /// </summary>
        [JsonProperty("number")]
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong?>))]
        public ulong? Height { get; set; }

        /// <summary>
        /// 32 Bytes - hash of the block. null when its pending block.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// 32 Bytes - hash of the parent block.
        /// </summary>
        public string ParentHash { get; set; }

        /// <summary>
        /// x Bytes - hash.
        /// </summary>
        public string MixHash { get; set; }

        /// <summary>
        /// 8 Bytes - hash of the generated proof-of-work. null when its pending block.
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// An array containing all engine specific fields
        /// https://github.com/ethereum/EIPs/issues/95
        /// </summary>
        public string[] SealFields { get; set; }

        /// <summary>
        /// 32 Bytes - SHA3 of the uncles data in the block.
        /// </summary>
        public string Sha3Uncles { get; set; }

        /// <summary>
        /// 256 Bytes - the bloom filter for the logs of the block. null when its pending block.
        /// </summary>
        public string LogsBloom { get; set; }

        /// <summary>
        /// 32 Bytes - the root of the transaction trie of the block.
        /// </summary>
        public string TransactionsRoot { get; set; }

        /// <summary>
        /// 32 Bytes - the root of the final state trie of the block.
        /// </summary>
        public string StateRoot { get; set; }

        /// <summary>
        /// 32 Bytes - the root of the receipts trie of the block.
        /// </summary>
        public string ReceiptsRoot { get; set; }

        /// <summary>
        /// 20 Bytes - the address of the beneficiary to whom the mining rewards were given.
        /// </summary>
        public string Miner { get; set; }

        /// <summary>
        /// integer of the difficulty for this block
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// integer of the total difficulty of the chain until this block.
        /// </summary>
        public string TotalDifficulty { get; set; }

        /// <summary>
        /// the "extra data" field of this block
        /// </summary>
        public string ExtraData { get; set; }

        /// <summary>
        /// integer the size of this block in bytes
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong>))]
        public ulong Size { get; set; }

        /// <summary>
        /// This property will contain the value of the base fee for any block after the EIP-1559 fork
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong>))]
        public ulong BaseFeePerGas { get; set; }

        /// <summary>
        /// the maximum gas allowed in this block
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong>))]
        public ulong GasLimit { get; set; }

        /// <summary>
        /// the total used gas by all transactions in this block.
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong>))]
        public ulong GasUsed { get; set; }

        /// <summary>
        /// the unix timestamp for when the block was collated.
        /// </summary>
        [JsonConverter(typeof(HexToIntegralTypeJsonConverter<ulong>))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Array of transaction objects, or 32 Bytes transaction hashes depending on the last given parameter.
        /// </summary>
        public Transaction[] Transactions { get; set; }

        /// <summary>
        /// Array of uncle hashes.
        /// </summary>
        public string[] Uncles { get; set; }
    }
}
