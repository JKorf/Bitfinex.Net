using System;
using System.Text.RegularExpressions;

namespace Bitfinex.Net
{
    public static class BitfinexHelpers
    {
        /// <summary>
        /// Validate the string is a valid Bitfinex symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateAsBitfinexSymbol(this string symbolString)
        {
            if (!Regex.IsMatch(symbolString, "^(([t]|[f])[A-Z]{6,8})$"))
                throw new ArgumentException($"{symbolString} is not a valid Bitfinex symbol. Should be [t/f][QuoteCurrency][BaseCurrency], e.g. tBTCUSD");
        }
    }
}
