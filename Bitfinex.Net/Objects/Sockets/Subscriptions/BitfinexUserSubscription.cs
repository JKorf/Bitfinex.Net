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

            MessageRouter = MessageRouter.Create([
                new MessageRoute<BitfinexSocketPositionsEvent>("0ps", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexSocketPositionEvent>("0pn", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexSocketPositionEvent>("0pu", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexSocketPositionEvent>("0pc",(string?)null, DoHandleMessage),

                new MessageRoute<BitfinexBalanceEvent>("0bu",(string?)null, DoHandleMessage),

                new MessageRoute<BitfinexMarginBaseEvent>("0miubase",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexMarginSymbolEvent>("0miusym",(string?)null, DoHandleMessage),

                new MessageRoute<BitfinexFundingInfoEvent>("0fiu", (string?)null,DoHandleMessage),

                new MessageRoute<BitfinexWalletsEvent>("0ws", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexWalletEvent>("0wu", (string?)null,DoHandleMessage),

                new MessageRoute<BitfinexOrdersEvent>("0os", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexOrderEvent>("0on", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexOrderEvent>("0ou", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexOrderEvent>("0oc",(string?)null, DoHandleMessage),

                new MessageRoute<BitfinexTradeDetailEvent>("0te", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexTradeDetailEvent>("0tu",(string?)null, DoHandleMessage),

                new MessageRoute<BitfinexFundingTradeEvent>("0fte", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexFundingTradeEvent>("0ftu", (string?)null,DoHandleMessage),

                new MessageRoute<BitfinexOffersEvent>("0fos",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexOfferEvent>("0fon",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexOfferEvent>("0fou",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexOfferEvent>("0foc", (string?)null,DoHandleMessage),

                new MessageRoute<BitfinexFundingCreditsEvent>("0fcs",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexFundingCreditEvent>("0fcn",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexFundingCreditEvent>("0fcu",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexFundingCreditEvent>("0fcc",(string?)null, DoHandleMessage),

                new MessageRoute<BitfinexFundingsEvent>("0fls", (string?)null,DoHandleMessage),
                new MessageRoute<BitfinexFundingEvent>("0fln",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexFundingEvent>("0flu",(string?)null, DoHandleMessage),
                new MessageRoute<BitfinexFundingEvent>("0flc", (string?)null, DoHandleMessage),
                ]);

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

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexSocketPositionsEvent message)
        {
            _positionHandler?.Invoke(
                new DataEvent<BitfinexPosition[]>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("ps")
                    .WithDataTimestamp(message.Data.Any() ? message.Data.Max(x => x.UpdateTime) : null)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexSocketPositionEvent message)
        {
            _positionHandler?.Invoke(
                new DataEvent<BitfinexPosition[]>([message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.UpdateTime)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexBalanceEvent message)
        {
            _balanceHandler?.Invoke(
                new DataEvent<BitfinexBalance>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("bu")
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexMarginBaseEvent message)
        {
            _marginBaseHandler?.Invoke(
                new DataEvent<BitfinexMarginBase>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("miu")
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexMarginSymbolEvent message)
        {
            _marginSymbolHandler?.Invoke(
                new DataEvent<BitfinexMarginSymbol>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("miu")
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingInfoEvent message)
        {
            _fundingInfoHandler?.Invoke(
                new DataEvent<BitfinexFundingInfo>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId("fiu")
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexWalletsEvent message)
        {
            _walletHandler?.Invoke(
                new DataEvent<BitfinexWallet[]>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("ws")
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexWalletEvent message)
        {
            _walletHandler?.Invoke(
                new DataEvent<BitfinexWallet[]>([message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("wu")
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOrdersEvent message)
        {
            _orderHandler?.Invoke(
                new DataEvent<BitfinexOrder[]>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("os")
                    .WithDataTimestamp(message.Data.Any() ? message.Data.Max(x => x.UpdateTime) : null)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOrderEvent message)
        {
            _orderHandler?.Invoke(
                new DataEvent<BitfinexOrder[]>([message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.UpdateTime)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexTradeDetailEvent message)
        {
            _tradeHandler?.Invoke(
                new DataEvent<BitfinexTradeDetails>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.Timestamp)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingTradeEvent message)
        {
            _fundingTradeHandler?.Invoke(
                new DataEvent<BitfinexFundingTrade>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.Timestamp)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOffersEvent message)
        {
            _fundingOfferHandler?.Invoke(
                new DataEvent<BitfinexFundingOffer[]>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("fos")
                    .WithDataTimestamp(message.Data.Any() ? message.Data.Max(x => x.UpdateTime) : null)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOfferEvent message)
        {
            _fundingOfferHandler?.Invoke(
                new DataEvent<BitfinexFundingOffer[]>([message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.UpdateTime)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingCreditsEvent message)
        {
            _fundingCreditHandler?.Invoke(
                new DataEvent<BitfinexFundingCredit[]>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("fcs")
                    .WithDataTimestamp(message.Data.Any() ? message.Data.Max(x => x.UpdateTime) : null)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingCreditEvent message)
        {
            _fundingCreditHandler?.Invoke(
                new DataEvent<BitfinexFundingCredit[]>([message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.UpdateTime)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingsEvent message)
        {
            _fundingLoanHandler?.Invoke(
                new DataEvent<BitfinexFunding[]>(message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("fls")
                    .WithDataTimestamp(message.Data.Any() ? message.Data.Max(x => x.UpdateTime) : null)
                );
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingEvent message)
        {
            _fundingLoanHandler?.Invoke(
                new DataEvent<BitfinexFunding[]>([message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Data.UpdateTime)
                );
            return CallResult.SuccessResult;
        }
    }
}
