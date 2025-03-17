using CryptoExchange.Net.Converters.SystemTextJson;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models.Socket
{
    /// <summary>
    /// Socket event wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record BitfinexSocketEvent<T>
    {
        /// <summary>
        /// The channel id of the event
        /// </summary>
        [ArrayProperty(0)]
        private int ChannelId { get; set; }
        /// <summary>
        /// The type of the event
        /// </summary>
        [ArrayProperty(1)]
        public BitfinexEventType EventType { get; set; }

        /// <summary>
        /// The data
        /// </summary>
        [ArrayProperty(2)]
        [JsonConversion]
        public T Data { get; set; } = default!;

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexSocketEvent() { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public BitfinexSocketEvent(BitfinexEventType type, T data)
        {
            EventType = type;
            Data = data;
        }
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexSocketStringEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexSocketStringEvent : BitfinexSocketEvent<string> { }


    [JsonConverter(typeof(ArrayConverter<BitfinexSocketPositionsEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexSocketPositionsEvent : BitfinexSocketEvent<BitfinexPosition[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexSocketPositionEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexSocketPositionEvent : BitfinexSocketEvent<BitfinexPosition> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexBalanceEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexBalanceEvent : BitfinexSocketEvent<BitfinexBalance> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexMarginBaseEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexMarginBaseEvent : BitfinexSocketEvent<BitfinexMarginBase> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexMarginSymbolEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexMarginSymbolEvent : BitfinexSocketEvent<BitfinexMarginSymbol> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingInfoEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingInfoEvent : BitfinexSocketEvent<BitfinexFundingInfo> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexWalletsEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexWalletsEvent : BitfinexSocketEvent<BitfinexWallet[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexWalletEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexWalletEvent : BitfinexSocketEvent<BitfinexWallet> { }
    
    [JsonConverter(typeof(ArrayConverter<BitfinexOrdersEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexOrdersEvent : BitfinexSocketEvent<BitfinexOrder[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexOrderEvent : BitfinexSocketEvent<BitfinexOrder> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexTradeDetailEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexTradeDetailEvent : BitfinexSocketEvent<BitfinexTradeDetails> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingTradeEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingTradeEvent : BitfinexSocketEvent<BitfinexFundingTrade> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOffersEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexOffersEvent : BitfinexSocketEvent<BitfinexFundingOffer[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOfferEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexOfferEvent : BitfinexSocketEvent<BitfinexFundingOffer> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingCreditsEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingCreditsEvent : BitfinexSocketEvent<BitfinexFundingCredit[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingCreditEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingCreditEvent : BitfinexSocketEvent<BitfinexFundingCredit> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingsEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingsEvent : BitfinexSocketEvent<BitfinexFunding[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingEvent : BitfinexSocketEvent<BitfinexFunding> { }


    [JsonConverter(typeof(ArrayConverter<BitfinexOrderNotificationEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexOrderNotificationEvent : BitfinexSocketEvent<BitfinexOrderNotification> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrdersNotificationEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexOrdersNotificationEvent : BitfinexSocketEvent<BitfinexOrdersNotification> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingOfferNotificationEvent, BitfinexSourceGenerationContext>))]
    [SerializationModel]
    internal record BitfinexFundingOfferNotificationEvent : BitfinexSocketEvent<BitfinexFundingOfferNotification> { }
}
