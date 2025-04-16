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
using System.Linq;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexUserSubscription : Subscription<BitfinexResponse, BitfinexResponse>
    {
        private static readonly MessagePath _messagePath = MessagePath.Get().Index(1);
        private static readonly MessagePath _marginInfoPath = MessagePath.Get().Index(2).Index(0);

        private readonly Action<DataEvent<BitfinexPosition[]>>? _positionHandler;
        private readonly Action<DataEvent<BitfinexWallet[]>>? _walletHandler;
        private readonly Action<DataEvent<BitfinexOrder[]>>? _orderHandler;
        private readonly Action<DataEvent<BitfinexFundingOffer[]>>? _fundingOfferHandler;
        private readonly Action<DataEvent<BitfinexFundingCredit[]>>? _fundingCreditHandler;
        private readonly Action<DataEvent<BitfinexFunding[]>>? _fundingLoanHandler;
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

            if (string.Equals(identifier, "hb", StringComparison.Ordinal))
                return typeof(BitfinexSocketStringEvent);

            if (string.Equals(identifier, "ps", StringComparison.Ordinal))
                return typeof(BitfinexSocketPositionsEvent);
            if (string.Equals(identifier, "pn", StringComparison.Ordinal)
                || string.Equals(identifier, "pu", StringComparison.Ordinal)
                || string.Equals(identifier, "pc", StringComparison.Ordinal))
            {
                return typeof(BitfinexSocketPositionEvent);
            }

            if (string.Equals(identifier, "bu", StringComparison.Ordinal))
                return typeof(BitfinexBalanceEvent);

            if (string.Equals(identifier, "miu", StringComparison.Ordinal))
            {
                var marginInfoType = message.GetValue<string>(_marginInfoPath);
                return string.Equals(marginInfoType, "base", StringComparison.Ordinal) ? typeof(BitfinexMarginBaseEvent) : typeof(BitfinexMarginSymbolEvent);
            }

            if (string.Equals(identifier, "fiu", StringComparison.Ordinal))
                return typeof(BitfinexFundingInfoEvent);

            if (string.Equals(identifier, "ws", StringComparison.Ordinal))
                return typeof(BitfinexWalletsEvent);
            if (string.Equals(identifier, "wu", StringComparison.Ordinal))
                return typeof(BitfinexWalletEvent);

            if (string.Equals(identifier, "os", StringComparison.Ordinal))
                return typeof(BitfinexOrdersEvent);
            if (string.Equals(identifier, "on", StringComparison.Ordinal)
                || string.Equals(identifier, "ou", StringComparison.Ordinal) 
                || string.Equals(identifier, "oc", StringComparison.Ordinal))
            {
                return typeof(BitfinexOrderEvent);
            }

            if (string.Equals(identifier, "te", StringComparison.Ordinal))
                return typeof(BitfinexTradeDetailEvent);
            if (string.Equals(identifier, "tu", StringComparison.Ordinal))
                return typeof(BitfinexTradeDetailEvent);

            if (string.Equals(identifier, "fte", StringComparison.Ordinal))
                return typeof(BitfinexFundingTradeEvent);
            if (string.Equals(identifier, "ftu", StringComparison.Ordinal))
                return typeof(BitfinexFundingTradeEvent);

            if (string.Equals(identifier, "fos", StringComparison.Ordinal))
                return typeof(BitfinexOffersEvent);
            if (string.Equals(identifier, "fon", StringComparison.Ordinal)
                || string.Equals(identifier, "fou", StringComparison.Ordinal)
                || string.Equals(identifier, "foc", StringComparison.Ordinal))
            {
                return typeof(BitfinexOfferEvent);
            }

            if (string.Equals(identifier, "fcs", StringComparison.Ordinal))
                return typeof(BitfinexFundingCreditsEvent);
            if (string.Equals(identifier, "fcn", StringComparison.Ordinal)
                || string.Equals(identifier, "fcu", StringComparison.Ordinal)
                || string.Equals(identifier, "fcc", StringComparison.Ordinal))
            {
                return typeof(BitfinexFundingCreditEvent);
            }

            if (string.Equals(identifier, "fls", StringComparison.Ordinal))
                return typeof(BitfinexFundingsEvent);
            if (string.Equals(identifier, "fln", StringComparison.Ordinal)
                || string.Equals(identifier, "flu", StringComparison.Ordinal)
                || string.Equals(identifier, "flc", StringComparison.Ordinal))
            {
                return typeof(BitfinexFundingEvent);
            }

            return null;
        }

        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string>() { "0" };

        public BitfinexUserSubscription(ILogger logger,
            Action<DataEvent<BitfinexPosition[]>>? positionHandler,
            Action<DataEvent<BitfinexWallet[]>>? walletHandler,
            Action<DataEvent<BitfinexOrder[]>>? orderHandler,
            Action<DataEvent<BitfinexFundingOffer[]>>? fundingOfferHandler,
            Action<DataEvent<BitfinexFundingCredit[]>>? fundingCreditHandler,
            Action<DataEvent<BitfinexFunding[]>>? fundingLoanHandler,
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

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            if (message.Data is BitfinexSocketPositionsEvent positionSnapshot)
                _positionHandler?.Invoke(message.As(positionSnapshot.Data, "ps", null, SocketUpdateType.Snapshot).WithDataTimestamp(positionSnapshot.Data.Max(x => x.UpdateTime)));
            else if (message.Data is BitfinexSocketPositionEvent positionUpdate)
                _positionHandler?.Invoke(message.As(new[] { positionUpdate.Data }, EnumConverter.GetString(positionUpdate.EventType), positionUpdate.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(positionUpdate.Data.UpdateTime));

            else if (message.Data is BitfinexFundingsEvent loanSnapshot)
                _fundingLoanHandler?.Invoke(message.As(loanSnapshot.Data, "fls", null, SocketUpdateType.Snapshot).WithDataTimestamp(loanSnapshot.Data.Max(x => x.UpdateTime)));
            else if (message.Data is BitfinexFundingEvent loanUpdate)
                _fundingLoanHandler?.Invoke(message.As(new[] { loanUpdate.Data }, EnumConverter.GetString(loanUpdate.EventType), loanUpdate.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(loanUpdate.Data.UpdateTime));

            else if (message.Data is BitfinexFundingCreditsEvent creditSnapshot)
                _fundingCreditHandler?.Invoke(message.As(creditSnapshot.Data, "fcs", null, SocketUpdateType.Snapshot).WithDataTimestamp(creditSnapshot.Data.Max(x => x.UpdateTime)));
            else if (message.Data is BitfinexFundingCreditEvent creditUpdate)
                _fundingCreditHandler?.Invoke(message.As(new[] { creditUpdate.Data }, EnumConverter.GetString(creditUpdate.EventType), creditUpdate.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(creditUpdate.Data.UpdateTime));

            else if (message.Data is BitfinexOffersEvent offerSnapshot)
                _fundingOfferHandler?.Invoke(message.As(offerSnapshot.Data, "fos", null, SocketUpdateType.Snapshot).WithDataTimestamp(offerSnapshot.Data.Max(x => x.UpdateTime)));
            else if (message.Data is BitfinexOfferEvent offerUpdate)
                _fundingOfferHandler?.Invoke(message.As(new[] { offerUpdate.Data }, EnumConverter.GetString(offerUpdate.EventType), offerUpdate.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(offerUpdate.Data.UpdateTime));

            else if (message.Data is BitfinexOrdersEvent orderSnapshot)
                _orderHandler?.Invoke(message.As(orderSnapshot.Data, "os", null, SocketUpdateType.Snapshot).WithDataTimestamp(orderSnapshot.Data.Max(x => x.UpdateTime)));
            else if (message.Data is BitfinexOrderEvent orderUpdate)
                _orderHandler?.Invoke(message.As(new[] { orderUpdate.Data }, EnumConverter.GetString(orderUpdate.EventType), orderUpdate.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(orderUpdate.Data.UpdateTime));

            else if (message.Data is BitfinexFundingTradeEvent fundingTrade)
                _fundingTradeHandler?.Invoke(message.As(fundingTrade.Data, EnumConverter.GetString(fundingTrade.EventType), null, SocketUpdateType.Update).WithDataTimestamp(fundingTrade.Data.Timestamp));

            else if (message.Data is BitfinexTradeDetailEvent trade)
                _tradeHandler?.Invoke(message.As(trade.Data, EnumConverter.GetString(trade.EventType), trade.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(trade.Data.Timestamp));

            else if (message.Data is BitfinexWalletsEvent walletSnapshot)
                _walletHandler?.Invoke(message.As(walletSnapshot.Data, "ws", null, SocketUpdateType.Snapshot));
            else if (message.Data is BitfinexWalletEvent walletUpdate)
                _walletHandler?.Invoke(message.As(new[] { walletUpdate.Data }, "wu", null, SocketUpdateType.Update));

            else if (message.Data is BitfinexBalanceEvent balanceUpdate)
                _balanceHandler?.Invoke(message.As(balanceUpdate.Data, "bu", null, SocketUpdateType.Update));
            else if (message.Data is BitfinexFundingInfoEvent fundingInfoUpdate)
                _fundingInfoHandler?.Invoke(message.As(fundingInfoUpdate.Data, "fiu", fundingInfoUpdate.Data.Symbol, SocketUpdateType.Update));

            else if (message.Data is BitfinexMarginBaseEvent marginBaseUpdate)
                _marginBaseHandler?.Invoke(message.As(marginBaseUpdate.Data, "miu", null, SocketUpdateType.Update));
            else if (message.Data is BitfinexMarginSymbolEvent marginSymbolUpdate)
                _marginSymbolHandler?.Invoke(message.As(marginSymbolUpdate.Data, "miu", marginSymbolUpdate.Data.Symbol, SocketUpdateType.Update));

            return CallResult.SuccessResult;
        }
    }
}
