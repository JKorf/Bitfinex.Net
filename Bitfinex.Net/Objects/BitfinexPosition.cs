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
        public double Amount { get; set; }

        [BitfinexProperty(3)]
        public double BasePrice { get; set; }

        [BitfinexProperty(4)]
        public double MarginFunding { get; set; }

        [BitfinexProperty(5), JsonConverter(typeof(MarginFundingTypeConverter))]
        public MarginFundingType MarginFundingType { get; set; }

        [BitfinexProperty(6)]
        public double ProfitLoss { get; set; }

        [BitfinexProperty(7)]
        public double ProfitLossPercentage { get; set; }

        [BitfinexProperty(8)]
        public double LiquidationPrice { get; set; }

        [BitfinexProperty(9)]
        public double Leverage { get; set; }
    }
}
