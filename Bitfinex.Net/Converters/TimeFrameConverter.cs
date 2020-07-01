using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class TimeFrameConverter: BaseConverter<TimeFrame>
    {
        public TimeFrameConverter(): this(true) { }
        public TimeFrameConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<TimeFrame, string>> Mapping => new List<KeyValuePair<TimeFrame, string>>
        {
            new KeyValuePair<TimeFrame, string>(TimeFrame.OneMinute, "1m"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.FiveMinute, "5m"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.FifteenMinute, "15m"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.ThirtyMinute, "30m"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.OneHour, "1h"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.ThreeHour, "3h"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.SixHour, "6h"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.TwelveHour, "12h"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.OneDay, "1D"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.SevenDay, "7D"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.FourteenDay, "14D"),
            new KeyValuePair<TimeFrame, string>(TimeFrame.OneMonth, "1M")
        };
    }
}
