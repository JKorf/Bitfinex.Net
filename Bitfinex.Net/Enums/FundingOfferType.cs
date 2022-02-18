namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding type
    /// </summary>
    public enum FundingOfferType
    {
        /// <summary>
        /// Lend
        /// </summary>
        Limit,
        /// <summary>
        /// FFR delta var
        /// </summary>
        FlashReturnRateDeltaVariable,
        /// <summary>
        /// FFR delta fix
        /// </summary>
        FlashReturnRateDeltaFixed
    }
}
