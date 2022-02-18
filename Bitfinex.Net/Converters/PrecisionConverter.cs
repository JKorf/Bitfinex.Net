using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class PrecisionConverter: BaseConverter<Precision>
    {
        public PrecisionConverter(): this(true) { }
        public PrecisionConverter(bool useQuotes) : base(useQuotes)
        {
        }

        protected override List<KeyValuePair<Precision, string>> Mapping => new List<KeyValuePair<Precision, string>>
        {
            new KeyValuePair<Precision, string>(Precision.PrecisionLevel0, "P0"),
            new KeyValuePair<Precision, string>(Precision.PrecisionLevel1, "P1"),
            new KeyValuePair<Precision, string>(Precision.PrecisionLevel2, "P2"),
            new KeyValuePair<Precision, string>(Precision.PrecisionLevel3, "P3"),
            new KeyValuePair<Precision, string>(Precision.R0, "R0")
        };
    }
}
