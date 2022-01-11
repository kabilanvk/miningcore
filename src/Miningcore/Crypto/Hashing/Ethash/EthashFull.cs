using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Miningcore.Blockchain.Ethereum;
using Miningcore.Contracts;
using NLog;

namespace Miningcore.Crypto.Hashing.Ethash
{
    public class EthashFull : IDisposable
    {
        public EthashFull(int numCaches, string dagDir, ILogger logger)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(dagDir), $"{nameof(dagDir)} must not be empty");

            this.numCaches = numCaches;
            this.dagDir = dagDir;
            log = logger;

            CleanupDag();
        }

        private ILogger log;
        private int numCaches; // Maximum number of caches to keep before eviction (only init, don't modify)
        private readonly object cacheLock = new object();
        private readonly Dictionary<ulong, Dag> caches = new Dictionary<ulong, Dag>();
        private Dag future;
        private readonly string dagDir;

        public void Dispose()
        {
            foreach(var value in caches.Values)
                value.Dispose();
        }

        public async Task<Dag> GetDagAsync(ulong block, ILogger logger, CancellationToken ct)
        {
            var epoch = block / EthereumConstants.EpochLength;
            Dag result;

            lock(cacheLock)
            {
                if(numCaches == 0)
                    numCaches = 3;

                if(!caches.TryGetValue(epoch, out result))
                {
                    // No cached DAG, evict the oldest if the cache limit was reached
                    while(caches.Count >= numCaches)
                    {
                        var toEvict = caches.Values.OrderBy(x => x.LastUsed).First();
                        var key = caches.First(pair => pair.Value == toEvict).Key;
                        var epochToEvict = toEvict.Epoch;

                        logger.Info(() => $"Evicting DAG for epoch {epochToEvict} in favour of epoch {epoch}");
                        toEvict.Dispose();
                        caches.Remove(key);
                    }

                    // If we have the new DAG pre-generated, use that, otherwise create a new one
                    if(future != null && future.Epoch == epoch)
                    {
                        logger.Info(() => $"Using pre-generated DAG for epoch {epoch}");

                        result = future;
                        future = null;
                    }

                    else
                    {
                        logger.Info(() => $"No pre-generated DAG available, creating new for epoch {epoch}");
                        result = new Dag(epoch);
                    }

                    caches[epoch] = result;
                }

                // If we used up the future cache, or need a refresh, regenerate
                else if(future == null || future.Epoch <= epoch)
                {
                    logger.Info(() => $"Pre-generating DAG for epoch {epoch + 1}");
                    future = new Dag(epoch + 1);

#pragma warning disable 4014
                    future.GenerateAsync(dagDir, logger, ct);
#pragma warning restore 4014
                }

                result.LastUsed = DateTime.UtcNow;
            }

            await result.GenerateAsync(dagDir, logger, ct);

            return result;
        }

        private void CleanupDag()
        {
            // Cleanup old Dag files asynchronously
            _ = Task.Run(() =>
            {
                try
                {
                    log.Info($"Cleaning up old DAG files in {dagDir}");

                    var dagFiles = Directory.GetFiles(dagDir);

                    if(dagFiles.Length > numCaches)
                    {
                        log.Info($"There are {dagFiles.Length} DAG files, and {dagFiles.Length - numCaches} will be deleted");

                        // Comparing f2 to f1 (instead of f1 to f2) will sort the filenames in descending order by creation time
                        // i.e., newer files will be first, older files after
                        Array.Sort(dagFiles, Comparer<string>.Create((f1, f2) => File.GetCreationTimeUtc(f2).CompareTo(File.GetCreationTimeUtc(f1))));

                        for(var i = numCaches; i < dagFiles.Length; i++)
                        {
                            log.Debug($"Deleting {dagFiles[i]}");
                            File.Delete(dagFiles[i]);
                            log.Info($"Deleted {dagFiles[i]}");
                        }
                    }

                    log.Info($"Finished cleaning up old DAG files in {dagDir}");
                }
                catch(Exception e)
                {
                    log.Error(() => $"Exception while cleaning up old Dag files in {dagDir}: {e}");
                }
            });
        }
    }
}
