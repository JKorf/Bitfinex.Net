namespace Bitfinex.Net.Objects.RestV1Objects
{
    /// <summary>
    /// Close position result
    /// </summary>
    public class BitfinexClosePositionResult
    {
        /// <summary>
        /// Status message
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// The position status
        /// </summary>
        public BitfinexPositionV1 Position { get; set; } = default!;

        /// <summary>
        /// The order used to close the position
        /// </summary>
        public BitfinexPlacedOrder Order { get; set; } = default!;
    }
}
