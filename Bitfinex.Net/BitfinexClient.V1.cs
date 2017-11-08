using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    public partial class BitfinexClient
    {
        private const string SymbolsEndpoint = "symbols";
        private const string SymbolDetailsEndpoint = "symbols_details";
        private const string AccountInfosEndpoint = "account_infos";
        private const string AccountFeesEndpoint = "account_fees";
        private const string AccountSummaryEndpoint = "summary";
        private const string DepositAddressEndpoint = "deposit/new";
        private const string KeyPermissionsEndpoint = "key_info";
        private const string MarginInfoEndpoint = "margin_infos";
        private const string WithdrawEndpoint = "withdraw";
        private const string NewOrderEndpoint = "order/new";
        private const string CancelOrderEndpoint = "order/cancel";
        private const string CancelAllOrdersEndpoint = "order/cancel/all";
        private const string OrderStatusEndpoint = "order/status";
        private const string ClaimPositionEndpoint = "position/claim";


        public BitfinexApiResult<string[]> GetSymbols() => GetSymbolsAsync().Result;
        public async Task<BitfinexApiResult<string[]>> GetSymbolsAsync()
        {
            return await ExecutePublicRequest<string[]>(GetUrl(SymbolsEndpoint, OldApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexSymbol[]> GetSymbolDetails() => GetSymbolDetailsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexSymbol[]>> GetSymbolDetailsAsync()
        {
            return await ExecutePublicRequest<BitfinexSymbol[]>(GetUrl(SymbolDetailsEndpoint, OldApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexAccountInfo> GetAccountInfo() => GetAccountInfoAsync().Result;
        public async Task<BitfinexApiResult<BitfinexAccountInfo>> GetAccountInfoAsync()
        {
            var result = await ExecuteAuthenticatedRequestV1<BitfinexAccountInfo[]>(GetUrl(AccountInfosEndpoint, OldApiVersion), PostMethod);
            if (result.Success)
                return ReturnResult(result.Result[0]);
            return ThrowErrorMessage<BitfinexAccountInfo>(result.Error);
        }

        public BitfinexApiResult<BitfinexAccountFee> GetAccountFees() => GetAccountFeesAsync().Result;
        public async Task<BitfinexApiResult<BitfinexAccountFee>> GetAccountFeesAsync()
        {
			// TODO Doesnt work?
            return await ExecuteAuthenticatedRequestV1<BitfinexAccountFee>(GetUrl(AccountFeesEndpoint, OldApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexAccountSummary> GetAccountSummary() => GetAccountSummaryAsync().Result;
        public async Task<BitfinexApiResult<BitfinexAccountSummary>> GetAccountSummaryAsync()
        {
            return await ExecuteAuthenticatedRequestV1<BitfinexAccountSummary>(GetUrl(AccountSummaryEndpoint, OldApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexDepositAddress> GetDepositAddress(DepositMethod method, WalletType wallet, bool? renew = null) => GetDepositAddressAsync(method, wallet, renew).Result;
        public async Task<BitfinexApiResult<BitfinexDepositAddress>> GetDepositAddressAsync(DepositMethod method, WalletType wallet, bool? renew = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "method", JsonConvert.SerializeObject(method, new DepositMethodConverter(false)) },
                { "wallet_name",  JsonConvert.SerializeObject(wallet, new WalletTypeConverter(false)) } 
            };

			AddOptionalParameter(parameters, "renew", renew?.ToString());

            var result = await ExecuteAuthenticatedRequestV1<BitfinexDepositAddress>(GetUrl(DepositAddressEndpoint, OldApiVersion), PostMethod, parameters);
            if (result.Error != null)
                return ThrowErrorMessage<BitfinexDepositAddress>(result.Error);
            if (result.Result.Result != "success")
                return ThrowErrorMessage<BitfinexDepositAddress>(-3001, result.Result.Result);
            return ReturnResult(result.Result);
        }

        public BitfinexApiResult<BitfinexApiKeyPermissions> GetApiKeyPermissions() => GetApiKeyPermissionsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexApiKeyPermissions>> GetApiKeyPermissionsAsync()
        {
            return await ExecuteAuthenticatedRequestV1<BitfinexApiKeyPermissions>(GetUrl(KeyPermissionsEndpoint, OldApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexMarginInfo[]> GetMarginInformation() => GetMarginInformationAsync().Result;
        public async Task<BitfinexApiResult<BitfinexMarginInfo[]>> GetMarginInformationAsync()
        {
            return await ExecuteAuthenticatedRequestV1<BitfinexMarginInfo[]>(GetUrl(MarginInfoEndpoint, OldApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexWithdrawResult> WithdrawCrypto(WithdrawType withdrawType, WalletType2 walletType, double amount, string address, string paymentId = null) => WithdrawCryptoAsync(withdrawType, walletType, amount, address, paymentId).Result;
        public async Task<BitfinexApiResult<BitfinexWithdrawResult>> WithdrawCryptoAsync(WithdrawType withdrawType, WalletType2 walletType, double amount, string address, string paymentId = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "withdraw_type", JsonConvert.SerializeObject(withdrawType, new WithdrawTypeConverter(false)) },
                { "walletselected",  JsonConvert.SerializeObject(walletType, new WalletType2Converter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "address", address }
            };
			AddOptionalParameter(parameters, "payment_id", paymentId);

            var result = await ExecuteAuthenticatedRequestV1<BitfinexWithdrawResult[]>(GetUrl(WithdrawEndpoint, OldApiVersion), PostMethod, parameters);
            if (result.Error != null)
                return ThrowErrorMessage<BitfinexWithdrawResult>(result.Error);
            if (result.Result[0].Status != "success")
                return ThrowErrorMessage<BitfinexWithdrawResult>(-3002, result.Result[0].Message);
            return ReturnResult(result.Result[0]);
        }

        public BitfinexApiResult<BitfinexWithdrawResult> WithdrawWire(WithdrawType withdrawType,
			WalletType2 walletType, 
			double amount, 
			string accountNumber, 
			string bankName, 
			string bankAddress, 
			string bankCity, 
			string bankCountry, 
			string accountName = null,
			string swiftCode = null,
			string detailPayment= null,
            bool? useExpressWire = null,
			string intermediaryBankName = null,
            string intermediaryBankAddress = null,
            string intermediaryBankCity = null,
            string intermediaryBankCountry = null,
            string intermediaryBankAccount = null,
            string intermediaryBankSwift = null) => WithdrawCryptoAsync(withdrawType, walletType, amount, accountNumber, bankName, bankAddress, bankCity, bankCountry, accountName, 
				swiftCode, detailPayment, useExpressWire, intermediaryBankName, intermediaryBankAddress, intermediaryBankCity, intermediaryBankAccount, intermediaryBankSwift).Result;
        public async Task<BitfinexApiResult<BitfinexWithdrawResult>> WithdrawCryptoAsync(WithdrawType withdrawType,
            WalletType2 walletType,
            double amount,
            string accountNumber,
            string bankName,
            string bankAddress,
            string bankCity,
            string bankCountry,
            string accountName = null,
            string swiftCode = null,
            string detailPayment = null,
            bool? useExpressWire = null,
            string intermediaryBankName = null,
            string intermediaryBankAddress = null,
            string intermediaryBankCity = null,
            string intermediaryBankCountry = null,
            string intermediaryBankAccount = null,
            string intermediaryBankSwift = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "withdraw_type", JsonConvert.SerializeObject(withdrawType, new WithdrawTypeConverter(false)) },
                { "walletselected",  JsonConvert.SerializeObject(walletType, new WalletType2Converter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "account_number", accountNumber },
                { "bank_name", bankName },
                { "bank_address", bankAddress },
                { "bank_city", bankCity },
                { "bank_country", bankCountry }
            };
            AddOptionalParameter(parameters, "account_name", accountName);
            AddOptionalParameter(parameters, "swift", swiftCode);
            AddOptionalParameter(parameters, "detail_payment", detailPayment);
            AddOptionalParameter(parameters, "expressWire", useExpressWire != null ? JsonConvert.SerializeObject(useExpressWire, new BoolToIntConverter(false)) : null);
            AddOptionalParameter(parameters, "intermediary_bank_name", intermediaryBankName);
            AddOptionalParameter(parameters, "intermediary_bank_address", intermediaryBankAddress);
            AddOptionalParameter(parameters, "intermediary_bank_city", intermediaryBankCity);
            AddOptionalParameter(parameters, "intermediary_bank_country", intermediaryBankCountry);
            AddOptionalParameter(parameters, "intermediary_bank_account", intermediaryBankAccount);
            AddOptionalParameter(parameters, "intermediary_bank_swift", intermediaryBankSwift);

            var result = await ExecuteAuthenticatedRequestV1<BitfinexWithdrawResult[]>(GetUrl(WithdrawEndpoint, OldApiVersion), PostMethod, parameters);
            if (result.Error != null)
                return ThrowErrorMessage<BitfinexWithdrawResult>(result.Error);
            if (result.Result[0].Status != "success")
                return ThrowErrorMessage<BitfinexWithdrawResult>(-3003, result.Result[0].Message);
            return ReturnResult(result.Result[0]);
        }

        public BitfinexApiResult<BitfinexPlacedOrder> PlaceOrder(string symbol, 
			double amount,
			double price,
			OrderSide side, 
			OrderType2 type,
			bool? hidden = null,
			bool? postOnly = null, 
			bool? useAllAvailable = null, 
			bool? ocoOrder = null, 
			double? buyPriceOco = null, 
			double? sellPriceOco = null) => PlaceOrderAsync(symbol, amount, price, side, type, hidden, postOnly, useAllAvailable, ocoOrder, buyPriceOco, sellPriceOco).Result;
        public async Task<BitfinexApiResult<BitfinexPlacedOrder>> PlaceOrderAsync(string symbol,
            double amount,
            double price,
            OrderSide side,
            OrderType2 type,
            bool? hidden = null,
            bool? postOnly = null,
            bool? useAllAvailable = null,
            bool? ocoOrder = null,
            double? buyPriceOco = null,
            double? sellPriceOco = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "symbol", symbol },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) },
                { "exchange", "bitfinex" },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "type", JsonConvert.SerializeObject(type, new OrderType2Converter(false)) },
            };

            AddOptionalParameter(parameters, "is_hidden", hidden?.ToString());
            AddOptionalParameter(parameters, "is_postonly", postOnly?.ToString());
            AddOptionalParameter(parameters, "use_all_available", useAllAvailable != null ? JsonConvert.SerializeObject(useAllAvailable, new BoolToIntConverter(false)) : null);
            AddOptionalParameter(parameters, "ocoorder", ocoOrder?.ToString());
            AddOptionalParameter(parameters, "buy_price_oco", buyPriceOco?.ToString(CultureInfo.InvariantCulture));
            AddOptionalParameter(parameters, "sell_price_oco", sellPriceOco?.ToString(CultureInfo.InvariantCulture));

            return await ExecuteAuthenticatedRequestV1<BitfinexPlacedOrder>(GetUrl(NewOrderEndpoint, OldApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexBaseOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;
        public async Task<BitfinexApiResult<BitfinexBaseOrder>> CancelOrderAsync(long orderId)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"order_id", orderId},
            };
            return await ExecuteAuthenticatedRequestV1<BitfinexBaseOrder>(GetUrl(CancelOrderEndpoint, OldApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexResponseMessage> CancelAllOrders() => CancelAllOrdersAsync().Result;
        public async Task<BitfinexApiResult<BitfinexResponseMessage>> CancelAllOrdersAsync()
        {
            return await ExecuteAuthenticatedRequestV1<BitfinexResponseMessage>(GetUrl(CancelAllOrdersEndpoint, OldApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexBaseOrder> GetOrderStatus(long orderId) => GetOrderStatusAsync(orderId).Result;
        public async Task<BitfinexApiResult<BitfinexBaseOrder>> GetOrderStatusAsync(long orderId)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"order_id", orderId},
            };
            return await ExecuteAuthenticatedRequestV1<BitfinexBaseOrder>(GetUrl(OrderStatusEndpoint, OldApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexClaimedPosition> ClaimPosition(long positionId, double amount) => ClaimPositionAsync(positionId, amount).Result;
        public async Task<BitfinexApiResult<BitfinexClaimedPosition>> ClaimPositionAsync(long positionId, double amount)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"position_id", positionId},
                {"amount", amount},
            };
            return await ExecuteAuthenticatedRequestV1<BitfinexClaimedPosition>(GetUrl(ClaimPositionEndpoint, OldApiVersion), PostMethod, parameters);
        }
    }
}
