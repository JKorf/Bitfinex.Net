using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Logging;
using WebSocketSharp;
using Bitfinex.Net.Objects.SocketObjets;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Errors;
using System.Diagnostics;

namespace Bitfinex.Net
{
    public class BitfinexSocketClient: BitfinexAbstractClient
    {
        private const string BaseAddress = "wss://api.bitfinex.com/ws/2";
        private const string AuthenticationSucces = "OK";

        private const string PositionsSnapshotEvent = "ps";
        private const string WalletsSnapshotEvent = "ws";
        private const string OrdersSnapshotEvent = "os";
        private const string FundingOffersSnapshotEvent = "fos";
        private const string FundingCreditsSnapshotEvent = "fcs";
        private const string FundingLoansSnapshotEvent = "fls";
        private const string ActiveTradesSnapshotEvent = "ats"; // OK?
        private const string HeartbeatEvent = "hb";

        private static WebSocket socket;
        private static bool authenticated;

        private List<BitfinexEventRegistration> eventRegistrations = new List<BitfinexEventRegistration>();

        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString(CultureInfo.InvariantCulture);

        private object subscriptionLock = new object();
        private object registrationIdLock = new object();
        private object eventListLock = new object();

        private long _regId;
        private long registrationId
        {
            get
            {
                lock (registrationIdLock)
                {
                    return ++_regId;
                }
            }
        }

        public BitfinexSocketClient()
        {
        }

        public BitfinexSocketClient(string apiKey, string apiSecret)
        {
            SetApiCredentials(apiKey, apiSecret);
        }

        public void Connect()
        {
            socket = new WebSocket(BaseAddress);
            socket.Log.Level = LogLevel.Info;
            socket.OnClose += SocketClosed;
            socket.OnError += SocketError;
            socket.OnOpen += SocketOpened;
            socket.OnMessage += SocketMessage;

            socket.Connect();
        }

        private void SocketClosed(object sender, CloseEventArgs args)
        {
            log.Write(LogVerbosity.Debug, "Socket closed");
        }

        private void SocketError(object sender, ErrorEventArgs args)
        {
            log.Write(LogVerbosity.Error, $"Socket error: {args.Exception.GetType().Name} - {args.Message}");
        }

        private void SocketOpened(object sender, EventArgs args)
        {
            log.Write(LogVerbosity.Debug, $"Socket opened");

            if(!string.IsNullOrEmpty(apiKey) && encryptor != null)
                Authenticate();
        }

        private void SocketMessage(object sender, MessageEventArgs args)
        {
            var dataObject = JToken.Parse(args.Data);
            if(dataObject is JObject)
            {
                log.Write(LogVerbosity.Debug, $"Received object message: {dataObject}");
                var evnt = dataObject["event"].ToString();
                if (evnt == "info")
                    HandleInfoEvent(dataObject.ToObject<BitfinexInfo>());
                else if (evnt == "auth")
                    HandleAuthenticationEvent(dataObject.ToObject<BitfinexAuthenticationResponse>());
                else if (evnt == "subscribed")
                    HandleSubscriptionEvent(dataObject.ToObject<BitfinexSubscriptionResponse>());
                else if (evnt == "error")
                    HandleErrorEvent(dataObject.ToObject<BitfinexSocketError>());                
                else
                    HandleUnhandledEvent((JObject)dataObject);                
            }
            else if(dataObject is JArray)
            {
                log.Write(LogVerbosity.Debug, $"Received array message: {dataObject}");
                if(dataObject[1].ToString() == "hb")
                {
                    // Heartbeat, no need to do anything with that
                    return;
                }

                if (dataObject[0].ToString() == "0")
                    HandleAccountEvent(dataObject.ToObject<BitfinexSocketEvent>());
                else
                    HandleChannelEvent((JArray)dataObject);
            }
        }

        private void Authenticate()
        {
            var n = nonce;
            var authentication = new BitfinexAuthentication()
            {
                Event = "auth",
                ApiKey = apiKey,
                Nonce = n,
                Payload = "AUTH" + n
            };
            authentication.Signature = ByteToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(authentication.Payload)));

