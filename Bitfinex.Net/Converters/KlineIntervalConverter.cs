using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class KlineIntervalConverter: BaseConverter<KlineInterval>
    {
        public KlineIntervalConverter(): this(true) { }
        public KlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KlineInterval, string>> Mapping => new List<KeyValuePair<KlineInterval, string>>
        {
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMinute, "1m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinutes, "5m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FifteenMinutes, "15m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinutes, "30m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneHour, "1h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeHours, "3h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SixHours, "6h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwelveHours, "12h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "1D"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SevenDays, "7D"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FourteenDays, "14D"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMonth, "1M")
        };
    }
}
