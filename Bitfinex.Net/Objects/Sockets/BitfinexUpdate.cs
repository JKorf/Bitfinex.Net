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

    [JsonConverter(typeof(ArrayConverter<BitfinexStringUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexStringUpdate: BitfinexUpdate<string> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamTickerUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexStreamTickerUpdate: BitfinexUpdate<BitfinexStreamTicker> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamTickerArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexStreamTickerArrayUpdate: BitfinexUpdate<BitfinexStreamTicker[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamFundingTickerUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexStreamFundingTickerUpdate : BitfinexUpdate<BitfinexStreamFundingTicker> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexStreamFundingTickerArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexStreamFundingTickerArrayUpdate : BitfinexUpdate<BitfinexStreamFundingTicker[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookEntryUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexOrderBookEntryUpdate : BitfinexUpdate<BitfinexOrderBookEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookEntryArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexOrderBookEntryArrayUpdate : BitfinexUpdate<BitfinexOrderBookEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookFundingEntryUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexOrderBookFundingEntryUpdate : BitfinexUpdate<BitfinexOrderBookFundingEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookFundingEntryArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexOrderBookFundingEntryArrayUpdate : BitfinexUpdate<BitfinexOrderBookFundingEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookEntryUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookEntryUpdate : BitfinexUpdate<BitfinexRawOrderBookEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookEntryArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookEntryArrayUpdate : BitfinexUpdate<BitfinexRawOrderBookEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookFundingEntryUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookFundingEntryUpdate : BitfinexUpdate<BitfinexRawOrderBookFundingEntry> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookFundingEntryArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexRawOrderBookFundingEntryArrayUpdate : BitfinexUpdate<BitfinexRawOrderBookFundingEntry[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexTradeUpdate, BitfinexSourceGenerationContext>))]
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

    [JsonConverter(typeof(ArrayConverter<BitfinexTradeArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexTradeArrayUpdate : BitfinexUpdate<BitfinexTradeSimple[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexKlineUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexKlineUpdate : BitfinexUpdate<BitfinexKline> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexKlineArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexKlineArrayUpdate : BitfinexUpdate<BitfinexKline[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexLiquidationUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexLiquidationUpdate : BitfinexUpdate<BitfinexLiquidation> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexLiquidationArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexLiquidationArrayUpdate : BitfinexUpdate<BitfinexLiquidation[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexDerivativesStatusUpdateUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexDerivativesStatusUpdateUpdate : BitfinexUpdate<BitfinexDerivativesStatusUpdate> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexDerivativesStatusUpdateArrayUpdate, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal class BitfinexDerivativesStatusUpdateArrayUpdate : BitfinexUpdate<BitfinexDerivativesStatusUpdate[]> { }

}
