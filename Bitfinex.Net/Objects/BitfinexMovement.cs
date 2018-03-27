using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMovement
    {
        [ArrayProperty(0)]
        public string Id { get; set; }
        [ArrayProperty(1)]
        public string Currency { get; set; }
        [ArrayProperty(2)]
        public string CurrencyName { get; set; }
        [ArrayProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime Started { get; set; }
        [ArrayProperty(6), JsonConverter(typeof(TimestampConverter))]
        public DateTime Updated { get; set; }
        [ArrayProperty(9)]
        public string Status { get; set; }
        [ArrayProperty(12)]
        public decimal Amount { get; set; }
        [ArrayProperty(13)]
        public decimal Fees { get; set; }
        [ArrayProperty(16)]
        public string Address { get; set; }
        [ArrayProperty(20)]
        public string TransactionId { get; set; }
    }
}
