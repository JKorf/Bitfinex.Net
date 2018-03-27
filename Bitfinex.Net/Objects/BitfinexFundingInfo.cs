using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingInfo
    {
        [ArrayProperty(0)]
        public string Type { get; set; }

        [ArrayProperty(1)]
        public string Symbol { get; set; }

        [ArrayProperty(2), JsonConverter(typeof(ArrayConverter))]
        public BitfinexFundingInfoDetails Data { get; set; }
    }

    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingInfoDetails
    {
        [ArrayProperty(0)]
        public decimal YieldLoan { get; set; }
        [ArrayProperty(1)]
        public decimal YieldLend { get; set; }
        [ArrayProperty(2)]
        public decimal DurationLoan { get; set; }
        [ArrayProperty(3)]
        public decimal DurationLend { get; set; }
    }
}
