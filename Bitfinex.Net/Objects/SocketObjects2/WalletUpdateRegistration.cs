using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class WalletUpdateRegistration: SubscriptionRegistration
    {
        private Action<BitfinexWallet[]> handler;

        public WalletUpdateRegistration(Action<BitfinexWallet[]> handler) : base(typeof(BitfinexWallet), "ws", "wu")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexWallet[]) obj);
        }
    }
}
