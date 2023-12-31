using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
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
        private bool _firstUpdate;

        public override Dictionary<string, Type> TypeMapping { get; } = new Dictionary<string, Type>
        {
            { "hb-single", typeof(BitfinexUpdate3<string>) },

            { "ps-array", typeof(BitfinexUpdate3<IEnumerable<BitfinexPosition>>) },
            { "pn-single", typeof(BitfinexUpdate3<BitfinexPosition>) },
            { "pu-single", typeof(BitfinexUpdate3<BitfinexPosition>) },
            { "pc-single", typeof(BitfinexUpdate3<BitfinexPosition>) },

            { "bu-single", typeof(BitfinexUpdate3<IEnumerable<decimal>>) },

            { "miu-single", typeof(BitfinexUpdate3<string>) },

            { "fiu-single", typeof(BitfinexUpdate3<string>) },

            { "ws-array", typeof(BitfinexUpdate3<IEnumerable<BitfinexWallet>>) },
            { "wu-single", typeof(BitfinexUpdate3<BitfinexWallet>) },

            { "os-array", typeof(BitfinexUpdate3<IEnumerable<BitfinexOrder>>) },
            { "on-single", typeof(BitfinexUpdate3<BitfinexOrder>) },
            { "ou-single", typeof(BitfinexUpdate3<BitfinexOrder>) },
            { "oc-single", typeof(BitfinexUpdate3<BitfinexOrder>) },

            { "te-single", typeof(BitfinexUpdate3<BitfinexTradeDetails>) },
            { "tu-single", typeof(BitfinexUpdate3<BitfinexTradeDetails>) },

            { "fte-single", typeof(BitfinexUpdate3<string>) },
            { "ftu-single", typeof(BitfinexUpdate3<string>) },

            { "fos-array", typeof(BitfinexUpdate3<IEnumerable<BitfinexFundingOffer>>) },
            { "fon-single", typeof(BitfinexUpdate3<BitfinexFundingOffer>) },
            { "fou-single", typeof(BitfinexUpdate3<BitfinexFundingOffer>) },
            { "foc-single", typeof(BitfinexUpdate3<BitfinexFundingOffer>) },

            { "fcs-array", typeof(BitfinexUpdate3<IEnumerable<BitfinexFundingCredit>>) },
            { "fcc-single", typeof(BitfinexUpdate3<BitfinexFundingCredit>) },
            { "fcn-single", typeof(BitfinexUpdate3<BitfinexFundingCredit>) },
            { "fcu-single", typeof(BitfinexUpdate3<BitfinexFundingCredit>) },

            { "fls-array", typeof(BitfinexUpdate3<IEnumerable<BitfinexFunding>>) },
            { "flc-single", typeof(BitfinexUpdate3<BitfinexFunding>) },
            { "fln-single", typeof(BitfinexUpdate3<BitfinexFunding>) },
            { "flu-single", typeof(BitfinexUpdate3<BitfinexFunding>) },
        };

        public override List<string> StreamIdentifiers { get; } = new List<string>() { "0" };

        public BitfinexUserSubscription(ILogger logger)
            : base(logger, true)
        {
        }

        public override BaseQuery? GetSubQuery(SocketConnection connection) => null;

        public override BaseQuery? GetUnsubQuery() => null;

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<BaseParsedMessage> message)
        {
            Debug.WriteLine($"{message.Data.Data.GetType()}; {message.Data.OriginalData}");

            if (message.Data.TypeIdentifier == "hb-single")
                return Task.FromResult(new CallResult(null));
            //if (message.Data.TypeIdentifier == "bu-single")
            //    return Task.FromResult(new CallResult(null));
            //if (message.Data.TypeIdentifier == "ws-array")
            //    return Task.FromResult(new CallResult(null));
            //if (message.Data.TypeIdentifier == "wu-single")
            //    return Task.FromResult(new CallResult(null));

            return Task.FromResult(new CallResult(null));
        }
    }
}
