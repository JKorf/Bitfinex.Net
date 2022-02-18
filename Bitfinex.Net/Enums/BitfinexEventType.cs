namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Socket event types
    /// </summary>
    public enum BitfinexEventType
    {
        /// <summary>
        /// Heartbeat
        /// </summary>
        HeartBeat,

        /// <summary>
        /// Balance update
        /// </summary>
        BalanceUpdate,

        /// <summary>
        /// Position snapshot
        /// </summary>
        PositionSnapshot,
        /// <summary>
        /// New position
        /// </summary>
        PositionNew,
        /// <summary>
        /// Position update
        /// </summary>
        PositionUpdate,
        /// <summary>
        /// Position closed
        /// </summary>
        PositionClose,

        /// <summary>
        /// Wallet snapshot
        /// </summary>
        WalletSnapshot,
        /// <summary>
        /// Wallet update
        /// </summary>
        WalletUpdate,

        /// <summary>
        /// Orders snapshot
        /// </summary>
        OrderSnapshot,
        /// <summary>
        /// New order
        /// </summary>
        OrderNew,
        /// <summary>
        /// New order request
        /// </summary>
        OrderNewRequest,
        /// <summary>
        /// Order update
        /// </summary>
        OrderUpdate,
        /// <summary>
        /// Order update request
        /// </summary>
        OrderUpdateRequest,
        /// <summary>
        /// Order canceled
        /// </summary>
        OrderCancel,
        /// <summary>
        /// Order cancel request
        /// </summary>
        OrderCancelRequest,
        /// <summary>
        /// Multiple orders canceled
        /// </summary>
        OrderCancelMulti,
        /// <summary>
        /// Multiple orders cancel request
        /// </summary>
        OrderCancelMultiRequest,

        /// <summary>
        /// Trade snapshot
        /// </summary>
        TradeSnapshot,
        /// <summary>
        /// Trade executed
        /// </summary>
        TradeExecuted,
        /// <summary>
        /// Trade execution update
        /// </summary>
        TradeExecutionUpdate,

        /// <summary>
        /// Funding trade execution
        /// </summary>
        FundingTradeExecution,
        /// <summary>
        /// Funding trade update
        /// </summary>
        FundingTradeUpdate,

        /// <summary>
        /// Historical order snapshot
        /// </summary>
        HistoricalOrderSnapshot,

        /// <summary>
        /// Margin info snapshot
        /// </summary>
        MarginInfoSnapshot,
        /// <summary>
        /// Margin info update
        /// </summary>
        MarginInfoUpdate,

        /// <summary>
        /// Notification
        /// </summary>
        Notification,

        /// <summary>
        /// Funding offer snapshot
        /// </summary>
        FundingOfferSnapshot,
        /// <summary>
        /// New funding offer
        /// </summary>
        FundingOfferNew,
        /// <summary>
        /// Funding offer update
        /// </summary>
        FundingOfferUpdate,
        /// <summary>
        /// Funding offer canceled
        /// </summary>
        FundingOfferCancel,

        /// <summary>
        /// Historical funding offer snapshot
        /// </summary>
        HistoricalFundingOfferSnapshot,

        /// <summary>
        /// Funding credits snapshot
        /// </summary>
        FundingCreditsSnapshot,
        /// <summary>
        /// New funding credits
        /// </summary>
        FundingCreditsNew,
        /// <summary>
        /// Funding credits update
        /// </summary>
        FundingCreditsUpdate,
        /// <summary>
        /// Funding credits closed
        /// </summary>
        FundingCreditsClose,

        /// <summary>
        /// Historical funding credits snapshot
        /// </summary>
        HistoricalFundingCreditsSnapshot,

        /// <summary>
        /// Funding loan snapshot
        /// </summary>
        FundingLoanSnapshot,
        /// <summary>
        /// New funding loan
        /// </summary>
        FundingLoanNew,
        /// <summary>
        /// Funding loan update
        /// </summary>
        FundingLoanUpdate,
        /// <summary>
        /// Funding loan closed
        /// </summary>
        FundingLoanClose,

        /// <summary>
        /// Historical funding loan snapshot
        /// </summary>
        HistoricalFundingLoanSnapshot,

        /// <summary>
        /// Historical funding trade snapshot
        /// </summary>
        HistoricalFundingTradeSnapshot,

        /// <summary>
        /// Custom user price alert
        /// </summary>
        UserCustomPriceAlert
    }
}
