using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMovement
    {
        [BitfinexProperty(0)]
        public string Id { get; set; }
        [BitfinexProperty(1)]
        public string Currency { get; set; }
        [BitfinexProperty(2)]
        public string CurrencyName { get; set; }
        [BitfinexProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime Started { get; set; }
        [BitfinexProperty(6), JsonConverter(typeof(TimestampConverter))]
        public DateTime Updated { get; set; }
        [BitfinexProperty(9)]
        public string Status { get; set; }
        [BitfinexProperty(12)]
        public decimal Amount { get; set; }
        [BitfinexProperty(13)]
        public decimal Fees { get; set; }
        [BitfinexProperty(16)]
        public string Address { get; set; }
        [BitfinexProperty(20)]
        public string TransactionId { get; set; }
    }
}
