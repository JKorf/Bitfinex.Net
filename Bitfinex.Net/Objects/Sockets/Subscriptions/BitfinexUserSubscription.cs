using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexUserSubscription : Subscription<BitfinexResponse, BitfinexResponse>
    {
        private readonly Action<DataEvent<IEnumerable<BitfinexPosition>>> _positionHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexWallet>>> _walletHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexOrder>>> _orderHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexFundingOffer>>> _fundingOfferHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexFundingCredit>>> _fundingCreditHandler;
        private readonly Action<DataEvent<IEnumerable<BitfinexFunding>>> _fundingLoanHandler;
        private readonly Action<DataEvent<BitfinexBalance>> _balanceHandler;
        private readonly Action<DataEvent<BitfinexTradeDetails>> _tradeHandler;
        private readonly Action<DataEvent<BitfinexFundingTrade>> _fundingTradeHandler;
        private readonly Action<DataEvent<BitfinexMarginBase>> _marginInfoHandler; // TODO
        private readonly Action<DataEvent<BitfinexFundingInfo>> _fundingInfoHandler;

        public override Dictionary<string, Type> TypeMapping { get; } = new Dictionary<string, Type>
        {
            { "hb-single", typeof(BitfinexSocketEvent<string>) },

            { "ps-array", typeof(BitfinexSocketEvent<List<BitfinexPosition>>) },
            { "pn-single", typeof(BitfinexSocketEvent<BitfinexPosition>) },
            { "pu-single", typeof(BitfinexSocketEvent<BitfinexPosition>) },
            { "pc-single", typeof(BitfinexSocketEvent<BitfinexPosition>) },

            { "bu-single", typeof(BitfinexSocketEvent<BitfinexBalance>) },

            { "miu-single", typeof(BitfinexSocketEvent<string>) },

            { "fiu-single", typeof(BitfinexSocketEvent<BitfinexFundingInfo>) },

            { "ws-array", typeof(BitfinexSocketEvent<List<BitfinexWallet>>) },
            { "wu-single", typeof(BitfinexSocketEvent<BitfinexWallet>) },

            { "os-array", typeof(BitfinexSocketEvent<List<BitfinexOrder>>) },
            { "on-single", typeof(BitfinexSocketEvent<BitfinexOrder>) },
            { "ou-single", typeof(BitfinexSocketEvent<BitfinexOrder>) },
            { "oc-single", typeof(BitfinexSocketEvent<BitfinexOrder>) },

            { "te-single", typeof(BitfinexSocketEvent<BitfinexTradeDetails>) },
            { "tu-single", typeof(BitfinexSocketEvent<BitfinexTradeDetails>) },

            { "fte-single", typeof(BitfinexSocketEvent<BitfinexFundingTrade>) },
            { "ftu-single", typeof(BitfinexSocketEvent<BitfinexFundingTrade>) },

            { "fos-array", typeof(BitfinexSocketEvent<List<BitfinexFundingOffer>>) },
            { "fon-single", typeof(BitfinexSocketEvent<BitfinexFundingOffer>) },
            { "fou-single", typeof(BitfinexSocketEvent<BitfinexFundingOffer>) },
            { "foc-single", typeof(BitfinexSocketEvent<BitfinexFundingOffer>) },

            { "fcs-array", typeof(BitfinexSocketEvent<List<BitfinexFundingCredit>>) },
            { "fcc-single", typeof(BitfinexSocketEvent<BitfinexFundingCredit>) },
            { "fcn-single", typeof(BitfinexSocketEvent<BitfinexFundingCredit>) },
            { "fcu-single", typeof(BitfinexSocketEvent<BitfinexFundingCredit>) },

            { "fls-array", typeof(BitfinexSocketEvent<List<BitfinexFunding>>) },
            { "flc-single", typeof(BitfinexSocketEvent<BitfinexFunding>) },
            { "fln-single", typeof(BitfinexSocketEvent<BitfinexFunding>) },
            { "flu-single", typeof(BitfinexSocketEvent<BitfinexFunding>) },
        };

        public override List<string> StreamIdentifiers { get; } = new List<string>() { "0" };

        public BitfinexUserSubscription(ILogger logger, 
            Action<DataEvent<IEnumerable<BitfinexPosition>>> positionHandler,
            Action<DataEvent<IEnumerable<BitfinexWallet>>> walletHandler,
            Action<DataEvent<IEnumerable<BitfinexOrder>>> orderHandler,
            Action<DataEvent<IEnumerable<BitfinexFundingOffer>>> fundingOfferHandler,
            Action<DataEvent<IEnumerable<BitfinexFundingCredit>>> fundingCreditHandler,
            Action<DataEvent<IEnumerable<BitfinexFunding>>> fundingLoanHandler,
            Action<DataEvent<BitfinexBalance>> balanceHandler,
            Action<DataEvent<BitfinexTradeDetails>> tradeHandler,
            Action<DataEvent<BitfinexFundingTrade>> fundingTradeHandler,
            Action<DataEvent<BitfinexFundingInfo>> fundingInfoHandler
            //Action<DataEvent<BitfinexMarginBase>> marginInfoHandler
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
        }

        public override BaseQuery? GetSubQuery(SocketConnection connection) => null;

        public override BaseQuery? GetUnsubQuery() => null;

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<BaseParsedMessage> message)
        {
            Debug.WriteLine($"{message.Data.Data.GetType()}; {message.Data.OriginalData}");

            return message.Data.TypeIdentifier switch
            {
                "hb-single" => Task.FromResult(new CallResult(null)),

                "ps-array" => InvokeAndReturnSnapshot(_positionHandler, message),
                "pn-single" => InvokeAndReturnUpdate(_positionHandler, message),
                "pu-single" => InvokeAndReturnUpdate(_positionHandler, message),
                "pc-single" => InvokeAndReturnUpdate(_positionHandler, message),

                "bu-single" => InvokeAndReturnSingleUpdate(_balanceHandler, message),

                "fiu-single" => InvokeAndReturnSingleUpdate(_fundingInfoHandler, message),

                "ws-array" => InvokeAndReturnSnapshot(_walletHandler, message),
                "wu-single" => InvokeAndReturnUpdate(_walletHandler, message),

                "os-array" => InvokeAndReturnSnapshot(_orderHandler, message),
                "on-single" => InvokeAndReturnUpdate(_orderHandler, message),
                "ou-single" => InvokeAndReturnUpdate(_orderHandler, message),
                "oc-single" => InvokeAndReturnUpdate(_orderHandler, message),

                "te-single" => InvokeAndReturnSingleUpdate(_tradeHandler, message),
                "tu-single" => InvokeAndReturnSingleUpdate(_tradeHandler, message),

                "fte-single" => InvokeAndReturnSingleUpdate(_fundingTradeHandler, message),
                "ftu-single" => InvokeAndReturnSingleUpdate(_fundingTradeHandler, message),

                "fos-array" => InvokeAndReturnSnapshot(_fundingOfferHandler, message),
                "fon-single" => InvokeAndReturnUpdate(_fundingOfferHandler, message),
                "fou-single" => InvokeAndReturnUpdate(_fundingOfferHandler, message),
                "foc-single" => InvokeAndReturnUpdate(_fundingOfferHandler, message),

                "fcs-array" => InvokeAndReturnSnapshot(_fundingCreditHandler, message),
                "fcn-single" => InvokeAndReturnUpdate(_fundingCreditHandler, message),
                "fcu-single" => InvokeAndReturnUpdate(_fundingCreditHandler, message),
                "fcc-single" => InvokeAndReturnUpdate(_fundingCreditHandler, message),

                "fls-array" => InvokeAndReturnSnapshot(_fundingLoanHandler, message),
                "fln-single" => InvokeAndReturnUpdate(_fundingLoanHandler, message),
                "flu-single" => InvokeAndReturnUpdate(_fundingLoanHandler, message),
                "flc-single" => InvokeAndReturnUpdate(_fundingLoanHandler, message),
                _ => throw new NotImplementedException()
            };
        }

        private Task<CallResult> InvokeAndReturnSnapshot<T>(Action<DataEvent<IEnumerable<T>>> handler, DataEvent<BaseParsedMessage> message)
        {
            var data = (BitfinexSocketEvent<List<T>>)message.Data.Data;
            handler?.Invoke(message.As<IEnumerable<T>>(data.Data, null, SocketUpdateType.Snapshot));
            return Task.FromResult(new CallResult(null));
        }

        private Task<CallResult> InvokeAndReturnUpdate<T>(Action<DataEvent<IEnumerable<T>>> handler, DataEvent<BaseParsedMessage> message)
        {
            var data = (BitfinexSocketEvent<T>)message.Data.Data;
            handler?.Invoke(message.As<IEnumerable<T>>(new[] { data.Data }, null, SocketUpdateType.Update));
            return Task.FromResult(new CallResult(null));
        }

        private Task<CallResult> InvokeAndReturnSingleUpdate<T>(Action<DataEvent<T>> handler, DataEvent<BaseParsedMessage> message)
        {
            var data = (BitfinexSocketEvent<T>)message.Data.Data;
            handler?.Invoke(message.As(data.Data, null, SocketUpdateType.Update));
            return Task.FromResult(new CallResult(null));
        }
    }
}
