using System;
using System.Text.RegularExpressions;

namespace Bitfinex.Net.ExtensionMethods
{
    /// <summary>
    /// Extension methods specific to using the Bitfinex API
    /// </summary>
    public static class BitfinexExtensionMethods
    {
        /// <summary>
        /// Validate the string is a valid Bitfinex symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateBitfinexSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([t]([A-Z0-9|:]{6,}))$") && !Regex.IsMatch(symbolString, "^([f]([A-Z0-9]{3,}))$"))
                throw new ArgumentException($"{symbolString} is not a valid Bitfinex symbol. Should be [t][QuoteAsset][BaseAsset] for trading pairs " +
                     "or [f][Asset] for margin symbols, e.g. tBTCUSD or fUSD");
        }

        /// <summary>
        /// Validate the string is a valid Bitfinex symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateBitfinexFundingSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([f]([A-Z0-9]{3,}))$"))
                throw new ArgumentException($"{symbolString} is not a valid Bitfinex funding symbol. Should be [f][Asset] for funding symbols, e.g. fUSD");
        }

        /// <summary>
        /// Validate the string is a valid Bitfinex symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateBitfinexTradingSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([t]([A-Z0-9|:]{6,}))$"))
                throw new ArgumentException($"{symbolString} is not a valid Bitfinex symbol. Should be [t][QuoteAsset][BaseAsset] for trading pairs, e.g. tBTCUSD");
        }
    }
}
