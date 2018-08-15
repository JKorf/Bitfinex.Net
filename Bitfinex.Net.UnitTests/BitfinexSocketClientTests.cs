using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class BitfinexSocketClientTests
    {
        [TestCase()]
        public void CallingStart_Should_ConnectWebsocket()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct());

            // act
            var result = client.Start();

            // assert
            Assert.IsTrue(result);
            socket.Verify(s => s.Connect(), Times.Once);
        }

        [TestCase()]
        public void CallingStop_Should_DisconnectWebsocket()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct());

            // act
            var result = client.Start();
            client.Stop();
            Thread.Sleep(50);

            // assert
            Assert.IsTrue(result);
            socket.Verify(s => s.Close(), Times.Once);
        }

        [TestCase()]
        public void CallingPlaceOrderWithoutStarting_Should_FailAndGiveError()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct());

            // act
            var result = client.PlaceOrder(OrderType.Stop, "Test", 1);

            // assert
            Assert.IsFalse(result.Success);
        }

        [TestCase()]
        public void NotReceivingSubResponse_Should_ResultInError()
        {
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                SubscribeResponseTimeout = TimeSpan.FromMilliseconds(100)
            }));
            client.Start();

            var result = client.SubscribeToBookUpdates("Test", Precision.PrecisionLevel0, Frequency.Realtime, 10, data => { });
            result.Wait();

            // assert
            Assert.IsFalse(result.Result.Success);

            client.Stop();
        }

        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public void SubscribingToBookUpdates_Should_SubscribeSuccessfully(Precision prec, Frequency freq)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            // act
            var result = client.SubscribeToBookUpdates("Test", prec, freq, 10, data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 10,
                Pair = "Test",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public void SubscribingToBookUpdates_Should_TriggerWithBookUpdate(Precision prec, Frequency freq)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();
            BitfinexOrderBookEntry[] expected = new[] { new BitfinexOrderBookEntry() };
            BitfinexOrderBookEntry[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToBookUpdates("Test", prec, freq, 10, data => {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 10,
                Pair = "Test",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(socket, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [TestCase(TimeFrame.OneMinute)]
        [TestCase(TimeFrame.FiveMinute)]
        [TestCase(TimeFrame.FiveteenMinute)]
        [TestCase(TimeFrame.ThirtyMinute)]
        [TestCase(TimeFrame.OneHour)]
        [TestCase(TimeFrame.ThreeHour)]
        [TestCase(TimeFrame.SixHour)]
        [TestCase(TimeFrame.TwelfHour)]
        [TestCase(TimeFrame.OneDay)]
        [TestCase(TimeFrame.SevenDay)]
        [TestCase(TimeFrame.FourteenDay)]
        public void SubscribingToCandleUpdates_Should_SubscribeSuccessfully(TimeFrame timeframe)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            // act
            var result = client.SubscribeToCandleUpdates("Test", timeframe, data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new TimeFrameConverter(false)) + ":Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [TestCase(TimeFrame.OneMinute)]
        [TestCase(TimeFrame.FiveMinute)]
        [TestCase(TimeFrame.FiveteenMinute)]
        [TestCase(TimeFrame.ThirtyMinute)]
        [TestCase(TimeFrame.OneHour)]
        [TestCase(TimeFrame.ThreeHour)]
        [TestCase(TimeFrame.SixHour)]
        [TestCase(TimeFrame.TwelfHour)]
        [TestCase(TimeFrame.OneDay)]
        [TestCase(TimeFrame.SevenDay)]
        [TestCase(TimeFrame.FourteenDay)]
        public void SubscribingToCandleUpdates_Should_TriggerWithCandleUpdate(TimeFrame timeframe)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();
            BitfinexCandle[] expected = new[] { new BitfinexCandle() };
            BitfinexCandle[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToCandleUpdates("Test", timeframe, data => {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new TimeFrameConverter(false)) + ":Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(socket, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            // act
            var result = client.SubscribeToTickerUpdates("Test", data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_TriggerWithTickerUpdate()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();
            BitfinexMarketOverview[] expected = new[] { new BitfinexMarketOverview() };
            BitfinexMarketOverview[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToTickerUpdates("Test", data => {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(socket, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            // act
            var result = client.SubscribeToRawBookUpdates("Test", 10,  data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_TriggerWithRawBookUpdate()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();
            BitfinexRawOrderBookEntry[] expected = new[] { new BitfinexRawOrderBookEntry() };
            BitfinexRawOrderBookEntry[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToRawBookUpdates("Test", 10, data => {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(socket, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            // act
            var result = client.SubscribeToTradeUpdates("Test", data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();
            BitfinexTradeSimple[] expected = new[] { new BitfinexTradeSimple() };
            BitfinexTradeSimple[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToTradeUpdates("Test", data => {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(socket, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [TestCase("ou", BitfinexEventType.OrderUpdate)]
        [TestCase("on", BitfinexEventType.OrderNew)]
        [TestCase("oc", BitfinexEventType.OrderCancel)]
        public void SubscribingToOrderUpdates_Should_TriggerWithOrderUpdate(string updateType, BitfinexEventType eventType)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexOrder>(eventType, new BitfinexOrder() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexOrder[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToOrderUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexOrder() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToOrderUpdates_Should_TriggerWithOrderSnapshot()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexOrder[]>(BitfinexEventType.OrderSnapshot, new[] { new BitfinexOrder() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexOrder[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToOrderUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "os", new[] { new BitfinexOrder() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected, actual, "Data"));
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("te", BitfinexEventType.TradeExecuted)]
        [TestCase("tu", BitfinexEventType.TradeExecutionUpdate)]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate(string updateType, BitfinexEventType eventType)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexTradeDetails>(eventType, new BitfinexTradeDetails());
            BitfinexSocketEvent<BitfinexTradeDetails[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToTradeUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexTradeDetails() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToWalletUpdates_Should_TriggerWithWalletUpdate()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexWallet>(BitfinexEventType.WalletUpdate, new BitfinexWallet());
            BitfinexSocketEvent<BitfinexWallet[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToWalletUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "wu", new BitfinexWallet() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToWalletUpdates_Should_TriggerWithWalletSnapshot()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexWallet[]>(BitfinexEventType.WalletSnapshot, new [] { new BitfinexWallet() });
            BitfinexSocketEvent<BitfinexWallet[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToWalletUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "ws", new[] { new BitfinexWallet() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("pn", BitfinexEventType.PositionNew)]
        [TestCase("pu", BitfinexEventType.PositionUpdate)]
        [TestCase("pc", BitfinexEventType.PositionClose)]
        public void SubscribingToPositionUpdates_Should_TriggerWithPositionUpdates(string updateType, BitfinexEventType evntType)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexPosition>(evntType, new BitfinexPosition() { });
            BitfinexSocketEvent<BitfinexPosition[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToPositionUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexPosition() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToPositionUpdates_Should_TriggerWithPositionSnapshot()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexPosition[]>(BitfinexEventType.PositionSnapshot, new[] { new BitfinexPosition() });
            BitfinexSocketEvent<BitfinexPosition[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToPositionUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "ps", new[] { new BitfinexPosition() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("fcn", BitfinexEventType.FundingCreditsNew)]
        [TestCase("fcu", BitfinexEventType.FundingCreditsUpdate)]
        [TestCase("fcc", BitfinexEventType.FundingCreditsClose)]    
        public void SubscribingToFundingCreditsUpdates_Should_TriggerWithFundingCreditsUpdates(string updateType, BitfinexEventType evntType)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexFundingCredit>(evntType, new BitfinexFundingCredit() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexFundingCredit[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingCreditsUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexFundingCredit() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        public void SubscribingToFundingCreditsUpdates_Should_TriggerWithFundingCreditsSnapshot()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexFundingCredit[]>(BitfinexEventType.FundingCreditsSnapshot, new[] { new BitfinexFundingCredit() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexFundingCredit[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingCreditsUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "fcs", new[] { new BitfinexFundingCredit() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("fln", BitfinexEventType.FundingLoanNew)]
        [TestCase("flu", BitfinexEventType.FundingLoanUpdate)]
        [TestCase("flc", BitfinexEventType.FundingLoanClose)]
        public void SubscribingToFundingLoanUpdates_Should_TriggerWithFundingLoanUpdates(string updateType, BitfinexEventType evntType)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexFundingLoan>(evntType, new BitfinexFundingLoan() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexFundingLoan[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingLoansUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexFundingLoan() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToFundingLoanUpdates_Should_TriggerWithFundingLoanSnapshot()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexFundingLoan[]>(BitfinexEventType.FundingLoanSnapshot, new[] { new BitfinexFundingLoan() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexFundingLoan[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingLoansUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "fls", new[] { new BitfinexFundingLoan() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("fon", BitfinexEventType.FundingOfferNew)]
        [TestCase("fou", BitfinexEventType.FundingOfferUpdate)]
        [TestCase("foc", BitfinexEventType.FundingOfferCancel)]
        public void SubscribingToFundingOfferUpdates_Should_TriggerWithFundingOfferUpdates(string updateType, BitfinexEventType evntType)
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexFundingOffer>(evntType, new BitfinexFundingOffer() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexFundingOffer[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingOfferUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexFundingOffer() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToFundingOfferUpdates_Should_TriggerWithFundingOfferSnapshot()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();
            var expected = new BitfinexSocketEvent<BitfinexFundingOffer[]>(BitfinexEventType.FundingOfferSnapshot, new[] { new BitfinexFundingOffer() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexFundingOffer[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingOfferUpdates(data => {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new object[] { 0, "fos", new[] { new BitfinexFundingOffer() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void UnsubscribingFromUpdates_Should_UnsubscribeSuccessfully()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            // act
            var task = client.UnsubscribeFromChannel(result.Result.Data);
            TestHelpers.InvokeWebsocket(socket, "{ \"event\": \"unsubscribed\", \"chanId\": 1, \"channel\": \"book\" }");
            task.Wait();

            // assert
            Assert.IsTrue(task.Result.Success);

            client.Stop();
        }

        [Test]
        public void UnsubscribingFromUpdatesNoResponse_Should_NotSucceed()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                SubscribeResponseTimeout = TimeSpan.FromMilliseconds(100)
            }));
            client.Start();

            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            // act
            var task = client.UnsubscribeFromChannel(result.Result.Data).Result;

            // assert
            Assert.IsFalse(task.Success);

            client.Stop();
        }

        [Test]
        public void UnsubscribingFromUpdates_Should_NoLongerReceiveUpdates()
        {
            // arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug
            }));
            client.Start();

            BitfinexRawOrderBookEntry[] expected = new[] { new BitfinexRawOrderBookEntry() };
            BitfinexRawOrderBookEntry[] actual = null;

            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { actual = data; });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            var task = client.UnsubscribeFromChannel(result.Result.Data);
            TestHelpers.InvokeWebsocket(socket, "{ \"event\": \"unsubscribed\", \"chanId\": 1, \"channel\": \"book\" }");
            task.Wait();

            // act
            TestHelpers.InvokeWebsocket(socket, $"[1, {JsonConvert.SerializeObject(expected)}]");
            Thread.Sleep(100);

            // assert
            Assert.IsTrue(result.Result.Success);
            Assert.IsTrue(task.Result.Success);
            Assert.IsTrue(actual == null);

            client.Stop();
        }

        [Test]
        public void WhenConnectionIsLost_Socket_ShouldReconnect()
        {
            // Arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(200)
                
            }));
            client.Start();
            ManualResetEvent evnt = new ManualResetEvent(false);
            socket.Setup(s => s.Connect()).Returns(Task.FromResult(true)).Callback(() =>
            {
                evnt.Set();
            });

            // Act
            socket.Raise(r => r.OnClose += null);
            

            bool triggered = evnt.WaitOne(1000);

            // Assert
            Assert.IsTrue(triggered);
        }

        [Test]
        public void WhenNoDataReceived_Socket_ShouldReconnect()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());

            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter>() { sb },
                SocketReceiveTimeout = TimeSpan.FromMilliseconds(200),
                ReconnectionInterval = TimeSpan.FromMilliseconds(100)

            }));
            client.Start();
            ManualResetEvent evnt = new ManualResetEvent(false);
            socket.Setup(s => s.Connect()).Returns(Task.FromResult(true)).Callback(() =>
            {
                evnt.Set();
            });

            // Act
            bool triggered = evnt.WaitOne(5000);
            client.Stop();
            Thread.Sleep(100);

            // Assert
            Assert.IsTrue(triggered, sb.ToString());
        }

        [Test]
        public void WhenReconnecting_Socket_ShouldResubscribe()
        {
            // Arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100)

            }));
            client.Start();
            var result = client.SubscribeToCandleUpdates("Test", TimeFrame.FiveMinute, data => { });
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(TimeFrame.FiveMinute, new TimeFrameConverter(false)) + ":Test"
            }));
            result.Wait();

            ManualResetEvent reconEvnt = new ManualResetEvent(false);
            socket.Setup(s => s.Connect()).Returns(Task.FromResult(true)).Callback(() =>
            {
                reconEvnt.Set();
                socket.Raise(s => s.OnOpen += null);
            });

            string resubData = null;
            ManualResetEvent resubEvnt = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) => {
                resubData = msg;
                resubEvnt.Set();
            }));

            // Act
            socket.Raise(r => r.OnClose += null);
            resubEvnt.WaitOne(1000);

            // Assert
            Assert.IsTrue(resubData != null);
            Assert.IsTrue(resubData == "{\"key\":\"trade:5m:Test\",\"event\":\"subscribe\",\"channel\":\"candles\"}");
        }

        [Test]
        public void WhenResubscribingFails_Socket_ShouldReconnectAndTryAgain()
        {
            // Arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100)

            }));
            // Sub first time
            client.Start();
            string resubData = null;
            ManualResetEvent resubEvnt = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) => {
                resubData = msg;
                resubEvnt.Set();
            }));

            var result = client.SubscribeToCandleUpdates("Test", TimeFrame.FiveMinute, data => { });
            resubEvnt.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(TimeFrame.FiveMinute, new TimeFrameConverter(false)) + ":Test"
            }));
            result.Wait();
            // Sub confirmed

            ManualResetEvent reconEvnt = new ManualResetEvent(false);
            socket.Setup(s => s.Connect()).Returns(Task.FromResult(true)).Callback(() =>
            {                
                socket.Raise(s => s.OnOpen += null);
                reconEvnt.Set();
            });            

            // Disconnect and wait reconnect
            socket.Object.Close().Wait();
            resubEvnt.WaitOne(1000);

            // Act
            resubData = null;
            reconEvnt.Reset();
            resubEvnt.Reset();
            socket.Object.Close().Wait(); // Disconnect again

            // Wait for recon and resub events
            reconEvnt.WaitOne(1000);
            resubEvnt.WaitOne(1000);


            // Assert
            Assert.IsTrue(resubData != null);
            Assert.IsTrue(resubData == "{\"key\":\"trade:5m:Test\",\"event\":\"subscribe\",\"channel\":\"candles\"}");
        }

        [Test]
        public void PlacingAnOrder_Should_SucceedIfSuccessResponse()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();

            var authRequest = new ManualResetEvent(false);
            var orderRequest = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                authRequest.Set();
            }));

            authRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234
            });
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                orderRequest.Set();
            }));

            // Act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "Test", 1, price: 1, clientOrderId: 1234);
            orderRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, $"[0, \"on\", {response}]");
            placeTask.Wait();

            // Assert
            Assert.IsTrue(placeTask.Result.Success, sb.ToString());
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfErrorResponse()
        {
            // Arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();

            var authRequest = new ManualResetEvent(false);
            var orderRequest = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                authRequest.Set();
            }));

            authRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234
            });
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                orderRequest.Set();
            }));

            // Act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "Test", 1, price: 1, clientOrderId: 1234);
            orderRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, $"[0, \"n\", [0, \"on-req\", 0, 0, {response}, 0, \"error\", \"order placing failed\"]]");
            placeTask.Wait();

            // Assert
            Assert.IsFalse(placeTask.Result.Success);
            Assert.IsTrue(placeTask.Result.Error.Message.Contains("order placing failed"));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfNoResponse()
        {
            // Arrange
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test"),
                OrderActionConfirmationTimeout = TimeSpan.FromMilliseconds(100)
            }));
            client.Start();
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            
            // Act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "Test", 1, price: 1, clientOrderId: 1234);

            // Assert
            Assert.IsFalse(placeTask.Result.Success);
        }

        [Test]
        public void PlacingAMarketOrder_Should_SucceedIfSuccessResponse()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();

            var authRequest = new ManualResetEvent(false);
            var orderRequest = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                authRequest.Set();
            }));

            authRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234,
                Status = OrderStatus.Executed,
                StatusString = "EXECUTED"
            });
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                orderRequest.Set();
            }));

            // Act
            var placeTask = client.PlaceOrderAsync(OrderType.Market, "Test", 1, price: 1, clientOrderId: 1234);
            orderRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, $"[0, \"oc\", {response}]");
            placeTask.Wait();

            // Assert
            Assert.IsTrue(placeTask.Result.Success, sb.ToString());
        }

        [TestCase(OrderType.Market)]
        [TestCase(OrderType.ExchangeMarket)]
        public void PlacingAMarketOrder_Should_FailIfStatusNotExecuted(OrderType orderType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test"),
                OrderActionConfirmationTimeout = TimeSpan.FromMilliseconds(100)
            }));
            client.Start();

            var authRequest = new ManualResetEvent(false);
            var orderRequest = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                authRequest.Set();
            }));

            authRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234,
                Status = OrderStatus.Canceled,
                Type = orderType
            });
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                orderRequest.Set();
            }));

            // Act
            var placeTask = client.PlaceOrderAsync(orderType, "Test", 1, price: 1, clientOrderId: 1234);
            orderRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, $"[0, \"oc\", {response}]");
            placeTask.Wait();

            // Assert
            Assert.IsFalse(placeTask.Result.Success, sb.ToString());
        }

        [TestCase(OrderType.FillOrKill)]
        [TestCase(OrderType.ExchangeFillOrKill)]
        public void PlacingAFOKOrder_Should_FailIfReceiveCanceled(OrderType type)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var (socket, client) = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            client.Start();

            var authRequest = new ManualResetEvent(false);
            var orderRequest = new ManualResetEvent(false);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                authRequest.Set();
            }));

            authRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234,
                Type = type,
                Status = OrderStatus.Canceled
            });
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((msg) =>
            {
                orderRequest.Set();
            }));

            // Act
            var placeTask = client.PlaceOrderAsync(type, "Test", 1, price: 1, clientOrderId: 1234);
            orderRequest.WaitOne(1000);
            TestHelpers.InvokeWebsocket(socket, $"[0, \"oc\", {response}]");
            placeTask.Wait();

            // Assert
            Assert.IsTrue(placeTask.Result.Success, sb.ToString());
        }


        private BitfinexSocketClient Construct(BitfinexSocketClientOptions options = null)
        {
            if (options != null)
                return new BitfinexSocketClient(options);
            return new BitfinexSocketClient();
        }
    }
}
