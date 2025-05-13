using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Objects.Sockets
{
    internal abstract class BitfinexUpdate<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        [JsonConversion]
        public T Data { get; set; } = default!;
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexStringUpdate>))]
    [SerializationModel]
    internal class BitfinexStringUpdate: BitfinexUpdate<string> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamTickerUpdate>))]
    [SerializationModel]
    internal class BitfinexStreamTickerUpdate: BitfinexUpdate<BitfinexStreamTicker> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamTickerArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexStreamTickerArrayUpdate: BitfinexUpdate<BitfinexStreamTicker[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamFundingTickerUpdate>))]
    [SerializationModel]
    internal class BitfinexStreamFundingTickerUpdate : BitfinexUpdate<BitfinexStreamFundingTicker> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamFundingTickerArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexStreamFundingTickerArrayUpdate : BitfinexUpdate<BitfinexStreamFundingTicker[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookEntryUpdate>))]
    [SerializationModel]
    internal class BitfinexOrderBookEntryUpdate : BitfinexUpdate<BitfinexOrderBookEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookEntryArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexOrderBookEntryArrayUpdate : BitfinexUpdate<BitfinexOrderBookEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookFundingEntryUpdate>))]
    [SerializationModel]
    internal class BitfinexOrderBookFundingEntryUpdate : BitfinexUpdate<BitfinexOrderBookFundingEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookFundingEntryArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexOrderBookFundingEntryArrayUpdate : BitfinexUpdate<BitfinexOrderBookFundingEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookEntryUpdate>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookEntryUpdate : BitfinexUpdate<BitfinexRawOrderBookEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookEntryArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookEntryArrayUpdate : BitfinexUpdate<BitfinexRawOrderBookEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookFundingEntryUpdate>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookFundingEntryUpdate : BitfinexUpdate<BitfinexRawOrderBookFundingEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookFundingEntryArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookFundingEntryArrayUpdate : BitfinexUpdate<BitfinexRawOrderBookFundingEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexTradeUpdate>))]
    [SerializationModel]
    internal class BitfinexTradeUpdate
    {

        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        public string Topic { get; set; } = string.Empty;
        [ArrayProperty(2)]
        public BitfinexTradeSimple Data { get; set; } = default!;    
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexTradeArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexTradeArrayUpdate : BitfinexUpdate<BitfinexTradeSimple[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexKlineUpdate>))]
    [SerializationModel]
    internal class BitfinexKlineUpdate : BitfinexUpdate<BitfinexKline> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexKlineArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexKlineArrayUpdate : BitfinexUpdate<BitfinexKline[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexLiquidationUpdate>))]
    [SerializationModel]
    internal class BitfinexLiquidationUpdate : BitfinexUpdate<BitfinexLiquidation> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexLiquidationArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexLiquidationArrayUpdate : BitfinexUpdate<BitfinexLiquidation[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexDerivativesStatusUpdateUpdate>))]
    [SerializationModel]
    internal class BitfinexDerivativesStatusUpdateUpdate : BitfinexUpdate<BitfinexDerivativesStatusUpdate> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexDerivativesStatusUpdateArrayUpdate>))]
    [SerializationModel]
    internal class BitfinexDerivativesStatusUpdateArrayUpdate : BitfinexUpdate<BitfinexDerivativesStatusUpdate[]> { }

}
