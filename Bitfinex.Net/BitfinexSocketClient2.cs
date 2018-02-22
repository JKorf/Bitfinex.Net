using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects.SocketObjects2;
using Bitfinex.Net.Objects.SocketObjets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace Bitfinex.Net
{
    public class BitfinexSocketClient2: BitfinexAbstractClient
    {
        private const string BaseAddress = "wss://api.bitfinex.com/ws/2";
        private static WebSocket socket;

        private static AutoResetEvent messageEvent;
        private static AutoResetEvent sendEvent;
        private static ConcurrentQueue<string> receivedMessages;
        private static ConcurrentQueue<string> toSendMessages;

        private static List<SubsciptionRequest> outstandingRequests;
        private static bool running;
        private static object subscriptionLock;

        public BitfinexSocketClient2()
        {
            receivedMessages = new ConcurrentQueue<string>();
            toSendMessages = new ConcurrentQueue<string>();
            messageEvent = new AutoResetEvent(true);
            sendEvent = new AutoResetEvent(true);
            outstandingRequests = new List<SubsciptionRequest>();
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

        public async Task SubscribeToTicker(string symbol)
        {
            var request = new TickerSubscriptionRequest(symbol);
            outstandingRequests.Add(request);
            Send(JsonConvert.SerializeObject(request));
            await Task.Run(() =>
            {
                bool result = request.ConfirmedEvent.WaitOne(2000);
                outstandingRequests.Remove(request);
                if (!result)
                    log.Write(LogVerbosity.Warning, "No confirmation received");
                else
                    log.Write(LogVerbosity.Warning, "Subscription confirmed");
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
                        if (evnt == "subscribed")
                        {
                            var channel = dataObject["channel"].ToString();
                            SubscriptionResponse subResponse = null;

                            if (channel == "ticker")
                                subResponse = dataObject.ToObject<TickerSubscriptionResponse>();

                            if (subResponse == null)
                            {
                                log.Write(LogVerbosity.Debug, "Unknown subscription channel response");
                                continue;
                            }

                            var pending = outstandingRequests.SingleOrDefault(r => r.GetSubscriptionKey() == subResponse.GetSubscriptionKey());
                            if (pending == null)
                            {
                                log.Write(LogVerbosity.Debug, "Couldn't find sub request for response");
                                continue;
                            }
                            // Do something with channelid
                            pending.ConfirmedEvent.Set();
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
