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
                MessageRoute<BitfinexStringUpdate>.CreateWithoutTopicFilter("0hb", DoHandleMessage),

                MessageRoute<BitfinexSocketPositionsEvent>.CreateWithoutTopicFilter("0ps",DoHandleMessage),
                MessageRoute<BitfinexSocketPositionEvent>.CreateWithoutTopicFilter("0pn",DoHandleMessage),
                MessageRoute<BitfinexSocketPositionEvent>.CreateWithoutTopicFilter("0pu",DoHandleMessage),
                MessageRoute<BitfinexSocketPositionEvent>.CreateWithoutTopicFilter("0pc", DoHandleMessage),

                MessageRoute<BitfinexBalanceEvent>.CreateWithoutTopicFilter("0bu", DoHandleMessage),

                MessageRoute<BitfinexMarginBaseEvent>.CreateWithoutTopicFilter("0miubase", DoHandleMessage),
                MessageRoute<BitfinexMarginSymbolEvent>.CreateWithoutTopicFilter("0miusym", DoHandleMessage),

                MessageRoute<BitfinexFundingInfoEvent>.CreateWithoutTopicFilter("0fiu",DoHandleMessage),

                MessageRoute<BitfinexWalletsEvent>.CreateWithoutTopicFilter("0ws",DoHandleMessage),
                MessageRoute<BitfinexWalletEvent>.CreateWithoutTopicFilter("0wu",DoHandleMessage),

                MessageRoute<BitfinexOrdersEvent>.CreateWithoutTopicFilter("0os",DoHandleMessage),
                MessageRoute<BitfinexOrderEvent>.CreateWithoutTopicFilter("0on",DoHandleMessage),
                MessageRoute<BitfinexOrderEvent>.CreateWithoutTopicFilter("0ou",DoHandleMessage),
                MessageRoute<BitfinexOrderEvent>.CreateWithoutTopicFilter("0oc", DoHandleMessage),

                MessageRoute<BitfinexTradeDetailEvent>.CreateWithoutTopicFilter("0te",DoHandleMessage),
                MessageRoute<BitfinexTradeDetailEvent>.CreateWithoutTopicFilter("0tu", DoHandleMessage),

                MessageRoute<BitfinexFundingTradeEvent>.CreateWithoutTopicFilter("0fte",DoHandleMessage),
                MessageRoute<BitfinexFundingTradeEvent>.CreateWithoutTopicFilter("0ftu",DoHandleMessage),

                MessageRoute<BitfinexOffersEvent>.CreateWithoutTopicFilter("0fos", DoHandleMessage),
                MessageRoute<BitfinexOfferEvent>.CreateWithoutTopicFilter("0fon", DoHandleMessage),
                MessageRoute<BitfinexOfferEvent>.CreateWithoutTopicFilter("0fou", DoHandleMessage),
                MessageRoute<BitfinexOfferEvent>.CreateWithoutTopicFilter("0foc", DoHandleMessage),

                MessageRoute<BitfinexFundingCreditsEvent>.CreateWithoutTopicFilter("0fcs", DoHandleMessage),
                MessageRoute<BitfinexFundingCreditEvent>.CreateWithoutTopicFilter("0fcn", DoHandleMessage),
                MessageRoute<BitfinexFundingCreditEvent>.CreateWithoutTopicFilter("0fcu", DoHandleMessage),
                MessageRoute<BitfinexFundingCreditEvent>.CreateWithoutTopicFilter("0fcc", DoHandleMessage),

                MessageRoute<BitfinexFundingsEvent>.CreateWithoutTopicFilter("0fls",DoHandleMessage),
                MessageRoute<BitfinexFundingEvent>.CreateWithoutTopicFilter("0fln", DoHandleMessage),
                MessageRoute<BitfinexFundingEvent>.CreateWithoutTopicFilter("0flu", DoHandleMessage),
                MessageRoute<BitfinexFundingEvent>.CreateWithoutTopicFilter("0flc", DoHandleMessage),
                ]);

        }

        protected override Query? GetSubQuery(SocketConnection connection) => null;

        protected override Query? GetUnsubQuery(SocketConnection connection) => null;

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexStringUpdate message)
        {
            // Heartbeat
            connection.UpdateSequenceNumber(message.Sequence);
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
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
            return CallResult.SuccessResult;
        }
    }
}
