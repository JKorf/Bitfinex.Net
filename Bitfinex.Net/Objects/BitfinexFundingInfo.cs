using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexFundingInfo
    {
        [BitfinexProperty(0)]
        public string Type { get; set; }

        [BitfinexProperty(1)]
        public string Symbol { get; set; }

        [BitfinexProperty(2), JsonConverter(typeof(BitfinexResultConverter))]
        public BitfinexFundingInfoDetails Data { get; set; }
    }

    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexFundingInfoDetails
    {
        [BitfinexProperty(0)]
        public double YieldLoan { get; set; }
        [BitfinexProperty(1)]
        public double YieldLend { get; set; }
        [BitfinexProperty(2)]
        public double DurationLoan { get; set; }
        [BitfinexProperty(3)]
        public double DurationLend { get; set; }
    }
}
