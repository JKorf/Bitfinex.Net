using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects2;
using Bitfinex.Net.Objects.SocketObjets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace Bitfinex.Net
{
    public partial class BitfinexSocketClient2: BitfinexAbstractClient
    {
        private const string BaseAddress = "wss://api.bitfinex.com/ws/2";
        private static WebSocket socket;

        private static AutoResetEvent messageEvent;
        private static AutoResetEvent sendEvent;
        private static ConcurrentQueue<string> receivedMessages;
        private static ConcurrentQueue<string> toSendMessages;

        private static Dictionary<string, Type> subscriptionResponseTypes;

        private static List<SubscriptionRequest> outstandingRequests;
        private static List<SubscriptionRequest> confirmedRequests;

        private static bool running;
        private static object subscriptionLock;

        public BitfinexSocketClient2()
        {
            receivedMessages = new ConcurrentQueue<string>();
            toSendMessages = new ConcurrentQueue<string>();
            messageEvent = new AutoResetEvent(true);
            sendEvent = new AutoResetEvent(true);
            outstandingRequests = new List<SubscriptionRequest>();
            confirmedRequests = new List<SubscriptionRequest>();

            subscriptionResponseTypes = new Dictionary<string, Type>();
            foreach (Type t in typeof(SubscriptionResponse).Assembly.GetTypes())
            {
                if (typeof(SubscriptionResponse).IsAssignableFrom(t) && t.Name != typeof(SubscriptionResponse).Name)
                {
                    var attribute = (SubscriptionChannelAttribute)t.GetCustomAttributes(typeof(SubscriptionChannelAttribute), true)[0];
                    subscriptionResponseTypes.Add(attribute.ChannelName, t);
                }
            }
        }

        public void Start()
        {
            socket = new WebSocket(BaseAddress);
            socket.Log.Level = LogLevel.Info;
            socket.OnClose += SocketClosed;
            socket.OnError += SocketError;
            socket.OnOpen += SocketOpened;
            socket.OnMessage += SocketMessage;

            socket.Connect();

            running = true;
            Task.Run(() => ProcessData());
            Task.Run(() => ProcessSending());
        }

        

        private async Task SubscribeAndWait(SubscriptionRequest request)
        {
            outstandingRequests.Add(request);
            Send(JsonConvert.SerializeObject(request));
            await Task.Run(() =>
            {
                bool result = request.ConfirmedEvent.WaitOne(2000);
                outstandingRequests.Remove(request);
                confirmedRequests.Add(request);
                log.Write(LogVerbosity.Debug, !result ? "No confirmation received" : "Subscription confirmed");
            });
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
        }

        private void SocketMessage(object sender, MessageEventArgs args)
        {
            log.Write(LogVerbosity.Debug, "Received message: " + args.Data);
            receivedMessages.Enqueue(args.Data);
            messageEvent.Set();
        }

        private void Send(string data)
        {
            toSendMessages.Enqueue(data);
            sendEvent.Set();
        }

        private void ProcessSending()
        {
            while (true)
            {
                sendEvent.WaitOne();
                if (!running)
                    break;
                
                string data;
                while (toSendMessages.TryDequeue(out data))
                {
                    log.Write(LogVerbosity.Debug, "Sending " + data);
                    socket.Send(data);
                }
            }
        }

        private void ProcessData()
        {
            while (true)
            {
                messageEvent.WaitOne();
                if (!running)
                    break;

                string dequeued;
                while (receivedMessages.TryDequeue(out dequeued))
                {
                    log.Write(LogVerbosity.Debug, "Processing " + dequeued);

                    var dataObject = JToken.Parse(dequeued);
                    if (dataObject is JObject)
                    {
                        var evnt = dataObject["event"].ToString();
                        if (evnt == "auth")
                        {
                            ProcessAuthenticationResponse(dataObject.ToObject<BitfinexAuthenticationResponse>());
                        }
                        else if (evnt == "subscribed")
                        {
                            var channel = dataObject["channel"].ToString();
                            if (!subscriptionResponseTypes.ContainsKey(channel))
                            {
                                log.Write(LogVerbosity.Warning, "Unknown response channel name: " + channel);
                                continue;
                            }

                            SubscriptionResponse subResponse = (SubscriptionResponse) dataObject.ToObject(subscriptionResponseTypes[channel]);
                            var pending = outstandingRequests.SingleOrDefault(r => r.GetSubscriptionKey() == subResponse.GetSubscriptionKey());
                            if (pending == null)
                            {
                                log.Write(LogVerbosity.Debug, "Couldn't find sub request for response");
                                continue;
                            }
                            // Do something with channelid
                            pending.ChannelId = subResponse.ChannelId;
                            pending.ConfirmedEvent.Set();
                        }
                    }
                    else
                    {
                        var channelId = (int)dataObject[0];
                        if (dataObject[1].ToString() == "hb")
                            continue;

                        var channelReg = confirmedRequests.SingleOrDefault(c => c.ChannelId == channelId);
                        if (channelReg != null)
                        {
                            channelReg.Handle((JArray) dataObject);
                            return;
                        }

                        var accountReg = registrations.SingleOrDefault(r => r.UpdateKeys.Contains(dataObject[1].ToString()));
                        if (accountReg != null)
                        {
                            accountReg.Handle((JArray)dataObject);
                            return;
                        }
                    }
                }
            }

            socket.Close();
        }

        public void Stop()
        {
            running = false;
            messageEvent.Set();
            sendEvent.Set();
        }
    }
}
