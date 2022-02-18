using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class FrequencyConverter : BaseConverter<Frequency>
    {
        public FrequencyConverter() : this(true) { }
        public FrequencyConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<Frequency, string>> Mapping => new List<KeyValuePair<Frequency, string>>
        {
            new KeyValuePair<Frequency, string>(Frequency.Realtime, "F0"),
            new KeyValuePair<Frequency, string>(Frequency.TwoSeconds, "F1")
        };
    }
}
