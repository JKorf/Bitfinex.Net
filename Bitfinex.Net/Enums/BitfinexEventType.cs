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
        /// ["<c>hb</c>"] Heartbeat
        /// </summary>
        [Map("hb")]
        HeartBeat,

        /// <summary>
        /// ["<c>bu</c>"] Balance update
        /// </summary>
        [Map("bu")]
        BalanceUpdate,

        /// <summary>
        /// ["<c>ps</c>"] Position snapshot
        /// </summary>
        [Map("ps")]
        PositionSnapshot,
        /// <summary>
        /// ["<c>pn</c>"] New position
        /// </summary>
        [Map("pn")]
        PositionNew,
        /// <summary>
        /// ["<c>pu</c>"] Position update
        /// </summary>
        [Map("pu")]
        PositionUpdate,
        /// <summary>
        /// ["<c>pc</c>"] Position closed
        /// </summary>
        [Map("pc")]
        PositionClose,

        /// <summary>
        /// ["<c>ws</c>"] Wallet snapshot
        /// </summary>
        [Map("ws")]
        WalletSnapshot,
        /// <summary>
        /// ["<c>wu</c>"] Wallet update
        /// </summary>
        [Map("wu")]
        WalletUpdate,

        /// <summary>
        /// ["<c>os</c>"] Orders snapshot
        /// </summary>
        [Map("os")]
        OrderSnapshot,
        /// <summary>
        /// ["<c>on</c>"] New order
        /// </summary>
        [Map("on")]
        OrderNew,
        /// <summary>
        /// ["<c>on-req</c>"] New order request
        /// </summary>
        [Map("on-req")]
        OrderNewRequest,
        /// <summary>
        /// ["<c>ou</c>"] Order update
        /// </summary>
        [Map("ou")]
        OrderUpdate,
        /// <summary>
        /// ["<c>ou-req</c>"] Order update request
        /// </summary>
        [Map("ou-req")]
        OrderUpdateRequest,
        /// <summary>
        /// ["<c>oc</c>"] Order canceled
        /// </summary>
        [Map("oc")]
        OrderCancel,
        /// <summary>
        /// ["<c>oc-req</c>"] Order cancel request
        /// </summary>
        [Map("oc-req")]
        OrderCancelRequest,
        /// <summary>
        /// ["<c>oc_multi</c>"] Multiple orders canceled
        /// </summary>
        [Map("oc_multi")]
        OrderCancelMulti,
        /// <summary>
        /// ["<c>oc_multi-req</c>"] Multiple orders cancel request
        /// </summary>
        [Map("oc_multi-req")]
        OrderCancelMultiRequest,

        /// <summary>
        /// ["<c>te</c>"] Trade executed
        /// </summary>
        [Map("te")]
        TradeExecuted,
        /// <summary>
        /// ["<c>tu</c>"] Trade execution update
        /// </summary>
        [Map("tu")]
        TradeExecutionUpdate,

        /// <summary>
        /// ["<c>fte</c>"] Funding trade execution
        /// </summary>
        [Map("fte")]
        FundingTradeExecution,
        /// <summary>
        /// ["<c>ftu</c>"] Funding trade update
        /// </summary>
        [Map("ftu")]
        FundingTradeUpdate,

        /// <summary>
        /// ["<c>fiu</c>"] Funding info update
        /// </summary>
        [Map("fiu")]
        FundingInfoUpdate,

        /// <summary>
        /// ["<c>mis</c>"] Margin info snapshot
        /// </summary>
        [Map("mis")]
        MarginInfoSnapshot,
        /// <summary>
        /// ["<c>miu</c>"] Margin info update
        /// </summary>
        [Map("miu")]
        MarginInfoUpdate,

        /// <summary>
        /// ["<c>n</c>"] Notification
        /// </summary>
        [Map("n")]
        Notification,

        /// <summary>
        /// ["<c>fos</c>"] Funding offer snapshot
        /// </summary>
        [Map("fos")]
        FundingOfferSnapshot,
        /// <summary>
        /// ["<c>fon</c>"] New funding offer
        /// </summary>
        [Map("fon")]
        FundingOfferNew,
        /// <summary>
        /// ["<c>fon-req</c>"] New funding offer request
        /// </summary>
        [Map("fon-req")]
        FundingOfferNewRequest,
        /// <summary>
        /// ["<c>fou</c>"] Funding offer update
        /// </summary>
        [Map("fou")]
        FundingOfferUpdate,
        /// <summary>
        /// ["<c>foc</c>"] Funding offer canceled
        /// </summary>
        [Map("foc")]
        FundingOfferCancel,
        /// <summary>
        /// ["<c>foc-req</c>"] Funding offer cancel request
        /// </summary>
        [Map("foc-req")]
        FundingOfferCancelRequest,

        /// <summary>
        /// ["<c>fcs</c>"] Funding credits snapshot
        /// </summary>
        [Map("fcs")]
        FundingCreditsSnapshot,
        /// <summary>
        /// ["<c>fcn</c>"] New funding credits
        /// </summary>
        [Map("fcn")]
        FundingCreditsNew,
        /// <summary>
        /// ["<c>fcu</c>"] Funding credits update
        /// </summary>
        [Map("fcu")]
        FundingCreditsUpdate,
        /// <summary>
        /// ["<c>fcc</c>"] Funding credits closed
        /// </summary>
        [Map("fcc")]
        FundingCreditsClose,
        
        /// <summary>
        /// ["<c>fls</c>"] Funding loan snapshot
        /// </summary>
        [Map("fls")]
        FundingLoanSnapshot,
        /// <summary>
        /// ["<c>fln</c>"] New funding loan
        /// </summary>
        [Map("fln")]
        FundingLoanNew,
        /// <summary>
        /// ["<c>flu</c>"] Funding loan update
        /// </summary>
        [Map("flu")]
        FundingLoanUpdate,
        /// <summary>
        /// ["<c>flc</c>"] Funding loan closed
        /// </summary>
        [Map("flc")]
        FundingLoanClose,

        /// <summary>
        /// ["<c>uac</c>"] Custom user price alert
        /// </summary>
        [Map("uac")]
        UserCustomPriceAlert
    }
}
