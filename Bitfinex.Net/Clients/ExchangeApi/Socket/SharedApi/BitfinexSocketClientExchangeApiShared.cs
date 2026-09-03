using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    internal partial class BitfinexSocketClientExchangeSharedApi : 
        SharedApiBase,
        IBitfinexSocketClientExchangeApiShared,
        IBitfinexSocketClientExchangeSharedApi
    {
        private readonly BitfinexSocketClientExchangeApi _api;

        private const string _exchangeName = "Bitfinex";
        private const string _topicSpotId = "BitfinexSpot";
        private const string _topicFuturesId = "BitfinexFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(BitfinexExchange.Metadata, this);

        public BitfinexSocketClientExchangeSharedApi(BitfinexSocketClientExchangeApi api)
            : base(
                  SharedTransport.Socket,
                  api.Exchange,
                  new[] { TradingMode.Spot, TradingMode.PerpetualLinear },
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                SubscribeTickerOptions,
                SubscribeTradeOptions,
                SubscribeBookTickerOptions,
                SubscribeBalanceOptions,
                SubscribeSpotOrderOptions,
                SubscribeFuturesOrderOptions,
                SubscribeUserTradeOptions,
                SubscribePositionOptions,
                PlaceFuturesOrderOptions,
                CancelFuturesOrderOptions,
                PlaceSpotOrderOptions,
                CancelSpotOrderOptions,
                SubscribeKlineOptions
                );
        }
    }
}