            socket.Send(JsonConvert.SerializeObject(authentication));
        }

        private void HandleAuthenticationEvent(BitfinexAuthenticationResponse response)
        {
            if(response.Status == AuthenticationSucces)
            {
                authenticated = true;
                log.Write(LogVerbosity.Debug, $"Socket authentication successful, authentication id : {response.AuthenticationId}");
            }
            else
            {
                log.Write(LogVerbosity.Warning, $"Socket authentication failed. Status: {response.Status}, Error code: {response.ErrorCode}, Error message: {response.ErrorMessage}");
            }
        }

        private void HandleInfoEvent(BitfinexInfo info)
        {
            if (info.Version != 0)
                log.Write(LogVerbosity.Debug, $"API protocol version {info.Version}");

            if (info.Code != 0)
            {
                // 20051 reconnect
                // 20060 maintanance, pause
                // 20061 maintanance end, resub
            }
        }

        private void HandleSubscriptionEvent(BitfinexSubscriptionResponse subscription)
        {
            BitfinexEventRegistration pending;
            lock(eventListLock)
                pending = eventRegistrations.SingleOrDefault(r => r.ChannelName == subscription.ChannelName && !r.Confirmed);

            if (pending == null) {
                log.Write(LogVerbosity.Warning, "Received registration confirmation but have nothing pending?");
                return;
            }

            pending.ChannelId = subscription.ChannelId;
            pending.Confirmed = true;
            log.Write(LogVerbosity.Debug, $"Subscription confirmed for channel {subscription.ChannelName}, ID: {subscription.ChannelId}");
        }

        private void HandleErrorEvent(BitfinexSocketError error)
        {
            log.Write(LogVerbosity.Warning, $"Bitfinex socket error: {error.ErrorCode} - {error.ErrorMessage}");
            BitfinexEventRegistration waitingRegistration;
            lock (eventListLock) 
                waitingRegistration = eventRegistrations.SingleOrDefault(e => !e.Confirmed);

            if (waitingRegistration != null)
                waitingRegistration.Error = new BitfinexError(error.ErrorCode, error.ErrorMessage);
        }

        private void HandleUnhandledEvent(JObject data)
        {
            log.Write(LogVerbosity.Debug, $"Received uknown event: { data }");
        }

        private void HandleAccountEvent(BitfinexSocketEvent evnt)
        {
            if (evnt.Event == WalletsSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexWallet[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexWalletSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == OrdersSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexOrder[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexOrderSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == PositionsSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexPosition[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexPositionsSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == FundingOffersSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexFundingOffer[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexFundingOffersSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == FundingCreditsSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexFundingCredit[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexFundingCreditsSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == FundingLoansSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexFundingLoan[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexFundingLoansSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == ActiveTradesSnapshotEvent)
            {
            }
            else
            {
                log.Write(LogVerbosity.Warning, $"Received unknown account event: {evnt.Event}, data: {evnt.Data}");
            }
        }

        private void HandleChannelEvent(JArray evnt)
        {
            BitfinexEventRegistration registration;
            lock (eventListLock)
                registration = eventRegistrations.SingleOrDefault(s => s.ChannelId == (int)evnt[0]);

            if(registration == null)
            {
                log.Write(LogVerbosity.Warning, "Received event but have no registration");
                return;
            }

            if (registration is BitfinexTradingPairTickerEventRegistration)
                ((BitfinexTradingPairTickerEventRegistration)registration).Handler(evnt[1].ToObject<BitfinexSocketTradingPairTick>());

            if (registration is BitfinexFundingPairTickerEventRegistration)
                ((BitfinexFundingPairTickerEventRegistration)registration).Handler(evnt[1].ToObject<BitfinexSocketFundingPairTick>());

            if (registration is BitfinexTradeEventRegistration)
            {
                if(evnt[1] is JArray)
                    ((BitfinexTradeEventRegistration)registration).Handler(evnt[1].ToObject<BitfinexTradeSimple[]>());
                else
                    ((BitfinexTradeEventRegistration)registration).Handler(new[] { evnt[2].ToObject<BitfinexTradeSimple>() });
            }
        }

        public long SubscribeToOrdersSnapshot(Action<BitfinexOrder[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexOrderSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = OrdersSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public long SubscribeToWalletSnapshotEvent(Action<BitfinexWallet[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexWalletSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = WalletsSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public long SubscribeToPositionsSnapshotEvent(Action<BitfinexPosition[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexPositionsSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                EventTypes = new List<string>() { PositionsSnapshotEvent },
                Handler = handler
            });

            return id;
        }

        public long SubscribeToFundingOffersSnapshotEvent(Action<BitfinexFundingOffer[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexFundingOffersSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = FundingOffersSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public long SubscribeToFundingCreditsSnapshotEvent(Action<BitfinexFundingCredit[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexFundingCreditsSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = FundingCreditsSnapshotEvent,
                Handler = handler
            });

            return id;
        }
        
        public long SubscribeToFundingLoansSnapshotEvent(Action<BitfinexFundingLoan[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexFundingLoansSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = FundingLoansSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public BitfinexApiResult<long> SubscribeToTradingPairTicker(string symbol, Action<BitfinexSocketTradingPairTick> handler)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexTradingPairTickerEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "ticker",
                    Handler = handler
                };
                AddEventRegistration(registration);
                
                socket.Send(JsonConvert.SerializeObject(new BitfinexTickerSubscribeRequest(symbol)));

                return WaitSubscription(registration);
            }
        }

        public BitfinexApiResult<long> SubscribeToFundingPairTicker(string symbol, Action<BitfinexSocketFundingPairTick> handler)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexFundingPairTickerEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "ticker",
                    Handler = handler
                };
                AddEventRegistration(registration);
                
                socket.Send(JsonConvert.SerializeObject(new BitfinexTickerSubscribeRequest(symbol)));

                return WaitSubscription(registration);
            }
        }

        public BitfinexApiResult<long> SubscribeToTrades(string symbol, Action<BitfinexTradeSimple[]> handler)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexTradeEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "trades",
                    Handler = handler
                };
                AddEventRegistration(registration);

                socket.Send(JsonConvert.SerializeObject(new BitfinexTradeSubscribeRequest(symbol)));

                return WaitSubscription(registration);
            }
        }

        private BitfinexApiResult<long> WaitSubscription(BitfinexEventRegistration registration)
        {
            var sw = Stopwatch.StartNew();
            if (!registration.CompleteEvent.WaitOne(2000))
            {
                lock(eventListLock)
                    eventRegistrations.Remove(registration);
                return ThrowErrorMessage<long>(BitfinexErrors.GetError(BitfinexErrorKey.SubscriptionNotConfirmed));
            }
            sw.Stop();
            log.Write(LogVerbosity.Debug, $"Wait took {sw.ElapsedMilliseconds}ms");

            if (registration.Confirmed)
                return ReturnResult(registration.Id);

            lock(eventListLock)
                eventRegistrations.Remove(registration);
            return ThrowErrorMessage<long>(registration.Error);            
        }

        private void AddEventRegistration(BitfinexEventRegistration reg)
        {
            lock (eventListLock)
                eventRegistrations.Add(reg);
        }

        private IEnumerable<T> GetRegistrationsOfType<T>()
        {
            lock(eventListLock)
                return eventRegistrations.OfType<T>();
        }
    }
}
