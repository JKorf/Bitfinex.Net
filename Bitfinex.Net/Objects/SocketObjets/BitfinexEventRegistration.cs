using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.SocketObjets
{
    public class BitfinexEventRegistration
    {
        public long Id { get; set; }
        public List<string> EventTypes { get; set; }
        public string ChannelName { get; set; }
        public long ChannelId { get; set; }

        public ManualResetEvent CompleteEvent { get; } = new ManualResetEvent(false);

        private BitfinexError error;
        public BitfinexError Error
        {
            get { return error; }
            set
            {
                error = value;
                CompleteEvent.Set();
            }
        }

        private bool confirmed;
        public bool Confirmed
        {
            get { return confirmed; }
            set
            {
                confirmed = value;
                if (confirmed)
                    CompleteEvent.Set();
            }
        }
    }

    public class BitfinexWalletSnapshotEventRegistration: BitfinexEventRegistration
    {
        public Action<BitfinexWallet[]> Handler { get; set; }
    }

    public class BitfinexOrderSnapshotEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexOrder[]> Handler { get; set; }
    }

    public class BitfinexPositionsSnapshotEventRegistration: BitfinexEventRegistration
    {
        public Action<BitfinexPosition[]> Handler { get; set; }
    }

    public class BitfinexFundingOffersSnapshotEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexFundingOffer[]> Handler { get; set; }
    }

    public class BitfinexFundingCreditsSnapshotEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexFundingCredit[]> Handler { get; set; }
    }

    public class BitfinexFundingLoansSnapshotEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexFundingLoan[]> Handler { get; set; }
    }



    public class BitfinexTradingPairTickerEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexSocketTradingPairTick> Handler { get; set; }
    }

    public class BitfinexFundingPairTickerEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexSocketFundingPairTick> Handler { get; set; }
    }

    public class BitfinexTradeEventRegistration : BitfinexEventRegistration
    {
        public Action<BitfinexTradeSimple[]> Handler { get; set; }
    }
}
