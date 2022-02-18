using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.UnitTests.TestImplementations;
using Bitfinex.Net.UnitTests.TestImplementations;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class BitfinexSocketClientTests
    {
        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public async Task SubscribingToBookUpdates_Should_SubscribeSuccessfully(Precision prec, Frequency freq)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket, new BitfinexSocketClientOptions(){ LogLevel = LogLevel.Debug});

            BitfinexOrderBookEntry[] result;
            var subTask = client.SpotStreams.SubscribeToOrderBookUpdatesAsync("tBTCUSD", prec, freq, 25, data => result = data.Data.ToArray());

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 25,
                Pair = "BTCUSD",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "tBTCUSD"
            };

            // act
            socket.InvokeMessage(subResponse);

            var taskResult = await subTask.ConfigureAwait(false);

            // assert
            Assert.IsTrue(taskResult.Success);
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
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket, new BitfinexSocketClientOptions(){ LogLevel = LogLevel.Debug});

            BitfinexOrderBookEntry[] result = null;
            var subTask = client.SpotStreams.SubscribeToOrderBookUpdatesAsync("tBTCUSD", prec, freq, 25, data => result = data.Data.ToArray());

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 25,
                Pair = "BTCUSD",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "tBTCUSD"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexOrderBookEntry[] expected = new[] { new BitfinexOrderBookEntry() };

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }

        [TestCase(KlineInterval.OneMinute)]
        [TestCase(KlineInterval.FiveMinutes)]
        [TestCase(KlineInterval.FifteenMinutes)]
        [TestCase(KlineInterval.ThirtyMinutes)]
        [TestCase(KlineInterval.OneHour)]
        [TestCase(KlineInterval.ThreeHours)]
        [TestCase(KlineInterval.SixHours)]
        [TestCase(KlineInterval.TwelveHours)]
        [TestCase(KlineInterval.OneDay)]
        [TestCase(KlineInterval.SevenDays)]
        [TestCase(KlineInterval.FourteenDays)]
        public void SubscribingToCandleUpdates_Should_SubscribeSuccessfully(KlineInterval timeframe)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket, new BitfinexSocketClientOptions()
            {
                LogLevel = LogLevel.Debug
            });

            var subTask = client.SpotStreams.SubscribeToKlineUpdatesAsync("tBTCUSD", timeframe, data => { });

            var subResponse = new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "BTCUSD",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new KlineIntervalConverter(false)) + ":tBTCUSD"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [TestCase(KlineInterval.OneMinute)]
        [TestCase(KlineInterval.FiveMinutes)]
        [TestCase(KlineInterval.FifteenMinutes)]
        [TestCase(KlineInterval.ThirtyMinutes)]
        [TestCase(KlineInterval.OneHour)]
        [TestCase(KlineInterval.ThreeHours)]
        [TestCase(KlineInterval.SixHours)]
        [TestCase(KlineInterval.TwelveHours)]
        [TestCase(KlineInterval.OneDay)]
        [TestCase(KlineInterval.SevenDays)]
        [TestCase(KlineInterval.FourteenDays)]
        public void SubscribingToCandleUpdates_Should_TriggerWithCandleUpdate(KlineInterval timeframe)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexKline[] result = null;
            var subTask = client.SpotStreams.SubscribeToKlineUpdatesAsync("tBTCUSD", timeframe, data => result = data.Data.ToArray());

            var subResponse = new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "BTCUSD",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new KlineIntervalConverter(false)) + ":tBTCUSD"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexKline[] expected = new[] { new BitfinexKline() };

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SpotStreams.SubscribeToTickerUpdatesAsync("tBTCUSD", data => { });

            var subResponse = new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_TriggerWithTickerUpdate()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexStreamSymbolOverview result = null;
            var subTask = client.SpotStreams.SubscribeToTickerUpdatesAsync("tBTCUSD", data => result = data.Data);

            var subResponse = new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexStreamSymbolOverview expected = new BitfinexStreamSymbolOverview();

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result, expected));
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SpotStreams.SubscribeToRawOrderBookUpdatesAsync("tBTCUSD", 10, data => { });

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_TriggerWithRawBookUpdate()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexRawOrderBookEntry[] result = null;
            var subTask = client.SpotStreams.SubscribeToRawOrderBookUpdatesAsync("tBTCUSD", 10, data => result = data.Data.ToArray());

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexRawOrderBookEntry[] expected = new []{ new BitfinexRawOrderBookEntry()};

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            var subTask = client.SpotStreams.SubscribeToTradeUpdatesAsync("BTCUSD", data => { });

            var subResponse = new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "BTCUSD",
                Pair = "BTCUSD"
            };

            // act
            socket.InvokeMessage(subResponse);

            subTask.Wait(5000);

            // assert
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateSocketClient(socket);

            BitfinexTradeSimple[] result = null;
            var subTask = client.SpotStreams.SubscribeToTradeUpdatesAsync("BTCUSD", data => result = data.Data.ToArray());

            var subResponse = new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "BTCUSD",
                Pair = "BTCUSD"
            };
            socket.InvokeMessage(subResponse);
            subTask.Wait(5000);
            BitfinexTradeSimple[] expected = new[] { new BitfinexTradeSimple() };

            // act
            socket.InvokeMessage($"[1, {JsonConvert.SerializeObject(expected)}]");

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result[0], expected[0]));
        }
        
        [TestCase("ou", BitfinexEventType.OrderUpdate)]
        [TestCase("on", BitfinexEventType.OrderNew)]
        [TestCase("oc", BitfinexEventType.OrderCancel)]
        [TestCase("os", BitfinexEventType.OrderSnapshot, false)]
        public void SubscribingToOrderUpdates_Should_TriggerWithOrderUpdate(string updateType, BitfinexEventType eventType, bool single=true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexOrder>> result = null;
            var expected = new BitfinexSocketEvent<BitfinexOrder[]>(eventType, new [] { new BitfinexOrder() { StatusString = "ACTIVE" }});
            client.SpotStreams.SubscribeToUserTradeUpdatesAsync(data =>
            {
                result = data.Data;
                rstEvent.Set();
            }, null, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] {0, updateType, expected.Data[0] } : new object[] {0, updateType, new[] { expected.Data[0]} });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data[0]));
        }

        [TestCase("te", BitfinexEventType.TradeExecuted)]
        [TestCase("tu", BitfinexEventType.TradeExecutionUpdate)]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>> result = null;
            var expected = new BitfinexSocketEvent<BitfinexTradeDetails[]>(eventType, new[] { new BitfinexTradeDetails() { } });
            client.SpotStreams.SubscribeToUserTradeUpdatesAsync(null,
                data =>
                {
                    result = data.Data;
                    rstEvent.Set();
                }, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexTradeDetails() } : new object[] { 0, updateType, new[] { new BitfinexTradeDetails() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data[0]));
        }

        [TestCase("ws", BitfinexEventType.WalletSnapshot, false)]
        [TestCase("wu", BitfinexEventType.WalletUpdate)]
        public void SubscribingToWalletUpdates_Should_TriggerWithWalletUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexWallet>> result = null;
            var expected = new BitfinexSocketEvent<IEnumerable<BitfinexWallet>>(eventType, new[] { new BitfinexWallet() { } });
            client.SpotStreams.SubscribeToBalanceUpdatesAsync(data =>
                {
                    result = data.Data;
                    rstEvent.Set();
                });

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexWallet() } : new object[] { 0, updateType, new[] { new BitfinexWallet() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data.First()));
        }

        [TestCase("pn", BitfinexEventType.PositionNew)]
        [TestCase("pc", BitfinexEventType.PositionClose)]
        [TestCase("pu", BitfinexEventType.PositionUpdate)]
        [TestCase("ps", BitfinexEventType.PositionSnapshot, false)]
        public void SubscribingToPositionUpdates_Should_TriggerWithPositionUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexPosition>> result = null;
            var expected = new BitfinexSocketEvent<IEnumerable<BitfinexPosition>>(eventType, new[] { new BitfinexPosition() { } });
            client.SpotStreams.SubscribeToUserTradeUpdatesAsync(null, null, data =>
            {
                result = data.Data;
                rstEvent.Set();
            });

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexPosition() } : new object[] { 0, updateType, new[] { new BitfinexPosition() } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data.First()));
        }

        [TestCase("fcn", BitfinexEventType.FundingCreditsNew)]
        [TestCase("fcu", BitfinexEventType.FundingCreditsUpdate)]
        [TestCase("fcc", BitfinexEventType.FundingCreditsClose)]
        [TestCase("fcs", BitfinexEventType.FundingCreditsSnapshot, false)]
        public void SubscribingToFundingCreditsUpdates_Should_TriggerWithFundingCreditsUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>> result = null;
            var expected = new BitfinexSocketEvent<BitfinexFundingCredit[]>(eventType, new[] { new BitfinexFundingCredit() { StatusString="ACTIVE" } });
            client.SpotStreams.SubscribeToFundingUpdatesAsync(null,data =>
            {
                result = data.Data;
                rstEvent.Set();
            }, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexFundingCredit() { StatusString = "ACTIVE" } } : new object[] { 0, updateType, new[] { new BitfinexFundingCredit() { StatusString = "ACTIVE" } } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data[0]));
        }

        [TestCase("fln", BitfinexEventType.FundingLoanNew)]
        [TestCase("flu", BitfinexEventType.FundingLoanUpdate)]
        [TestCase("flc", BitfinexEventType.FundingLoanClose)]
        [TestCase("fls", BitfinexEventType.FundingLoanSnapshot, false)]
        public void SubscribingToFundingLoanUpdates_Should_TriggerWithFundingLoanUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexFunding>> result = null;
            var expected = new BitfinexSocketEvent<BitfinexFunding[]>(eventType, new[] { new BitfinexFunding() { StatusString = "ACTIVE" } });
            client.SpotStreams.SubscribeToFundingUpdatesAsync(null, null, data =>
            {
                result = data.Data;
                rstEvent.Set();
            });

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexFunding() { StatusString = "ACTIVE" } } : new object[] { 0, updateType, new[] { new BitfinexFunding() { StatusString = "ACTIVE" } } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data[0]));
        }

        [TestCase("fon", BitfinexEventType.FundingOfferNew)]
        [TestCase("fou", BitfinexEventType.FundingOfferUpdate)]
        [TestCase("foc", BitfinexEventType.FundingOfferCancel)]
        [TestCase("fos", BitfinexEventType.FundingOfferSnapshot, false)]
        public void SubscribingToFundingOfferUpdates_Should_TriggerWithFundingOfferUpdate(string updateType, BitfinexEventType eventType, bool single = true)
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var rstEvent = new ManualResetEvent(false);
            BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>> result = null;
            var expected = new BitfinexSocketEvent<BitfinexFundingOffer[]>(eventType, new[] { new BitfinexFundingOffer() { StatusString = "ACTIVE" } });
            client.SpotStreams.SubscribeToFundingUpdatesAsync(data =>
            {
                result = data.Data;
                rstEvent.Set();
            }, null, null);

            // act
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            socket.InvokeMessage(single ? new object[] { 0, updateType, new BitfinexFundingOffer() { StatusString = "ACTIVE" } } : new object[] { 0, updateType, new[] { new BitfinexFundingOffer() { StatusString = "ACTIVE" } } });
            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.AreEqual(result.Data.First(), expected.Data[0]));
        }

        [Test]
        public void PlacingAnOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                Quantity = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeLimit,
                ClientOrderId = 1234,
                StatusString = "ACTIVE"
            };

            // act
            var placeTask = client.SpotStreams.PlaceOrderAsync(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonConvert.SerializeObject(expected)}, 0, \"SUCCESS\", \"Submitted\"]]");
            var result = placeTask.Result;

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfErrorResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);
            var order = new BitfinexOrder() {ClientOrderId = 123};

            // act
            var placeTask = client.SpotStreams.PlaceOrderAsync(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 123);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonConvert.SerializeObject(order)}, 0, \"error\", \"order placing failed\"]]");
            var result = placeTask.Result;

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Error.Message.Contains("order placing failed"));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfNoResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket, new BitfinexSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("Test", "Test"),
                LogLevel = LogLevel.Debug,
                SocketResponseTimeout = TimeSpan.FromMilliseconds(100)
            });

            // act
            var placeTask = client.SpotStreams.PlaceOrderAsync(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 123);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            var result = placeTask.Result;

            // assert
            Assert.IsFalse(result.Success);
        }

        [Test]
        public void PlacingAnMarketOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                Quantity = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeMarket,
                ClientOrderId = 1234,
                StatusString = "EXECUTED"
            };

            // act
            var placeTask = client.SpotStreams.PlaceOrderAsync(OrderType.ExchangeMarket, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonConvert.SerializeObject(expected)}, 0, \"SUCCESS\", \"Submitted\"]]");
            var result = placeTask.Result;

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void PlacingAnFOKOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket);

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                Quantity = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeFillOrKill,
                ClientOrderId = 1234,
                StatusString = "CANCELED"
            };

            // act
            var placeTask = client.SpotStreams.PlaceOrderAsync(OrderType.ExchangeFillOrKill, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(new BitfinexAuthenticationResponse() { Event = "auth", Status = "OK" });
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonConvert.SerializeObject(expected)}, 0, \"SUCCESS\", \"Submitted\"]]");
            var result = placeTask.Result;

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void ReceivingAReconnectMessage_Should_ReconnectWebsocket()
        {
            // arrange
            var socket = new TestSocket();
            socket.CanConnect = true;
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket, new BitfinexSocketClientOptions()
            {
                LogLevel = LogLevel.Debug,
                ReconnectInterval = TimeSpan.FromMilliseconds(10)
            });

            var rstEvent = new ManualResetEvent(false);
            var subTask = client.SpotStreams.SubscribeToKlineUpdatesAsync("tBTCUSD", KlineInterval.FiveMinutes, data => { });
            socket.InvokeMessage(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Key = "trade:" + JsonConvert.SerializeObject(KlineInterval.FiveMinutes, new KlineIntervalConverter(false)) + ":tBTCUSD"
            });
            var subResult = subTask.Result;

            subResult.Data.ConnectionRestored += (t) => rstEvent.Set();

            // act
            socket.InvokeMessage("{\"event\":\"info\", \"code\": 20051}");
            Thread.Sleep(100);
            socket.InvokeMessage(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Key = "trade:" + JsonConvert.SerializeObject(KlineInterval.FiveMinutes, new KlineIntervalConverter(false)) + ":tBTCUSD"
            });

            var triggered = rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(triggered);
        }

        [Test]
        public async Task ReceivingAReconnectMessage_Should_PreventOperationsBeforeReconnect()
        {
            // arrange
            CallResult<UpdateSubscription> subResultWhenPaused = null;
            var socket = new TestSocket();
            socket.CanConnect = true;
            socket.CloseTime = TimeSpan.FromMilliseconds(1000);
            socket.OpenTime = TimeSpan.FromMilliseconds(100);
            socket.Url = "wss://api.bitfinex.com/ws/2";
            var client = TestHelpers.CreateAuthenticatedSocketClient(socket, new BitfinexSocketClientOptions()
            {
                LogLevel = LogLevel.Debug,
                ReconnectInterval = TimeSpan.FromMilliseconds(10)
            });

            var rstEvent = new ManualResetEvent(false);
            var subTask = client.SpotStreams.SubscribeToKlineUpdatesAsync("tBTCUSD", KlineInterval.FiveMinutes, data => { });
            socket.OnOpen += async () =>
            {
                await Task.Delay(10);
                socket.InvokeMessage(new CandleSubscriptionResponse()
                {
                    Channel = "candles",
                    Event = "subscribed",
                    ChannelId = 1,
                    Symbol = "tBTCUSD",
                    Key = "trade:" + JsonConvert.SerializeObject(KlineInterval.FiveMinutes, new KlineIntervalConverter(false)) +
                          ":tBTCUSD"
                });
            };
            var subResult = await subTask;

            subResult.Data.ActivityPaused += async () =>
            {
                subResultWhenPaused = await client.SpotStreams.SubscribeToKlineUpdatesAsync("tBTCUSD", KlineInterval.FiveMinutes, data => { });
                rstEvent.Set();
            };

            // act
            socket.InvokeMessage("{\"event\":\"info\", \"code\": 20051}");

            rstEvent.WaitOne(1000);

            // assert
            Assert.IsTrue(subResultWhenPaused?.Error?.Message.Contains("Socket is paused"));
        }
    }
}
