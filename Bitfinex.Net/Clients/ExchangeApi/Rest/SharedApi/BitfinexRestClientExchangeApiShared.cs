using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    internal partial class BitfinexRestClientExchangeSharedApi :
        SharedApiBase,
        IBitfinexRestClientExchangeApiShared,
        IBitfinexRestClientExchangeSharedApi
    {
        private readonly BitfinexRestClientExchangeApi _api;

        private const string _exchangeName = "Bitfinex";
        private const string _topicSpotId = "BitfinexSpot";
        private const string _topicFuturesId = "BitfinexFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(BitfinexExchange.Metadata, this);

        private static HashSet<string> _exchangeSupportedFiat = ["USD", "EUR", "GBP"];

        public BitfinexRestClientExchangeSharedApi(BitfinexRestClientExchangeApi api)
            : base(
                  SharedTransport.Rest,
                  api.Exchange,
                  new[] { TradingMode.Spot, TradingMode.PerpetualLinear },
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                GetKlinesOptions,
                GetAllAssetsOptions,
                GetAssetOptions,
                GetSpotSymbolsOptions,
                GetFuturesSymbolsOptions,
                GetTickerOptions,
                GetAllTickersOptions,
                GetBookTickerOptions,
                GetRecentTradesOptions,
                GetBalancesOptions,
                PlaceSpotOrderOptions,
                GetSpotOrderOptions,
                GetOpenSpotOrdersOptions,
                GetClosedSpotOrdersOptions,
                GetSpotOrderTradesOptions,
                GetSpotUserTradeHistoryOptions,
                CancelSpotOrderOptions,
                GetDepositAddressesOptions,
                GetDepositHistoryOptions,
                GetOrderBookOptions,
                GetTradeHistoryOptions,
                GetWithdrawalHistoryOptions,
                WithdrawOptions,
                GetFeeOptions,
                GetSpotTriggerOrderOptions,
                PlaceSpotTriggerOrderOptions,
                CancelSpotTriggerOrderOptions,
                TransferOptions,
                GetOpenFuturesOrdersOptions,
                PlaceFuturesOrderOptions,
                GetFuturesOrderOptions,
                GetOpenFuturesOrdersOptions,
                GetClosedFuturesOrdersOptions,
                GetFuturesOrderTradesOptions,
                GetFuturesUserTradeHistoryOptions,
                CancelFuturesOrderOptions,
                GetPositionsOptions
                );
        }
    }
}
