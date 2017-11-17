using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;

namespace Bitfinex.Net.Errors
{
    internal static class BitfinexErrors
    {
        internal static Dictionary<BitfinexErrorKey, BitfinexError> ErrorRegistrations =
            new Dictionary<BitfinexErrorKey, BitfinexError>()
            {
                { BitfinexErrorKey.NoApiCredentialsProvided, new BitfinexError(5000, "No api credentials provided, can't request private endpoints")},
                { BitfinexErrorKey.InputValidationFailed, new BitfinexError(5001, "A provided parameter was incorrect")},

                { BitfinexErrorKey.ErrorWeb, new BitfinexError(6001, "Server returned a not successful status")},
                { BitfinexErrorKey.CantConnectToServer, new BitfinexError(6002, "Could not connect to Bittrex server")},

                { BitfinexErrorKey.WithdrawFailed, new BitfinexError(6003, "Withdraw returned an error")},
                { BitfinexErrorKey.DepositAddressFailed, new BitfinexError(6004, "Get deposit address returned an error")},

                { BitfinexErrorKey.SubscriptionNotConfirmed, new BitfinexError(6005, "Subscription not confirmed by the server")},

                { BitfinexErrorKey.ParseErrorReader, new BitfinexError(7000, "Error reading the returned data. Data was not valid Json")},
                { BitfinexErrorKey.ParseErrorSerialization, new BitfinexError(7001, "Error parsing the returned data to object.")},

                { BitfinexErrorKey.UnknownError, new BitfinexError(8000, "An unknown error happened")},
            };

        internal static BitfinexError GetError(BitfinexErrorKey key)
        {
            return ErrorRegistrations.Single(e => e.Key == key).Value;
        }
    }
}
