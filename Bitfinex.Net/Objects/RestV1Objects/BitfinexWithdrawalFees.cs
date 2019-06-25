using System.Collections.Generic;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    public class BitfinexWithdrawalFees
    {
        /// <summary>
        /// List of fees
        /// </summary>
        public Dictionary<string, decimal> Withdraw { get; set; }
    }
}
