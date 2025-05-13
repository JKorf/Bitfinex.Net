using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Socket event types
    /// </summary>
    [JsonConverter(typeof(EnumConverter<BitfinexEventType>))]
    [SerializationModel]
    public enum BitfinexEventType
    {
        /// <summary>
        /// Heartbeat
        /// </summary>
        [Map("hb")]
        HeartBeat,

        /// <summary>
        /// Balance update
        /// </summary>
        [Map("bu")]
        BalanceUpdate,

        /// <summary>
        /// Position snapshot
        /// </summary>
        [Map("ps")]
        PositionSnapshot,
        /// <summary>
        /// New position
        /// </summary>
        [Map("pn")]
        PositionNew,
        /// <summary>
        /// Position update
        /// </summary>
        [Map("pu")]
        PositionUpdate,
        /// <summary>
        /// Position closed
        /// </summary>
        [Map("pc")]
        PositionClose,

        /// <summary>
        /// Wallet snapshot
        /// </summary>
        [Map("ws")]
        WalletSnapshot,
        /// <summary>
        /// Wallet update
        /// </summary>
        [Map("wu")]
        WalletUpdate,

        /// <summary>
        /// Orders snapshot
        /// </summary>
        [Map("os")]
        OrderSnapshot,
        /// <summary>
        /// New order
        /// </summary>
        [Map("on")]
        OrderNew,
        /// <summary>
        /// New order request
        /// </summary>
        [Map("on-req")]
        OrderNewRequest,
        /// <summary>
        /// Order update
        /// </summary>
        [Map("ou")]
        OrderUpdate,
        /// <summary>
        /// Order update request
        /// </summary>
        [Map("ou-req")]
        OrderUpdateRequest,
        /// <summary>
        /// Order canceled
        /// </summary>
        [Map("oc")]
        OrderCancel,
        /// <summary>
        /// Order cancel request
        /// </summary>
        [Map("oc-req")]
        OrderCancelRequest,
        /// <summary>
        /// Multiple orders canceled
        /// </summary>
        [Map("oc_multi")]
        OrderCancelMulti,
        /// <summary>
        /// Multiple orders cancel request
        /// </summary>
        [Map("oc_multi-req")]
        OrderCancelMultiRequest,

        /// <summary>
        /// Trade executed
        /// </summary>
        [Map("te")]
        TradeExecuted,
        /// <summary>
        /// Trade execution update
        /// </summary>
        [Map("tu")]
        TradeExecutionUpdate,

        /// <summary>
        /// Funding trade execution
        /// </summary>
        [Map("fte")]
        FundingTradeExecution,
        /// <summary>
        /// Funding trade update
        /// </summary>
        [Map("ftu")]
        FundingTradeUpdate,

        /// <summary>
        /// Funding info update
        /// </summary>
        [Map("fiu")]
        FundingInfoUpdate,

        /// <summary>
        /// Margin info snapshot
        /// </summary>
        [Map("mis")]
        MarginInfoSnapshot,
        /// <summary>
        /// Margin info update
        /// </summary>
        [Map("miu")]
        MarginInfoUpdate,

        /// <summary>
        /// Notification
        /// </summary>
        [Map("n")]
        Notification,

        /// <summary>
        /// Funding offer snapshot
        /// </summary>
        [Map("fos")]
        FundingOfferSnapshot,
        /// <summary>
        /// New funding offer
        /// </summary>
        [Map("fon")]
        FundingOfferNew,
        /// <summary>
        /// New funding offer request
        /// </summary>
        [Map("fon-req")]
        FundingOfferNewRequest,
        /// <summary>
        /// Funding offer update
        /// </summary>
        [Map("fou")]
        FundingOfferUpdate,
        /// <summary>
        /// Funding offer canceled
        /// </summary>
        [Map("foc")]
        FundingOfferCancel,
        /// <summary>
        /// Funding offer cancel request
        /// </summary>
        [Map("foc-req")]
        FundingOfferCancelRequest,

        /// <summary>
        /// Funding credits snapshot
        /// </summary>
        [Map("fcs")]
        FundingCreditsSnapshot,
        /// <summary>
        /// New funding credits
        /// </summary>
        [Map("fcn")]
        FundingCreditsNew,
        /// <summary>
        /// Funding credits update
        /// </summary>
        [Map("fcu")]
        FundingCreditsUpdate,
        /// <summary>
        /// Funding credits closed
        /// </summary>
        [Map("fcc")]
        FundingCreditsClose,
        
        /// <summary>
        /// Funding loan snapshot
        /// </summary>
        [Map("fls")]
        FundingLoanSnapshot,
        /// <summary>
        /// New funding loan
        /// </summary>
        [Map("fln")]
        FundingLoanNew,
        /// <summary>
        /// Funding loan update
        /// </summary>
        [Map("flu")]
        FundingLoanUpdate,
        /// <summary>
        /// Funding loan closed
        /// </summary>
        [Map("flc")]
        FundingLoanClose,

        /// <summary>
        /// Custom user price alert
        /// </summary>
        [Map("uac")]
        UserCustomPriceAlert
    }
}
