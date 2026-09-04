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
        #region Get Asset

        public GetAssetOptions GetAssetOptions { get; } = new GetAssetOptions(_exchangeName, false);
        async Task<ICallResult<SharedAsset>> IGetAsset.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
            => await GetAssetAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedAsset>> GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

            // Execute needed config requests in parallel
            var assetSymbols = _api.ExchangeData.GetAssetSymbolsAsync(ct: ct);
            var assetList = _api.ExchangeData.GetAssetsListAsync(ct: ct);
            var assetMethods = _api.ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
            var assetFees = _api.ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
            var assetTxStatus = _api.ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
            await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
            if (!assetSymbols.Result.Success)
                return HttpResult.Fail<SharedAsset>(assetSymbols.Result);
            if (!assetList.Result.Success)
                return HttpResult.Fail<SharedAsset>(assetList.Result);
            if (!assetMethods.Result.Success)
                return HttpResult.Fail<SharedAsset>(assetMethods.Result);
            if (!assetFees.Result.Success)
                return HttpResult.Fail<SharedAsset>(assetFees.Result);
            if (!assetTxStatus.Result.Success)
                return HttpResult.Fail<SharedAsset>(assetTxStatus.Result);

            var asset = assetList.Result.Data.SingleOrDefault(x => x.Name == request.Asset);
            if (asset == null)
                return HttpResult.Fail<SharedAsset>(assetList.Result, new ServerError(new ErrorInfo(ErrorType.UnknownAsset, "Not found")));

            var symbol = assetSymbols.Result.Data.SingleOrDefault(y => y.Key == asset.FullName).Value ?? asset.Name;
            var fees = assetFees.Result.Data.SingleOrDefault(y => y.Key.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            var assetResult = new SharedAsset(symbol)
            {
                FullName = asset.FullName,
                Networks = assetMethods.Result.Data.Where(y => y.Value.Contains(symbol))?.Select(x =>
                {
                    var status = assetTxStatus.Result.Data.SingleOrDefault(s => s.Method.Equals(x.Key, StringComparison.OrdinalIgnoreCase));
                    return new SharedAssetNetwork(x.Key)
                    {
                        WithdrawFee = fees.Value?.Skip(1).First(),
                        DepositEnabled = status?.DepositStatus ?? false,
                        WithdrawEnabled = status?.WithdrawalStatus ?? false,
                        MinConfirmations = status?.DepositConfirmations
                    };
                }).ToArray()
            };

            return HttpResult.Ok(assetList.Result, assetResult);
        }

        #endregion

        #region Get All Assets

        Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
            => GetAllAssetsAsync(request, ct);
        GetAllAssetsOptions IAssetsRestClient.GetAssetsOptions => GetAllAssetsOptions;

        public GetAllAssetsOptions GetAllAssetsOptions { get; } = new GetAllAssetsOptions(_exchangeName, false);

        async Task<ICallResult<SharedAsset[]>> IGetAllAssets.GetAllAssetsAsync(GetAssetsRequest request, CancellationToken ct)
            => await GetAllAssetsAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedAsset[]>> GetAllAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = GetAllAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

            // Execute needed config requests in parallel
            var assetSymbols = _api.ExchangeData.GetAssetSymbolsAsync(ct: ct);
            var assetList = _api.ExchangeData.GetAssetsListAsync(ct: ct);
            var assetMethods = _api.ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
            var assetFees = _api.ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
            var assetTxStatus = _api.ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
            await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
            if (!assetSymbols.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetSymbols.Result);
            if (!assetList.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetList.Result);
            if (!assetMethods.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetMethods.Result);
            if (!assetFees.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetFees.Result);
            if (!assetTxStatus.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetTxStatus.Result);

            return HttpResult.Ok<SharedAsset[]>(assetList.Result, assetList.Result.Data.Select(x =>
                {
                    var symbol = assetSymbols.Result.Data.SingleOrDefault(y => y.Key == x.FullName).Value ?? x.Name;
                    var fees = assetFees.Result.Data.SingleOrDefault(y => y.Key.Equals(symbol, StringComparison.OrdinalIgnoreCase));
                    if (fees.Key == null)
                        return null;

                    return new SharedAsset(symbol)
                    {
                        FullName = x.FullName,
                        Networks = assetMethods.Result.Data.Where(y => y.Value.Contains(symbol))?.Select(x =>
                        {
                            var status = assetTxStatus.Result.Data.SingleOrDefault(s => s.Method.Equals(x.Key, StringComparison.OrdinalIgnoreCase));
                            return new SharedAssetNetwork(x.Key)
                            {
                                WithdrawFee = fees.Value.Skip(1).First(),
                                DepositEnabled = status?.DepositStatus ?? false,
                                WithdrawEnabled = status?.WithdrawalStatus ?? false,
                                MinConfirmations = status?.DepositConfirmations
                            };
                        }).ToArray()
                    };
                }).Where(x => x != null).ToArray()!);
        }

        #endregion

    }
}
