using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Bitfinex.Net.Enums;
using System.Threading;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Clients.SpotApi;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexSocketClient" />
    public class BitfinexSocketClient : BaseSocketClient, IBitfinexSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexSocketClientSpotStreams SpotStreams { get; }

        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of BitfinexSocketClient using the default options
        /// </summary>
        public BitfinexSocketClient() : this(BitfinexSocketClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexSocketClient(BitfinexSocketClientOptions options) : base("Bitfinex", options)
        {
            if (options == null)
                throw new ArgumentException("Cant pass null options, use empty constructor for default");

            ContinueOnQueryResponse = true;
            UnhandledMessageExpected = true;

            SpotStreams = AddApiClient(new BitfinexSocketClientSpotStreams(log, this, options));

            AddGenericHandler("HB", (messageEvent) => { });
            AddGenericHandler("Info", InfoHandler);
            AddGenericHandler("Conf", ConfHandler);
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(BitfinexSocketClientOptions options)
        {
            BitfinexSocketClientOptions.Default = options;
        }

        #region private methods

        private void ConfHandler(MessageEvent messageEvent)
        {
            var confEvent = messageEvent.JsonData.Type == JTokenType.Object && messageEvent.JsonData["event"]?.ToString() == "conf";
            if (!confEvent)
                return;

            // Could check conf result;
        }

        private void InfoHandler(MessageEvent messageEvent)
        {
            var infoEvent = messageEvent.JsonData.Type == JTokenType.Object && messageEvent.JsonData["event"]?.ToString() == "info";
            if (!infoEvent)
                return;

            log.Write(LogLevel.Debug, $"Socket {messageEvent.Connection.Socket.Id} Info event received: {messageEvent.JsonData}");
            if (messageEvent.JsonData["code"] == null)
            {
                // welcome event, send a config message for receiving checsum updates for order book subscriptions
                messageEvent.Connection.Send(new BitfinexSocketConfig { Event = "conf", Flags = 131072 });
                return;
            }

            var code = messageEvent.JsonData["code"]?.Value<int>();
            switch (code)
            {
                case 20051:
                    log.Write(LogLevel.Information, $"Socket {messageEvent.Connection.Socket.Id} Code {code} received, reconnecting socket");
                    messageEvent.Connection.PausedActivity = true; // Prevent new operations to be send
                    messageEvent.Connection.Socket.CloseAsync();
                    break;
                case 20060:
                    log.Write(LogLevel.Information, $"Socket {messageEvent.Connection.Socket.Id} Code {code} received, entering maintenance mode");
                    messageEvent.Connection.PausedActivity = true;
                    break;
                case 20061:
                    log.Write(LogLevel.Information, $"Socket {messageEvent.Connection.Socket.Id} Code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
                    messageEvent.Connection.Socket.CloseAsync(); // Closing it via socket will automatically reconnect
                    break;
                default:
                    log.Write(LogLevel.Warning, $"Socket {messageEvent.Connection.Socket.Id} Unknown info code received: {code}");
                    break;
            }
        }
        internal Task<CallResult<UpdateSubscription>> SubscribeInternalAsync<T>(SocketApiClient apiClient, object? request, string? identifier, bool authenticated, Action<DataEvent<T>> dataHandler, CancellationToken ct)
        {
            return SubscribeAsync(apiClient, request, identifier, authenticated, dataHandler, ct);
        }

        internal Task<CallResult<T>> QueryInternalAsync<T>(SocketApiClient apiClient, object request, bool authenticated)
            => QueryAsync<T>(apiClient, request, authenticated);

        internal CallResult<T> DeserializeInternal<T>(JToken obj, JsonSerializer? serializer = null, int? requestId = null)
            => Deserialize<T>(obj, serializer, requestId);

        /// <inheritdoc />
        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
        {
            if(subscription.Request == null)
            {
                // If we don't have a request object we can't unsubscribe it. Probably is an auth subscription which gets pushed regardless
                // Just returning true here will remove the handler and close the socket if there are no other handlers left on the socket, which is the best we can do
                return true;
            }

            var channelId = ((BitfinexSubscriptionRequest) subscription.Request!).ChannelId;
            var unsub = new BitfinexUnsubscribeRequest(channelId);
            var result = false;
            await connection.SendAndWaitAsync(unsub, ClientOptions.SocketResponseTimeout, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var evnt = data["event"]?.ToString();
                var channel = data["chanId"]?.ToString();
                if (evnt == null || channel == null)
                    return false;

                if (!int.TryParse(channel, out var chan))
                    return false;

                result = evnt == "unsubscribed" && channelId == chan;
                return result;
            }).ConfigureAwait(false);
            return result;
        }

        private static BitfinexAuthentication GetAuthObject(SocketApiClient apiClient, params string[] filter)
        {
            var n = ((BitfinexAuthenticationProvider)apiClient.AuthenticationProvider!).GetNonce().ToString();
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = apiClient.AuthenticationProvider!.Credentials.Key!.GetString(),
                Nonce = n,
                Payload = "AUTH" + n
            };
            if (filter.Any())
                authentication.Filter = filter;
            authentication.Signature = apiClient.AuthenticationProvider.Sign(authentication.Payload).ToLower(CultureInfo.InvariantCulture);
            return authentication;
        }

        #endregion

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            if (s.ApiClient.AuthenticationProvider == null)
                return new CallResult<bool>(new NoApiCredentialsError());

            var authObject = BitfinexSocketClient.GetAuthObject(s.ApiClient);
            var result = new CallResult<bool>(new ServerError("No response from server"));
            await s.SendAndWaitAsync(authObject, ClientOptions.SocketResponseTimeout, tokenData =>
            {
                if (tokenData.Type != JTokenType.Object)
                    return false;

                if (tokenData["event"]?.ToString() != "auth")
                    return false;

                var authResponse = Deserialize<BitfinexAuthenticationResponse>(tokenData);
                if (!authResponse)
                {
                    log.Write(LogLevel.Warning, $"Socket {s.Socket.Id} authentication failed: " + authResponse.Error);
                    result = new CallResult<bool>(authResponse.Error!);
                    return false;
                }

                if (authResponse.Data.Status != "OK")
                {
                    var error = new ServerError(authResponse.Data.ErrorCode, authResponse.Data.ErrorMessage ?? "-");
                    result = new CallResult<bool>(error);
                    log.Write(LogLevel.Debug, $"Socket {s.Socket.Id} authentication failed: " + error);
                    return false;
                }

                log.Write(LogLevel.Debug, $"Socket {s.Socket.Id} authentication completed");
                result = new CallResult<bool>(true);
                return true;
            }).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
#pragma warning disable 8765
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T>? callResult)
#pragma warning restore 8765
        {
            callResult = null;
            if (data.Type != JTokenType.Array)
                return false;

            var array = (JArray) data;
            if (array.Count < 3)
                return false;

            var bfRequest = (BitfinexSocketQuery)request;
            var evntString = data[1]!.ToString();
            if (!BitfinexEvents.EventMapping.TryGetValue(evntString, out var eventType))
                return false;

            if (eventType == BitfinexEventType.Notification)
            {
                var notificationData = (JArray) data[2]!;
                var notificationType = BitfinexEvents.EventMapping[notificationData[1].ToString()];
                if (notificationType != BitfinexEventType.OrderNewRequest
                    && notificationType != BitfinexEventType.OrderCancelRequest
                    && notificationType != BitfinexEventType.OrderUpdateRequest
                    && notificationType != BitfinexEventType.OrderCancelMultiRequest)
                {
                    return false;
                }

                var statusString = (notificationData[6].ToString()).ToLower(CultureInfo.InvariantCulture);
                if (statusString == "error")
                {
                    if (bfRequest.QueryType == BitfinexEventType.OrderNew && notificationType == BitfinexEventType.OrderNewRequest)
                    {
                        var orderData = notificationData[4];
                        if (orderData[2]?.ToString() != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancel && notificationType == BitfinexEventType.OrderCancelRequest)
                    {
                        var orderData = notificationData[4];
                        if (orderData[0]?.ToString() != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderUpdate && notificationType == BitfinexEventType.OrderUpdateRequest)
                    {
                        // OrderUpdateRequest not found notification doesn't carry the order id, where as OrderCancelRequest not found notification does..
                        // Anyway, can't check for ids, so just assume its for this one

                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && notificationType == BitfinexEventType.OrderCancelMultiRequest)
                    {
                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }
                }

                if (notificationType == BitfinexEventType.OrderNewRequest
                || notificationType == BitfinexEventType.OrderUpdateRequest
                || notificationType == BitfinexEventType.OrderCancelRequest)
                {
                    if (bfRequest.QueryType == BitfinexEventType.OrderNew
                    || bfRequest.QueryType == BitfinexEventType.OrderUpdate
                    || bfRequest.QueryType == BitfinexEventType.OrderCancel)
                    {
                        var orderData = notificationData[4];
                        var dataOrderId = orderData[0]?.ToString();
                        var dataOrderClientId = orderData[2]?.ToString();
                        if (dataOrderId == bfRequest.Id || dataOrderClientId == bfRequest.Id)
                        {
                            var desResult = Deserialize<T>(orderData);
                            if (!desResult)
                            {
                                callResult = new CallResult<T>(desResult.Error!);
                                return true;
                            }

                            callResult = new CallResult<T>(desResult.Data);
                            return true;
                        }
                    }
                }

                if (notificationType == BitfinexEventType.OrderCancelMultiRequest)
                {
                    callResult = new CallResult<T>(Deserialize<T>(JToken.Parse("true")).Data);
                    return true;
                }
            }

            if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && eventType == BitfinexEventType.OrderCancel)
            {
                callResult = new CallResult<T>(Deserialize<T>(JToken.Parse("true")).Data);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken data, out CallResult<object>? callResult)
        {
            callResult = null;
            if (data.Type != JTokenType.Object)
                return false;

            var infoEvent = data["event"]?.ToString() == "subscribed";
            var errorEvent = data["event"]?.ToString() == "error";
            if (!infoEvent && !errorEvent)
                return false;

            if (infoEvent)
            {
                var subResponse = Deserialize<BitfinexSubscribeResponse>(data);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(subResponse.Error!);
                    log.Write(LogLevel.Warning, $"Socket {s.Socket.Id} subscription failed: " + subResponse.Error);
                    return false;
                }

                var bRequest = (BitfinexSubscriptionRequest) request;
                if (!bRequest.CheckResponse(data))
                    return false;                

                bRequest.ChannelId = subResponse.Data.ChannelId;
                callResult = subResponse.As<object>(subResponse.Data);
                return true;
            }
            else
            {
                var subResponse = Deserialize<BitfinexErrorResponse>(data);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(subResponse.Error!);
                    log.Write(LogLevel.Warning, $"Socket {s.Socket.Id} subscription failed: " + subResponse.Error);
                    return false;
                }

                var error = new ServerError(subResponse.Data.Code, subResponse.Data.Message);
                callResult = new CallResult<object>(error);
                log.Write(LogLevel.Debug, $"Socket {s.Socket.Id} subscription failed: " + error);
                return true;
            }
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            if (message.Type != JTokenType.Array)
                return false;

            var array = (JArray) message;
            if (array.Count < 2)
                return false;

            if (!int.TryParse(array[0].ToString(), out var channelId))
                return false;

            if (channelId == 0)
                return false;

            var subId = ((BitfinexSubscriptionRequest)request).ChannelId;
            return channelId == subId && array[1].ToString() != "hb";
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            if (message.Type == JTokenType.Object)
            {
                if (identifier == "Info")
                    return message["event"]?.ToString() == "info";
                if (identifier == "Conf")
                    return message["event"]?.ToString() == "conf";
            }

            else if (message.Type == JTokenType.Array)
            {
                var array = (JArray) message;
                if (array.Count < 2)
                    return false;

                if (identifier == "HB")
                    return array[1].ToString() == "hb";

                if (!int.TryParse(array[0].ToString(), out var channelId))
                    return false;

                if (channelId != 0)
                    return false;

                var split = identifier.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in split)
                {
                    var events = BitfinexEvents.GetEventsForCategory(id);
                    var eventTypeString = array[1].ToString();
                    var eventType = BitfinexEvents.EventMapping[eventTypeString];
                    var evnt = events.SingleOrDefault(e => e.EventType == eventType);
                    if (evnt != null)
                        return true;
                }
            }

            return false;
        }
    }
}
