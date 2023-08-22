using System;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Flags for an order
    /// </summary>
    [Flags]
    public enum OrderFlags
    {
        /// <summary>
        /// Order is hidden
        /// </summary>
        Hidden = 64,
        /// <summary>
        /// Close
        /// </summary>
        Close = 512,
        /// <summary>
        /// Ensures that the executed order does not flip the opened position.
        /// </summary>
        ReduceOnly = 1024,
        /// <summary>
        /// Only accept if it is a post
        /// </summary>
        PostOnly = 4096,
        /// <summary>
        /// Oco order
        /// </summary>
        OneCancelsOther = 16384,
        /// <summary>
        /// Excludes variable rate funding offers from matching against this order, if on margin
        /// </summary>
        NoVarRates = 524288
    }
}
