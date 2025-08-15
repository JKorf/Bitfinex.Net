using CryptoExchange.Net.Objects.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net
{
    internal static class BitfinexErrors
    {
        public static ErrorCollection Errors { get; } = new ErrorCollection(
            [
                new ErrorInfo(ErrorType.Unauthorized, false, "Invalid API key", "10100")
            ]
        );
    }
}
