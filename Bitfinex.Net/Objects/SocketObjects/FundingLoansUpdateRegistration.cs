using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class FundingLoansUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexFundingLoan[]> handler;

        public FundingLoansUpdateRegistration(Action<BitfinexFundingLoan[]> handler) : base(typeof(BitfinexFundingLoan), "fls", "fln", "flu", "flc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexFundingLoan[])obj);
        }
    }
}
