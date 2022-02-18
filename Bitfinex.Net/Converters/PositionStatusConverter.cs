using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class PositionStatusConverter: BaseConverter<PositionStatus>
    {
        public PositionStatusConverter(): this(true) { }
        public PositionStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<PositionStatus, string>> Mapping => new List<KeyValuePair<PositionStatus, string>>
        {
            new KeyValuePair<PositionStatus, string>(PositionStatus.Closed, "CLOSED"),
            new KeyValuePair<PositionStatus, string>(PositionStatus.Active, "ACTIVE")
        };
    }
}
