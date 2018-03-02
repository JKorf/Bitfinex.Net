using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class FundingCreditsUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexFundingCredit[]> handler;

        public FundingCreditsUpdateRegistration(Action<BitfinexFundingCredit[]> handler) : base(typeof(BitfinexFundingCredit), "fcs", "fcn", "fcu", "fcc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexFundingCredit[])obj);
        }
    }
}
