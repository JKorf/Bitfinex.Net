using System;
using System.Text.RegularExpressions;

namespace Bitfinex.Net
{
    /// <summary>
    /// Helper functions
    /// </summary>
    public static class BitfinexHelpers
    {
        /// <summary>
        /// Validate the string is a valid Bitfinex symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateBitfinexSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([t]([A-Z|:]{6,8}))$") && !Regex.IsMatch(symbolString, "^([f]([A-Z]{3,4}))$"))
                throw new ArgumentException($"{symbolString} is not a valid Bitfinex symbol. Should be [t][QuoteCurrency][BaseCurrency] for trading pairs " +
                     "or [f][Currency] for margin symbols, e.g. tBTCUSD or fUSD");
        }
    }
}
