using System;

namespace Bitfinex.Net.Objects
{
    public enum PlatformStatus
    {
        Maintenance,
        Operative
    }

    public enum Sorting
    {
        NewFirst,
        OldFirst
    }

    public enum Precision
    {
        PrecisionLevel0,
        PrecisionLevel1,
        PrecisionLevel2,
        PrecisionLevel3,
        R0
    }

    public enum Frequency
    {
        Realtime,
        TwoSeconds
    }

    public enum StatKey
    {
        TotalOpenPosition,
        TotalActiveFunding,
        ActiveFundingInPositions,
        ActiveFundingInPositionsPerTradingSymbol,
    }

    public enum StatSide
    {
        Long,
        Short
    }

    public enum StatSection
    {
        Last,
        History
    }

    public enum TimeFrame
    {
        OneMinute,
        FiveMinute,
        FiveteenMinute,
        ThirtyMinute,
        OneHour,
        ThreeHour,
        SixHour,
        TwelfHour,
        OneDay,
        SevenDay,
        FourteenDay,
        OneMonth
    }

    public enum WalletType
    {
        Exchange,
        Margin,
        Funding
    }

    public enum OrderType
    {
        Limit,
        Market,
        Stop,
        StopLimit,
        TrailingStop,
        ExchangeMarket,
        ExchangeLimit,
        ExchangeStop,
        ExchangeStopLimit,
        ExchangeTrailingStop,
        FillOrKill,
        ExchangeFillOrKill
    }

    public enum OrderTypeV1
    {
        Market,
        Limit,
        Stop,
        StopLimit,
        TrailingStop,
        FillOrKill,
        ExchangeMarket,
        ExchangeLimit,
        ExchangeStop,
        ExchangeStopLimit,
        ExchangeTrailingStop,
        ExchangeFillOrKill
    }

    public enum OrderStatus
    {
        Active,
        Executed,
        PartiallyFilled,
        Canceled,
        Unknown
    }

    public enum PositionStatus
    {
        Active,
        Closed
    }

    public enum MarginFundingType
    {
        Daily,
        Term
    }

    public enum FundingType
    {
        Lend,
        Loan
    }

    public enum DepositMethod
    {
        Bitcoin,
        Litecoin,
        Ethereum,
        Tether,
        EthereumClassic,
        ZCash,
        Monero,
        Iota,
        BCash
    }
    
    public enum OrderSide
    {
        Buy,
        Sell
    }

    [Flags]
    public enum OrderFlags
    {
        Hidden = 64,
        Close = 512,
        PostOnly = 4096,
        OneCancelsOther = 16384
    }

    public enum SocketState
    {
        Disconnected,
        Connecting,
        Connected,
        Paused,
        Disconnecting
    }

    public enum BitfinexEventType
    {
        HeartBeat,

        BalanceUpdate,

        PositionSnapshot,
        PositionNew,
        PositionUpdate,
        PositionClose,

        WalletSnapshot,
        WalletUpdate,

        OrderSnapshot,
        OrderNew,
        OrderNewRequest,
        OrderUpdate,
        OrderCancel,
        OrderCancelRequest,
        OrderCancelMultiRequest,

        TradeExecuted,
        TradeExecutionUpdate,

        FundingTradeExecution,
        FundingTradeUpdate,
        
        HistoricalOrderSnapshot,

        MarginInfoSnapshot,
        MarginInfoUpdate,

        Notification,

        FundingOfferSnapshot,
        FundingOfferNew,
        FundingOfferUpdate,
        FundingOfferCancel,

        HistoricalFundingOfferSnapshot,
        
        FundingCreditsSnapshot,
        FundingCreditsNew,
        FundingCreditsUpdate,
        FundingCreditsClose,

        HistoricalFundingCreditsSnapshot,

        FundingLoanSnapshot,
        FundingLoanNew,
        FundingLoanUpdate,
        FundingLoanClose,

        HistoricalFundingLoanSnapshot,
        
        HistoricalFundingTradeSnapshot,

        UserCustomPriceAlert
    }

    public enum WithdrawalType
    {
        Bitcoin,
        Litecoin,
        Ethereum,
        EthereumClassic,
        TetherUSO,
        TetherUSE,
        TetherEUE,
        ZCash,
        Monero,
        IOTA,
        Ripple,
        Dash,
        Adjustment,
        Wire,
        EOS,
        Santiment,
        OmiseGo,
        BitcoinCash,
        Neo,
        MetaVerse,
        QTUM,
        Aventus,
        Eidoo,
        Datacoin,
        BitcoinGold,
        Qash,
        Yoyow,
        Golem,
        Status,
        Bat,
        MNA,
        Fun,
        ZRX,
        TNB,
        SPK,
        TRX,
        RCN,
        RLC,
        AID,
        SNG,
        REP,
        ELF
    }

    public enum WithdrawWallet
    {
        Trading,
        Exchange,
        Deposit
    }
}
