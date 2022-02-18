namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Order types
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,
        /// <summary>
        /// Market order
        /// </summary>
        Market,
        /// <summary>
        /// Stop order
        /// </summary>
        Stop,
        /// <summary>
        /// Stop limit order
        /// </summary>
        StopLimit,
        /// <summary>
        /// Trailing stop order
        /// </summary>
        TrailingStop,
        /// <summary>
        /// Exchange market order
        /// </summary>
        ExchangeMarket,
        /// <summary>
        /// Exchange limit order
        /// </summary>
        ExchangeLimit,
        /// <summary>
        /// Exchange stop order
        /// </summary>
        ExchangeStop,
        /// <summary>
        /// Exchange stop limit order
        /// </summary>
        ExchangeStopLimit,
        /// <summary>
        /// Exchange trailing stop order
        /// </summary>
        ExchangeTrailingStop,
        /// <summary>
        /// Fill or kill order
        /// </summary>
        FillOrKill,
        /// <summary>
        /// Exchange fill or kill order
        /// </summary>
        ExchangeFillOrKill
    }
}
