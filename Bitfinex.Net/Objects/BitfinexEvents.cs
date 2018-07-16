using System.Collections.Generic;

namespace Bitfinex.Net.Objects
{
    public static class BitfinexEvents
    {
        public static Dictionary<string, BitfinexEventType> EventMapping = new Dictionary<string, BitfinexEventType>()
        {
            { "hb", BitfinexEventType.HeartBeat },
            { "bu", BitfinexEventType.BalanceUpdate },
            { "ps", BitfinexEventType.PositionSnapshot },
            { "pn", BitfinexEventType.PositionNew },
            { "pu", BitfinexEventType.PositionUpdate },
            { "pc", BitfinexEventType.PositionClose },
            { "ws", BitfinexEventType.WalletSnapshot },
            { "wu", BitfinexEventType.WalletUpdate },
            { "os", BitfinexEventType.OrderSnapshot },
            { "on", BitfinexEventType.OrderNew },
            { "on-req", BitfinexEventType.OrderNewRequest },
            { "ou", BitfinexEventType.OrderUpdate },
            { "oc", BitfinexEventType.OrderCancel },
            { "oc-req", BitfinexEventType.OrderCancelRequest },
            { "oc_multi-req", BitfinexEventType.OrderCancelMultiRequest },
            { "te", BitfinexEventType.TradeExecuted },
            { "tu", BitfinexEventType.TradeExecutionUpdate },
            { "fte", BitfinexEventType.FundingTradeExecution },
            { "ftu", BitfinexEventType.FundingTradeUpdate },
            { "hos", BitfinexEventType.HistoricalOrderSnapshot },
            { "mis", BitfinexEventType.MarginInfoSnapshot },
            { "miu", BitfinexEventType.MarginInfoUpdate },
            { "n", BitfinexEventType.Notification },
            { "fos", BitfinexEventType.FundingOfferSnapshot },
            { "fon", BitfinexEventType.FundingOfferNew },
            { "fou", BitfinexEventType.FundingOfferUpdate },
            { "foc", BitfinexEventType.FundingOfferCancel },
            { "hfos", BitfinexEventType.HistoricalFundingOfferSnapshot },
            { "fcs", BitfinexEventType.FundingCreditsSnapshot },
            { "fcn", BitfinexEventType.FundingCreditsNew },
            { "fcu", BitfinexEventType.FundingCreditsUpdate },
            { "fcc", BitfinexEventType.FundingCreditsClose },
            { "hfcs", BitfinexEventType.HistoricalFundingCreditsSnapshot },
            { "fls", BitfinexEventType.FundingLoanSnapshot },
            { "fln", BitfinexEventType.FundingLoanNew },
            { "flu", BitfinexEventType.FundingLoanUpdate },
            { "flc", BitfinexEventType.FundingLoanClose },
            { "hfls", BitfinexEventType.HistoricalFundingLoanSnapshot },
            { "hfts", BitfinexEventType.HistoricalFundingTradeSnapshot },
            { "uac", BitfinexEventType.UserCustomPriceAlert },
        };
    }
}
