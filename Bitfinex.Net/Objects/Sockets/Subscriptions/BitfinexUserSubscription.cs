using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexUserSubscription : Subscription<BitfinexResponse, BitfinexResponse>
    {
        private static readonly MessagePath _messagePath = MessagePath.Get().Index(1);
        private static readonly MessagePath _marginInfoPath = MessagePath.Get().Index(2).Index(0);

        private readonly Action<DataEvent<IEnumerable<BitfinexPosition>>>? _positionHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexWallet>>>? _walletHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexOrder>>>? _orderHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexFundingOffer>>>? _fundingOfferHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexFundingCredit>>>? _fundingCreditHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexFunding>>>? _fundingLoanHandler;
        private readonly Action<DataEvent<BitfinexBalance>>? _balanceHandler;
        private readonly Action<DataEvent<BitfinexTradeDetails>>? _tradeHandler;
        private readonly Action<DataEvent<BitfinexFundingTrade>>? _fundingTradeHandler;
        private readonly Action<DataEvent<BitfinexMarginBase>>? _marginBaseHandler;
        private readonly Action<DataEvent<BitfinexMarginSymbol>>? _marginSymbolHandler;
        private readonly Action<DataEvent<BitfinexFundingInfo>>? _fundingInfoHandler;

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            var identifier = message.GetValue<string>(_messagePath);

            if (identifier == "hb")
                return typeof(BitfinexSocketEvent<string>);

            if (identifier == "ps")
                return typeof(BitfinexSocketEvent<List<BitfinexPosition>>);
            if (identifier == "pn" || identifier == "pu" || identifier == "pc")
                return typeof(BitfinexSocketEvent<BitfinexPosition>);

            if (identifier == "bu")
                return typeof(BitfinexSocketEvent<BitfinexBalance>);

            if (identifier == "miu")
            {
                var marginInfoType = message.GetValue<string>(_marginInfoPath);
                return marginInfoType == "base" ? typeof(BitfinexSocketEvent<BitfinexMarginBase>) : typeof(BitfinexSocketEvent<BitfinexMarginSymbol>);
            }

            if (identifier == "fiu")
                return typeof(BitfinexSocketEvent<BitfinexFundingInfo>);

            if (identifier == "ws")
                return typeof(BitfinexSocketEvent<List<BitfinexWallet>>);
            if (identifier == "wu")
                return typeof(BitfinexSocketEvent<BitfinexWallet>);

            if (identifier == "os")
                return typeof(BitfinexSocketEvent<List<BitfinexOrder>>);
            if (identifier == "on" || identifier == "ou" || identifier == "oc")
                return typeof(BitfinexSocketEvent<BitfinexOrder>);

            if (identifier == "te")
                return typeof(BitfinexSocketEvent<BitfinexTradeDetails>);
            if (identifier == "tu")
                return typeof(BitfinexSocketEvent<BitfinexTradeDetails>);

            if (identifier == "fte")
                return typeof(BitfinexSocketEvent<BitfinexFundingTrade>);
            if (identifier == "ftu")
                return typeof(BitfinexSocketEvent<BitfinexFundingTrade>);

            if (identifier == "fos")
                return typeof(BitfinexSocketEvent<List<BitfinexFundingOffer>>);
            if (identifier == "fon" || identifier == "fou" || identifier == "foc")
                return typeof(BitfinexSocketEvent<BitfinexFundingOffer>);

            if (identifier == "fcs")
                return typeof(BitfinexSocketEvent<List<BitfinexFundingCredit>>);
            if (identifier == "fcn" || identifier == "fcu" || identifier == "fcc")
                return typeof(BitfinexSocketEvent<BitfinexFundingCredit>);

            if (identifier == "fls")
                return typeof(BitfinexSocketEvent<List<BitfinexFunding>>);
            if (identifier == "fln" || identifier == "flu" || identifier == "flc")
                return typeof(BitfinexSocketEvent<BitfinexFunding>);

            return null;
        }

        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string>() { "0" };

        public BitfinexUserSubscription(ILogger logger, 
            Action<DataEvent<IEnumerable<BitfinexPosition>>>? positionHandler,
            Action<DataEvent<IEnumerable<BitfinexWallet>>>? walletHandler,
            Action<DataEvent<IEnumerable<BitfinexOrder>>>? orderHandler,
            Action<DataEvent<IEnumerable<BitfinexFundingOffer>>>? fundingOfferHandler,
            Action<DataEvent<IEnumerable<BitfinexFundingCredit>>>? fundingCreditHandler,
            Action<DataEvent<IEnumerable<BitfinexFunding>>>? fundingLoanHandler,
            Action<DataEvent<BitfinexBalance>>? balanceHandler,
            Action<DataEvent<BitfinexTradeDetails>>? tradeHandler,
            Action<DataEvent<BitfinexFundingTrade>>? fundingTradeHandler,
            Action<DataEvent<BitfinexFundingInfo>>? fundingInfoHandler,
            Action<DataEvent<BitfinexMarginBase>>? marginBaseHandler,
            Action<DataEvent<BitfinexMarginSymbol>>? marginSymbolHandler
            )
            : base(logger, true)
        {
            _positionHandler = positionHandler;
            _walletHandler = walletHandler;
            _orderHandler = orderHandler;
            _fundingOfferHandler = fundingOfferHandler;
            _fundingCreditHandler = fundingCreditHandler;
            _fundingLoanHandler = fundingLoanHandler;
            _balanceHandler = balanceHandler;
            _tradeHandler = tradeHandler;
            _fundingTradeHandler = fundingTradeHandler;
            _fundingInfoHandler = fundingInfoHandler;
            _marginBaseHandler = marginBaseHandler;
            _marginSymbolHandler = marginSymbolHandler;
        }

        public override Query? GetSubQuery(SocketConnection connection) => null;

        public override Query? GetUnsubQuery() => null;

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<object> message)
        {
            if (message.Data is BitfinexSocketEvent<List<BitfinexPosition>> positionSnapshot)
                _positionHandler?.Invoke(message.As<IEnumerable<BitfinexPosition>>(positionSnapshot.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexPosition> positionUpdate)
                _positionHandler?.Invoke(message.As<IEnumerable<BitfinexPosition>>(new[] { positionUpdate.Data }));

            else if (message.Data is BitfinexSocketEvent<List<BitfinexFunding>> loanSnapshot)
                _fundingLoanHandler?.Invoke(message.As<IEnumerable<BitfinexFunding>>(loanSnapshot.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexFunding> loanUpdate)
                _fundingLoanHandler?.Invoke(message.As<IEnumerable<BitfinexFunding>>(new[] { loanUpdate.Data }));

            else if (message.Data is BitfinexSocketEvent<List<BitfinexFundingCredit>> creditSnapshot)
                _fundingCreditHandler?.Invoke(message.As<IEnumerable<BitfinexFundingCredit>>(creditSnapshot.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexFundingCredit> creditUpdate)
                _fundingCreditHandler?.Invoke(message.As<IEnumerable<BitfinexFundingCredit>>(new[] { creditUpdate.Data }));

            else if (message.Data is BitfinexSocketEvent<List<BitfinexFundingOffer>> offerSnapshot)
                _fundingOfferHandler?.Invoke(message.As<IEnumerable<BitfinexFundingOffer>>(offerSnapshot.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexFundingOffer> offerUpdate)
                _fundingOfferHandler?.Invoke(message.As<IEnumerable<BitfinexFundingOffer>>(new[] { offerUpdate.Data }));

            else if (message.Data is BitfinexSocketEvent<List<BitfinexOrder>> orderSnapshot)
                _orderHandler?.Invoke(message.As<IEnumerable<BitfinexOrder>>(orderSnapshot.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexOrder> orderUpdate)
                _orderHandler?.Invoke(message.As<IEnumerable<BitfinexOrder>>(new[] { orderUpdate.Data }));

            else if (message.Data is BitfinexSocketEvent<BitfinexFundingTrade> fundingTrade)
                _fundingTradeHandler?.Invoke(message.As(fundingTrade.Data));

            else if (message.Data is BitfinexSocketEvent<BitfinexTradeDetails> trade)
                _tradeHandler?.Invoke(message.As(trade.Data));

            else if (message.Data is BitfinexSocketEvent<List<BitfinexWallet>> walletSnapshot)
                _walletHandler?.Invoke(message.As<IEnumerable<BitfinexWallet>>(walletSnapshot.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexWallet> walletUpdate)
                _walletHandler?.Invoke(message.As<IEnumerable<BitfinexWallet>>(new[] { walletUpdate.Data }));

            else if (message.Data is BitfinexSocketEvent<BitfinexBalance> balanceUpdate)
                _balanceHandler?.Invoke(message.As(balanceUpdate.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexFundingInfo> fundingInfoUpdate)
                _fundingInfoHandler?.Invoke(message.As(fundingInfoUpdate.Data));

            else if (message.Data is BitfinexSocketEvent<BitfinexMarginBase> marginBaseUpdate)
                _marginBaseHandler?.Invoke(message.As(marginBaseUpdate.Data));
            else if (message.Data is BitfinexSocketEvent<BitfinexMarginSymbol> marginSymbolUpdate)
                _marginSymbolHandler?.Invoke(message.As(marginSymbolUpdate.Data));

            return Task.FromResult(new CallResult(null));
        }
    }
}
