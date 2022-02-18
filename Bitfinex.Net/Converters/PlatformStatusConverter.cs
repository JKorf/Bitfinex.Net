using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class PlatformStatusConverter: BaseConverter<PlatformStatus>
    {
        public PlatformStatusConverter(): this(true) { }
        public PlatformStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<PlatformStatus, string>> Mapping => new List<KeyValuePair<PlatformStatus, string>>
        {
            new KeyValuePair<PlatformStatus, string>(PlatformStatus.Maintenance, "0"),
            new KeyValuePair<PlatformStatus, string>(PlatformStatus.Operative, "1")
        };
    }
}
