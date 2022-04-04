using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Miningcore.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using ZeroMQ;

namespace Miningcore.DaemonInterface
{
    public interface IDaemonClient
    {
        void Configure(DaemonEndpointConfig[] endPoints, string digestAuthRealm = null);

        /// <summary>
        /// Executes the request against all configured demons and returns their responses as an array
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<DaemonResponse<JToken>[]> ExecuteCmdAllAsync(ILogger logger, string method);

        /// <summary>
        /// Executes the request against all configured demons and returns their responses as an array
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task<DaemonResponse<TResponse>[]> ExecuteCmdAllAsync<TResponse>(ILogger logger, string method,
            object payload = null, JsonSerializerSettings payloadJsonSerializerSettings = null)
            where TResponse : class;

        /// <summary>
        /// Executes the request against all configured demons and returns the first successful response
        /// </summary>
        /// <returns></returns>
        Task<DaemonResponse<JToken>> ExecuteCmdAnyAsync(ILogger logger, string method, bool throwOnError = false);

        /// <summary>
        /// Executes the request against all configured demons and returns the first successful response
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        Task<DaemonResponse<TResponse>> ExecuteCmdAnyAsync<TResponse>(ILogger logger, string method, object payload = null,
            JsonSerializerSettings payloadJsonSerializerSettings = null, bool throwOnError = false)
            where TResponse : class;

        /// <summary>
        /// Executes the request against all configured demons and returns the first successful response
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        Task<DaemonResponse<TResponse>> ExecuteCmdAnyAsync<TResponse>(ILogger logger, CancellationToken ct, string method, object payload = null,
            JsonSerializerSettings payloadJsonSerializerSettings = null, bool throwOnError = false)
            where TResponse : class;

        /// <summary>
        /// Executes the request against all configured demons and returns the first successful response
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<DaemonResponse<JToken>> ExecuteCmdSingleAsync(ILogger logger, string method);

        /// <summary>
        /// Executes the request against all configured demons and returns the first successful response
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task<DaemonResponse<TResponse>> ExecuteCmdSingleAsync<TResponse>(ILogger logger, string method, object payload = null, JsonSerializerSettings payloadJsonSerializerSettings = null)
            where TResponse : class;

        /// <summary>
        /// Executes the request against all configured demons and returns the first successful response
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task<DaemonResponse<TResponse>> ExecuteCmdSingleAsync<TResponse>(ILogger logger, CancellationToken ct, string method, object payload = null,
            JsonSerializerSettings payloadJsonSerializerSettings = null)
            where TResponse : class;

        /// <summary>
        /// Executes the requests against all configured demons and returns the first successful response array
        /// </summary>
        /// <returns></returns>
        Task<DaemonResponse<JToken>[]> ExecuteBatchAnyAsync(ILogger logger, params DaemonCmd[] batch);

        IObservable<byte[]> WebsocketSubscribe(ILogger logger, Dictionary<DaemonEndpointConfig,
            (int Port, string HttpPath, bool Ssl)> portMap, string method, object payload = null, JsonSerializerSettings payloadJsonSerializerSettings = null);

        IObservable<ZMessage> ZmqSubscribe(ILogger logger, Dictionary<DaemonEndpointConfig, (string Socket, string Topic)> portMap);

        IObservable<string[]> NotifyWorkSubscribe(ILogger logger, Dictionary<DaemonEndpointConfig, string> portMap);

    }
}
