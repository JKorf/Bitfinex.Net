using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexPosition
    {
        [BitfinexProperty(0)]
        public string Symbol { get; set; }

        [BitfinexProperty(1), JsonConverter(typeof(PositionStatusConverter))]
        public PositionStatus Status { get; set; }

        [BitfinexProperty(2)]
        public decimal Amount { get; set; }

        [BitfinexProperty(3)]
        public decimal BasePrice { get; set; }

        [BitfinexProperty(4)]
        public decimal MarginFunding { get; set; }

        [BitfinexProperty(5), JsonConverter(typeof(MarginFundingTypeConverter))]
        public MarginFundingType MarginFundingType { get; set; }

        [BitfinexProperty(6)]
        public decimal ProfitLoss { get; set; }

        [BitfinexProperty(7)]
        public decimal ProfitLossPercentage { get; set; }

        [BitfinexProperty(8)]
        public decimal LiquidationPrice { get; set; }

        [BitfinexProperty(9)]
        public decimal Leverage { get; set; }
    }
}
