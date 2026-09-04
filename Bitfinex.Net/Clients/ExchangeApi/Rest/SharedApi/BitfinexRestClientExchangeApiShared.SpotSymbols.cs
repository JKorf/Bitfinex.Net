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
    internal partial class BitfinexRestClientExchangeSharedApi
    {
        #region Get Spot Symbols

        public SharedSymbolCatalog? SpotSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchangeName, _topicSpotId, _api.EnvironmentName, null);
        public GetSpotSymbolsOptions GetSpotSymbolsOptions { get; } = new GetSpotSymbolsOptions(_exchangeName, false);
        async Task<ICallResult<SharedSpotSymbol[]>> IGetSpotSymbols.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
            => await GetSpotSymbolsAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedSpotSymbol[]>> GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(result);

            var data = result.Data
                .Select(x => ParseSymbol(x))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, _api.EnvironmentName, null, data);
            return HttpResult.Ok(result, SharedUtils.ApplySymbolFilter(data, request));
        }

        #endregion

        private SharedSpotSymbol ParseSymbol(KeyValuePair<string, BitfinexSymbolInfo> s)
        {
            var assets = GetAssets(s.Key);
            var result = new SharedSpotSymbol(assets.BaseAsset, assets.QuoteAsset, "t" + s.Key, true)
            {
                // These apply to all symbols
                PriceSignificantFigures = 5,
                PriceDecimals = 8,
                QuantityDecimals = 8,

                MinTradeQuantity = s.Value.MinOrderQuantity,
                MaxTradeQuantity = s.Value.MaxOrderQuantity,
                DisplayName = s.Key
            };

            if (_exchangeSupportedFiat.Contains(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.Fiat;
            }
            else if (LibraryHelpers.IsCommodity(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                result.BaseAssetSubType = SharedAssetSubType.Commodity;
            }
            else
            {
                result.BaseAssetType = SharedAssetType.Crypto;
            }

            if (_exchangeSupportedFiat.Contains(result.QuoteAsset))
            {
                result.QuoteAssetType = SharedAssetType.Fiat;
            }
            else if (LibraryHelpers.IsCommodity(result.QuoteAsset))
            {
                result.QuoteAssetType = SharedAssetType.TradFi;
                result.QuoteAssetSubType = SharedAssetSubType.Commodity;
            }
            else
            {
                result.QuoteAssetType = SharedAssetType.Crypto;
                if (LibraryHelpers.IsStableCoin(result.QuoteAsset, ["UST"]))
                    result.QuoteAssetSubType = SharedAssetSubType.StableCoin;
            }

            return result;
        }

        private (string BaseAsset, string QuoteAsset) GetAssets(string input)
        {
            string baseAsset;
            string quoteAsset;
            if (input.Contains(":"))
            {
                var split = input.Split(':');
                baseAsset = split[0];
                quoteAsset = split[1];
            }
            else
            {
                baseAsset = input.Substring(0, 3);
                quoteAsset = input.Substring(3);
            }

            return (BitfinexExchange.AssetAliases.ExchangeToCommonName(baseAsset), BitfinexExchange.AssetAliases.ExchangeToCommonName(quoteAsset));
        }
        public async Task<ExchangeCallResult<SharedSymbol[]>> GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicSpotId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, _api.EnvironmentName, null, symbolName));
        }
    }
}
