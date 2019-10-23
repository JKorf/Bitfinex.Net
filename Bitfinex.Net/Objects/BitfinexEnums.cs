using System;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Status of Bitfinex
    /// </summary>
    public enum PlatformStatus
    {
        /// <summary>
        /// In maintenance
        /// </summary>
        Maintenance,
        /// <summary>
        /// Working normally
        /// </summary>
        Operative
    }

    /// <summary>
    /// Order
    /// </summary>
    public enum Sorting
    {
        /// <summary>
        /// Newest first
        /// </summary>
        NewFirst,
        /// <summary>
        /// Oldest first
        /// </summary>
        OldFirst
    }

    /// <summary>
    /// Precision level
    /// </summary>
    public enum Precision
    {
        /// <summary>
        /// 0
        /// </summary>
        PrecisionLevel0,
        /// <summary>
        /// 1
        /// </summary>
        PrecisionLevel1,
        /// <summary>
        /// 2
        /// </summary>
        PrecisionLevel2,
        /// <summary>
        /// 3
        /// </summary>
        PrecisionLevel3,
        /// <summary>
        /// R0
        /// </summary>
        R0
    }

    /// <summary>
    /// Frequency of updates
    /// </summary>
    public enum Frequency
    {
        /// <summary>
        /// Realtime
        /// </summary>
        Realtime,
        /// <summary>
        /// Two seconds
        /// </summary>
        TwoSeconds
    }

    /// <summary>
    /// Stat types
    /// </summary>
    public enum StatKey
    {
        /// <summary>
        /// Total number of open positions
        /// </summary>
        TotalOpenPosition,
        /// <summary>
        /// Total active funding
        /// </summary>
        TotalActiveFunding,
        /// <summary>
        /// Active funding in positions
        /// </summary>
        ActiveFundingInPositions,
        /// <summary>
        /// Active funding positions per symbol
        /// </summary>
        ActiveFundingInPositionsPerTradingSymbol
    }

    /// <summary>
    /// Side for stats
    /// </summary>
    public enum StatSide
    {
        /// <summary>
        /// Long
        /// </summary>
        Long,
        /// <summary>
        /// Short
        /// </summary>
        Short
    }

    /// <summary>
    /// Section of stats
    /// </summary>
    public enum StatSection
    {
        /// <summary>
        /// Last
        /// </summary>
        Last,
        /// <summary>
        /// History
        /// </summary>
        History
    }

    /// <summary>
    /// Interval
    /// </summary>
    public enum TimeFrame
    {
        /// <summary>
        /// 1m
        /// </summary>
        OneMinute,
        /// <summary>
        /// 5m
        /// </summary>
        FiveMinute,
        /// <summary>
        /// 15m
        /// </summary>
        FifteenMinute,
        /// <summary>
        /// 30m
        /// </summary>
        ThirtyMinute,
        /// <summary>
        /// 1h
        /// </summary>
        OneHour,
        /// <summary>
        /// 3h
        /// </summary>
        ThreeHour,
        /// <summary>
        /// 6h
        /// </summary>
        SixHour,
        /// <summary>
        /// 12h
        /// </summary>
        TwelveHour,
        /// <summary>
        /// 1d
        /// </summary>
        OneDay,
        /// <summary>
        /// 7d
        /// </summary>
        SevenDay,
        /// <summary>
        /// 14d
        /// </summary>
        FourteenDay,
        /// <summary>
        /// 1m
        /// </summary>
        OneMonth
    }

    /// <summary>
    /// Type of wallet
    /// </summary>
    public enum WalletType
    {
        /// <summary>
        /// Exchange
        /// </summary>
        Exchange,
        /// <summary>
        /// Margin
        /// </summary>
        Margin,
        /// <summary>
        /// Funding
        /// </summary>
        Funding
    }

    /// <summary>
    /// Order types
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,
        /// <summary>
        /// Market order
        /// </summary>
        Market,
        /// <summary>
        /// Stop order
        /// </summary>
        Stop,
        /// <summary>
        /// Stop limit order
        /// </summary>
        StopLimit,
        /// <summary>
        /// Trailing stop order
        /// </summary>
        TrailingStop,
        /// <summary>
        /// Exchange market order
        /// </summary>
        ExchangeMarket,
        /// <summary>
        /// Exchange limit order
        /// </summary>
        ExchangeLimit,
        /// <summary>
        /// Exchange stop order
        /// </summary>
        ExchangeStop,
        /// <summary>
        /// Exchange stop limit order
        /// </summary>
        ExchangeStopLimit,
        /// <summary>
        /// Exchange trailing stop order
        /// </summary>
        ExchangeTrailingStop,
        /// <summary>
        /// Fill or kill order
        /// </summary>
        FillOrKill,
        /// <summary>
        /// Exchange fill or kill order
        /// </summary>
        ExchangeFillOrKill
    }

    /// <summary>
    /// Order types for V1 API
    /// </summary>
    public enum OrderTypeV1
    {
        /// <summary>
        /// Market order
        /// </summary>
        Market,
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,
        /// <summary>
        /// Stop order
        /// </summary>
        Stop,
        /// <summary>
        /// Stop limit order
        /// </summary>
        StopLimit,
        /// <summary>
        /// Trailing stop order
        /// </summary>
        TrailingStop,
        /// <summary>
        /// Fill or kill order
        /// </summary>
        FillOrKill,
        /// <summary>
        /// Exchange market order
        /// </summary>
        ExchangeMarket,
        /// <summary>
        /// Exchange limit order
        /// </summary>
        ExchangeLimit,
        /// <summary>
        /// Exchange stop order
        /// </summary>
        ExchangeStop,
        /// <summary>
        /// Exchange stop limit order
        /// </summary>
        ExchangeStopLimit,
        /// <summary>
        /// Exchange trailing stop order
        /// </summary>
        ExchangeTrailingStop,
        /// <summary>
        /// Exchange fill or kill order
        /// </summary>
        ExchangeFillOrKill
    }

    /// <summary>
    /// Status of an order
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        Active,
        /// <summary>
        /// Fully filled
        /// </summary>
        Executed,
        /// <summary>
        /// Partially filled
        /// </summary>
        PartiallyFilled,
        /// <summary>
        /// Cancelled
        /// </summary>
        Canceled,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Position status
    /// </summary>
    public enum PositionStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        Active,
        /// <summary>
        /// Closed
        /// </summary>
        Closed
    }

    /// <summary>
    /// Margin funding type
    /// </summary>
    public enum MarginFundingType
    {
        /// <summary>
        /// Daily
        /// </summary>
        Daily,
        /// <summary>
        /// Term
        /// </summary>
        Term
    }

    /// <summary>
    /// Funding type
    /// </summary>
    public enum FundingType
    {
        /// <summary>
        /// Lend
        /// </summary>
        Lend,
        /// <summary>
        /// Loan
        /// </summary>
        Loan
    }
    
    /// <summary>
    /// Side of an order
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// Buy
        /// </summary>
        Buy,
        /// <summary>
        /// Sell
        /// </summary>
        Sell
    }

    /// <summary>
    /// Flags for an order
    /// </summary>
    [Flags]
    public enum OrderFlags
    {
        /// <summary>
        /// Order is hidden
        /// </summary>
        Hidden = 64,
        /// <summary>
        /// Close
        /// </summary>
        Close = 512,
        /// <summary>
        /// Only accept if it is a post
        /// </summary>
        PostOnly = 4096,
        /// <summary>
        /// Oco order
        /// </summary>
        OneCancelsOther = 16384
    }
    
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
    
    /// <summary>
    /// Withdraw wallet type
    /// </summary>
    public enum WithdrawWallet
    {
        /// <summary>
        /// Trading
        /// </summary>
        Trading,
        /// <summary>
        /// Exchange
        /// </summary>
        Exchange,
        /// <summary>
        /// Deposit
        /// </summary>
        Deposit
    }
}
