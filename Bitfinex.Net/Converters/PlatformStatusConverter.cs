using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class PlatformStatusConverter: BaseConverter<PlatformStatus>
    {
        public PlatformStatusConverter(): this(true) { }
        public PlatformStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<PlatformStatus, string> Mapping => new Dictionary<PlatformStatus, string>()
        {
            { PlatformStatus.Maintenance, "0" },
            { PlatformStatus.Operative, "1" },
        };
    }
}
