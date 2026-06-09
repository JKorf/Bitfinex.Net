using Bitfinex.Net.Clients.SpotApi;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using CryptoExchange.Net.Sockets.Default.Routing;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexUserSubscription : Subscription
    {
        private BitfinexSocketClientSpotApi _client;

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
            BitfinexSocketClientSpotApi client,
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
            _client = client;
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
                MessageRoute.CreateForEvent<BitfinexStringUpdate>("0hb", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexSocketPositionsEvent>("0ps",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexSocketPositionEvent>("0pn",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexSocketPositionEvent>("0pu",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexSocketPositionEvent>("0pc", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexBalanceEvent>("0bu", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexMarginBaseEvent>("0miubase", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexMarginSymbolEvent>("0miusym", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexFundingInfoEvent>("0fiu",DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexWalletsEvent>("0ws",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexWalletEvent>("0wu",DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexOrdersEvent>("0os",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexOrderEvent>("0on",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexOrderEvent>("0ou",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexOrderEvent>("0oc", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexTradeDetailEvent>("0te",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexTradeDetailEvent>("0tu", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexFundingTradeEvent>("0fte",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingTradeEvent>("0ftu",DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexOffersEvent>("0fos", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexOfferEvent>("0fon", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexOfferEvent>("0fou", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexOfferEvent>("0foc", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexFundingCreditsEvent>("0fcs", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingCreditEvent>("0fcn", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingCreditEvent>("0fcu", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingCreditEvent>("0fcc", DoHandleMessage),

                MessageRoute.CreateForEvent<BitfinexFundingsEvent>("0fls",DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingEvent>("0fln", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingEvent>("0flu", DoHandleMessage),
                MessageRoute.CreateForEvent<BitfinexFundingEvent>("0flc", DoHandleMessage),
                ]);

        }

        protected override Query? GetSubQuery(SocketConnection connection) => null;

        protected override Query? GetUnsubQuery(SocketConnection connection) => null;

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexStringUpdate message)
        {
            // Heartbeat
            connection.UpdateSequenceNumber(message.Sequence);
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexSocketPositionsEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _positionHandler?.Invoke(
                new DataEvent<BitfinexPosition[]>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("ps")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexSocketPositionEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _positionHandler?.Invoke(
                new DataEvent<BitfinexPosition[]>(BitfinexExchange.ExchangeName, [message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexBalanceEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _balanceHandler?.Invoke(
                new DataEvent<BitfinexBalance>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("bu")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexMarginBaseEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _marginBaseHandler?.Invoke(
                new DataEvent<BitfinexMarginBase>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("miu")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexMarginSymbolEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _marginSymbolHandler?.Invoke(
                new DataEvent<BitfinexMarginSymbol>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("miu")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingInfoEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingInfoHandler?.Invoke(
                new DataEvent<BitfinexFundingInfo>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId("fiu")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexWalletsEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _walletHandler?.Invoke(
                new DataEvent<BitfinexWallet[]>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("ws")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexWalletEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _walletHandler?.Invoke(
                new DataEvent<BitfinexWallet[]>(BitfinexExchange.ExchangeName, [message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId("wu")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOrdersEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _orderHandler?.Invoke(
                new DataEvent<BitfinexOrder[]>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("os")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOrderEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _orderHandler?.Invoke(
                new DataEvent<BitfinexOrder[]>(BitfinexExchange.ExchangeName, [message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexTradeDetailEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _tradeHandler?.Invoke(
                new DataEvent<BitfinexTradeDetails>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingTradeEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingTradeHandler?.Invoke(
                new DataEvent<BitfinexFundingTrade>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOffersEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingOfferHandler?.Invoke(
                new DataEvent<BitfinexFundingOffer[]>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("fos")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexOfferEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingOfferHandler?.Invoke(
                new DataEvent<BitfinexFundingOffer[]>(BitfinexExchange.ExchangeName, [message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingCreditsEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingCreditHandler?.Invoke(
                new DataEvent<BitfinexFundingCredit[]>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("fcs")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingCreditEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingCreditHandler?.Invoke(
                new DataEvent<BitfinexFundingCredit[]>(BitfinexExchange.ExchangeName, [message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingsEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingLoanHandler?.Invoke(
                new DataEvent<BitfinexFunding[]>(BitfinexExchange.ExchangeName, message.Data, receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Snapshot)
                    .WithStreamId("fls")
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexFundingEvent message)
        {
            connection.UpdateSequenceNumber(message.SequenceNumber);
            _client.UpdateTimeOffset(message.Timestamp);

            _fundingLoanHandler?.Invoke(
                new DataEvent<BitfinexFunding[]>(BitfinexExchange.ExchangeName, [message.Data], receiveTime, originalData)
                    .WithUpdateType(SocketUpdateType.Update)
                    .WithSymbol(message.Data.Symbol)
                    .WithStreamId(EnumConverter.GetString(message.EventType))
                    .WithDataTimestamp(message.Timestamp, _client.GetTimeOffset())
                );
            return CallResult.Ok();
        }
    }
}
