﻿using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding - Order Types.
    /// </summary>
    public enum FundingOrderType
    {
        /// <summary>
        /// Place an order at an explicit, static rate.
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// Place an order at an implicit, static rate, relative to the FRR.
        /// </summary>
        [Map("FRRDELTAVAR")]
        FRRDeltaVar,
        /// <summary>
        /// Place an order at an implicit, dynamic rate, relative to the FRR.
        /// </summary>
        [Map("FRRDELTAFIX")]
        FRRDeltaFix,
    }
}
