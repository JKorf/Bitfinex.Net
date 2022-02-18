using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Enums;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexEvent
    {
        public string Id { get; set; }
        public BitfinexEventType EventType { get; set; }
        public string Category { get; set; }
        public bool Single { get; set; }

        public BitfinexEvent(string id, BitfinexEventType type, string category, bool single)
        {
            Id = id;
            EventType = type;
            Category = category;
            Single = single;
        }
    }

    internal static class BitfinexEvents
    {
        public static List<BitfinexEvent> Events = new List<BitfinexEvent>
        {
            new BitfinexEvent("hb", BitfinexEventType.HeartBeat, "Heartbeat", true),

            new BitfinexEvent("bu", BitfinexEventType.BalanceUpdate, "Balance", true),

            new BitfinexEvent("ps", BitfinexEventType.PositionSnapshot, "Positions", false),
            new BitfinexEvent("pn", BitfinexEventType.PositionNew, "Positions", true),
            new BitfinexEvent("pu", BitfinexEventType.PositionUpdate, "Positions", true),
            new BitfinexEvent("pc", BitfinexEventType.PositionClose, "Positions", true),

            new BitfinexEvent("ws", BitfinexEventType.WalletSnapshot, "Wallet", false),
            new BitfinexEvent("wu", BitfinexEventType.WalletUpdate, "Wallet", true),

            new BitfinexEvent("on", BitfinexEventType.OrderNew, "Orders", true),
            new BitfinexEvent("ou", BitfinexEventType.OrderUpdate, "Orders", true),
            new BitfinexEvent("oc", BitfinexEventType.OrderCancel, "Orders", true),
            new BitfinexEvent("os", BitfinexEventType.OrderSnapshot, "Orders", false),

            new BitfinexEvent("te", BitfinexEventType.TradeExecuted, "Trades", true),
            new BitfinexEvent("tu", BitfinexEventType.TradeExecutionUpdate, "Trades", true),

            new BitfinexEvent("foc", BitfinexEventType.FundingOfferCancel, "FundingOffers", true),
            new BitfinexEvent("fon", BitfinexEventType.FundingOfferNew, "FundingOffers", true),
            new BitfinexEvent("fos", BitfinexEventType.FundingOfferSnapshot, "FundingOffers", false),
            new BitfinexEvent("fou", BitfinexEventType.FundingOfferUpdate, "FundingOffers", true),

            new BitfinexEvent("fcc", BitfinexEventType.FundingCreditsClose, "FundingCredits", true),
            new BitfinexEvent("fcn", BitfinexEventType.FundingCreditsNew, "FundingCredits", true),
            new BitfinexEvent("fcu", BitfinexEventType.FundingCreditsUpdate, "FundingCredits", true),
            new BitfinexEvent("fcs", BitfinexEventType.FundingCreditsSnapshot, "FundingCredits", false),

            new BitfinexEvent("flc", BitfinexEventType.FundingLoanClose, "FundingLoans", true),
            new BitfinexEvent("fln", BitfinexEventType.FundingLoanNew, "FundingLoans", true),
            new BitfinexEvent("flu", BitfinexEventType.FundingLoanUpdate, "FundingLoans", true),
            new BitfinexEvent("fls", BitfinexEventType.FundingLoanSnapshot, "FundingLoans", false)

        };

        public static IEnumerable<BitfinexEvent> GetEventsForCategory(string cat)
        {
            return Events.Where(e => e.Category == cat);
        }

        public static Dictionary<string, BitfinexEventType> EventMapping = new Dictionary<string, BitfinexEventType>
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
            { "ou-req", BitfinexEventType.OrderUpdateRequest },
            { "oc", BitfinexEventType.OrderCancel },
            { "oc-req", BitfinexEventType.OrderCancelRequest },
            { "oc_multi", BitfinexEventType.OrderCancelMulti },
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
            { "uac", BitfinexEventType.UserCustomPriceAlert }
        };
    }
}
