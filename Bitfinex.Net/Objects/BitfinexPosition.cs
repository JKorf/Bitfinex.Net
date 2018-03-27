using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexPosition
    {
        [ArrayProperty(0)]
        public string Symbol { get; set; }

        [ArrayProperty(1), JsonConverter(typeof(PositionStatusConverter))]
        public PositionStatus Status { get; set; }

        [ArrayProperty(2)]
        public decimal Amount { get; set; }

        [ArrayProperty(3)]
        public decimal BasePrice { get; set; }

        [ArrayProperty(4)]
        public decimal MarginFunding { get; set; }

        [ArrayProperty(5), JsonConverter(typeof(MarginFundingTypeConverter))]
        public MarginFundingType MarginFundingType { get; set; }

        [ArrayProperty(6)]
        public decimal ProfitLoss { get; set; }

        [ArrayProperty(7)]
        public decimal ProfitLossPercentage { get; set; }

        [ArrayProperty(8)]
        public decimal LiquidationPrice { get; set; }

        [ArrayProperty(9)]
        public decimal Leverage { get; set; }
    }
}
