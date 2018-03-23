using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class FrequencyConverter : BaseConverter<Frequency>
    {
        public FrequencyConverter() : this(true) { }
        public FrequencyConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<Frequency, string> Mapping => new Dictionary<Frequency, string>()
        {
            { Frequency.Realtime, "F0" },
            { Frequency.TwoSeconds, "F1" },
        };
    }
}
