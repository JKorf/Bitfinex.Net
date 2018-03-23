using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class PositionStatusConverter: BaseConverter<PositionStatus>
    {
        public PositionStatusConverter(): this(true) { }
        public PositionStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<PositionStatus, string> Mapping => new Dictionary<PositionStatus, string>()
        {
            { PositionStatus.Closed, "CLOSED" },
            { PositionStatus.Active, "ACTIVE" }
        };
    }
}
