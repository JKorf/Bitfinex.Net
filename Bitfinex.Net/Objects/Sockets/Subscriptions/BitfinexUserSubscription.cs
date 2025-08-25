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

            MessageMatcher = MessageMatcher.Create([
                new MessageHandlerLink<BitfinexSocketPositionsEvent>("0ps", DoHandleMessage),
                new MessageHandlerLink<BitfinexSocketPositionEvent>("0pn", DoHandleMessage),
                new MessageHandlerLink<BitfinexSocketPositionEvent>("0pu", DoHandleMessage),
                new MessageHandlerLink<BitfinexSocketPositionEvent>("0pc", DoHandleMessage),

                new MessageHandlerLink<BitfinexBalanceEvent>("0bu", DoHandleMessage),

                new MessageHandlerLink<BitfinexMarginBaseEvent>("0miubase", DoHandleMessage),
                new MessageHandlerLink<BitfinexMarginSymbolEvent>("0miusym", DoHandleMessage),

                new MessageHandlerLink<BitfinexFundingInfoEvent>("0fiu", DoHandleMessage),

                new MessageHandlerLink<BitfinexWalletsEvent>("0ws", DoHandleMessage),
                new MessageHandlerLink<BitfinexWalletEvent>("0wu", DoHandleMessage),

                new MessageHandlerLink<BitfinexOrdersEvent>("0os", DoHandleMessage),
                new MessageHandlerLink<BitfinexOrderEvent>("0on", DoHandleMessage),
                new MessageHandlerLink<BitfinexOrderEvent>("0ou", DoHandleMessage),
                new MessageHandlerLink<BitfinexOrderEvent>("0oc", DoHandleMessage),

                new MessageHandlerLink<BitfinexTradeDetailEvent>("0te", DoHandleMessage),
                new MessageHandlerLink<BitfinexTradeDetailEvent>("0tu", DoHandleMessage),

                new MessageHandlerLink<BitfinexFundingTradeEvent>("0fte", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingTradeEvent>("0ftu", DoHandleMessage),

                new MessageHandlerLink<BitfinexOffersEvent>("0fos", DoHandleMessage),
                new MessageHandlerLink<BitfinexOfferEvent>("0fon", DoHandleMessage),
                new MessageHandlerLink<BitfinexOfferEvent>("0fou", DoHandleMessage),
                new MessageHandlerLink<BitfinexOfferEvent>("0foc", DoHandleMessage),

                new MessageHandlerLink<BitfinexFundingCreditsEvent>("0fcs", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingCreditEvent>("0fcn", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingCreditEvent>("0fcu", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingCreditEvent>("0fcc", DoHandleMessage),

                new MessageHandlerLink<BitfinexFundingsEvent>("0fls", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingEvent>("0fln", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingEvent>("0flu", DoHandleMessage),
                new MessageHandlerLink<BitfinexFundingEvent>("0flc", DoHandleMessage),
                ]);
        }

        protected override Query? GetSubQuery(SocketConnection connection) => null;

        protected override Query? GetUnsubQuery(SocketConnection connection) => null;

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexSocketPositionsEvent> message)
        {
            _positionHandler?.Invoke(message.As(message.Data.Data, "ps", null, SocketUpdateType.Snapshot).WithDataTimestamp(message.Data.Data.Any() ? message.Data.Data.Max(x => x.UpdateTime) : null));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexSocketPositionEvent> message)
        {
            _positionHandler?.Invoke(message.As(new[] { message.Data.Data }, EnumConverter.GetString(message.Data.EventType), message.Data.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.UpdateTime));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexBalanceEvent> message)
        {
            _balanceHandler?.Invoke(message.As(message.Data.Data, "bu", null, SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexMarginBaseEvent> message)
        {
            _marginBaseHandler?.Invoke(message.As(message.Data.Data, "miu", null, SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexMarginSymbolEvent> message)
        {
            _marginSymbolHandler?.Invoke(message.As(message.Data.Data, "miu", null, SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexFundingInfoEvent> message)
        {
            _fundingInfoHandler?.Invoke(message.As(message.Data.Data, "fiu", message.Data.Data.Symbol, SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexWalletsEvent> message)
        {
            _walletHandler?.Invoke(message.As(message.Data.Data, "ws", null, SocketUpdateType.Snapshot));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexWalletEvent> message)
        {
            _walletHandler?.Invoke(message.As(new[] { message.Data.Data }, "wu", null, SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexOrdersEvent> message)
        {
            _orderHandler?.Invoke(message.As(message.Data.Data, "os", null, SocketUpdateType.Snapshot).WithDataTimestamp(message.Data.Data.Any() ? message.Data.Data.Max(x => x.UpdateTime) : null));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexOrderEvent> message)
        {
            _orderHandler?.Invoke(message.As(new[] { message.Data.Data }, EnumConverter.GetString(message.Data.EventType), message.Data.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.UpdateTime));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexTradeDetailEvent> message)
        {
            _tradeHandler?.Invoke(message.As(message.Data.Data, EnumConverter.GetString(message.Data.EventType), message.Data.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.Timestamp));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexFundingTradeEvent> message)
        {
            _fundingTradeHandler?.Invoke(message.As(message.Data.Data, EnumConverter.GetString(message.Data.EventType), null, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.Timestamp));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexOffersEvent> message)
        {
            _fundingOfferHandler?.Invoke(message.As(message.Data.Data, "fos", null, SocketUpdateType.Snapshot).WithDataTimestamp(message.Data.Data.Any() ? message.Data.Data.Max(x => x.UpdateTime) : null));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexOfferEvent> message)
        {
            _fundingOfferHandler?.Invoke(message.As(new[] { message.Data.Data }, EnumConverter.GetString(message.Data.EventType), message.Data.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.UpdateTime));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexFundingCreditsEvent> message)
        {
            _fundingCreditHandler?.Invoke(message.As(message.Data.Data, "fcs", null, SocketUpdateType.Snapshot).WithDataTimestamp(message.Data.Data.Any() ? message.Data.Data.Max(x => x.UpdateTime) : null));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexFundingCreditEvent> message)
        {
            _fundingCreditHandler?.Invoke(message.As(new[] { message.Data.Data }, EnumConverter.GetString(message.Data.EventType), message.Data.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.UpdateTime));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexFundingsEvent> message)
        {
            _fundingLoanHandler?.Invoke(message.As(message.Data.Data, "fls", null, SocketUpdateType.Snapshot).WithDataTimestamp(message.Data.Data.Any() ? message.Data.Data.Max(x => x.UpdateTime) : null));
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexFundingEvent> message)
        {
            _fundingLoanHandler?.Invoke(message.As(new[] { message.Data.Data }, EnumConverter.GetString(message.Data.EventType), message.Data.Data.Symbol, SocketUpdateType.Update).WithDataTimestamp(message.Data.Data.UpdateTime));
            return CallResult.SuccessResult;
        }
    }
}
