namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Order types for V1 API
    /// </summary>
    public enum OrderTypeV1
    {
        /// <summary>
        /// Market order
        /// </summary>
        Market,
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,
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
        /// Fill or kill order
        /// </summary>
        FillOrKill,
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
        /// Exchange fill or kill order
        /// </summary>
        ExchangeFillOrKill
    }
}
