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
        P0,
        P1,
        P2,
        P3,
        R0
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

    public enum WalletType2
    {
        Exchange,
        Trading,
        Deposit
    }

    public enum OrderType
    {
        Limit,
        Market,
        Stop,
        TrailingStop,
        ExchangeMarket,
        ExchangeLimit,
        ExchangeStop,
        ExchangeTrailingStop,
        FOK,
        ExchangeFOK
    }

    public enum OrderStatus
    {
        Active,
        Executed,
        PartiallyFilled,
        Canceled
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

    public enum WithdrawType
    {
        Bitcoin,
        Litecoin,
        Ethereum,
        EthereumClassic,
        MasterCoin,
        ZCash,
        Monero,
        Wire,
        Dash,
        Ripple,
        EOS,
        Neo,
        Aventus,
        QTUM,
        Eidoo,
    }

    public enum UseExpressWire
    {
        Yes,
        No
    }

    public enum OrderSide
    {
        Buy,
        Sell
    }

    public enum OrderType2
    {
        Market,
        Limit,
        Stop,
        TrailingStop,
        FillOrKill,
        ExchangeMarket,
        ExchangeLimit,
        ExchangeStop,
        ExchangeTrailingStop,
        ExchangeFillOrKill
    }

    public enum WithdrawalDespositType
    {
        Withdrawal,
        Deposit
    }

    public enum SplitMerge
    {
        Split,
        Merge
    }
}
