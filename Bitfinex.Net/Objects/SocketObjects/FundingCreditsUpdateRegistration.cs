using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class FundingCreditsUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexFundingCredit[]> handler;

        public FundingCreditsUpdateRegistration(Action<BitfinexFundingCredit[]> handler, int streamId) : base(typeof(BitfinexFundingCredit), streamId, "fcs", "fcn", "fcu", "fcc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexFundingCredit[])obj);
        }
    }
}
