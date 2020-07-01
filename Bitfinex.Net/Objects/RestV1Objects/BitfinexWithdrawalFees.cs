using System.Collections.Generic;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    /// <summary>
    /// Withdrawal fee dictionary
    /// </summary>
    public class BitfinexWithdrawalFees
    {
        /// <summary>
        /// List of fees
        /// </summary>
        public Dictionary<string, decimal> Withdraw { get; set; } = new Dictionary<string, decimal>();
    }
}
