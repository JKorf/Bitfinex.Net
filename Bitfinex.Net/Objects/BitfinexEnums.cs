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
}
