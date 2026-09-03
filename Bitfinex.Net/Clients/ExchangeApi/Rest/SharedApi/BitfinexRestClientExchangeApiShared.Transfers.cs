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
        #region Transfer client

        public TransferOptions TransferOptions { get; } = new TransferOptions(_exchangeName, [
            SharedAccountType.Funding,
            SharedAccountType.Spot,
            SharedAccountType.CrossMargin,
            SharedAccountType.IsolatedMargin
            ]);
        public async Task<HttpResult<SharedId>> TransferAsync(TransferRequest request, CancellationToken ct)
        {
            var validationError = TransferOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var fromAccount = GetTransferType(request.FromAccountType);
            var toAccount = GetTransferType(request.ToAccountType);
            if (fromAccount == null || toAccount == null)
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid("To/From AccountType", "invalid to/from account combination"));

            // Get data
            var transfer = await _api.Account.WalletTransferAsync(
                request.Asset,
                request.Quantity,
                fromAccount.Value,
                toAccount.Value,
                ct: ct).ConfigureAwait(false);
            if (!transfer.Success)
                return HttpResult.Fail<SharedId>(transfer);

            return HttpResult.Ok(transfer, new SharedId(""));
        }

        private WithdrawWallet? GetTransferType(SharedAccountType type)
        {
            if (type == SharedAccountType.Funding) return WithdrawWallet.Deposit;
            if (type == SharedAccountType.Spot) return WithdrawWallet.Exchange;
            if (type.IsMarginAccount()) return WithdrawWallet.Trading;
            return null;
        }

        #endregion
    }
}
