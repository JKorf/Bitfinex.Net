using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class FundingOffersUpdateRegistration: SubscriptionRegistration
    {
        private Action<BitfinexFundingOffer[]> handler;

        public FundingOffersUpdateRegistration(Action<BitfinexFundingOffer[]> handler) : base(typeof(BitfinexFundingOffer), "fos", "fon", "fou", "foc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexFundingOffer[])obj);
        }
    }
}
