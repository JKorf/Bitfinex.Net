using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class FundingOffersUpdateRegistration: SubscriptionRegistration
    {
        private Action<BitfinexFundingOffer[]> handler;

        public FundingOffersUpdateRegistration(Action<BitfinexFundingOffer[]> handler, int streamId) : base(typeof(BitfinexFundingOffer), streamId, "fos", "fon", "fou", "foc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexFundingOffer[])obj);
        }
    }
}
