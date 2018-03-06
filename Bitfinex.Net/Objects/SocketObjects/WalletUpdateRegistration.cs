using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class WalletUpdateRegistration: SubscriptionRegistration
    {
        private Action<BitfinexWallet[]> handler;
        public int StreamId { get; set; }

        public WalletUpdateRegistration(Action<BitfinexWallet[]> handler, int streamId) : base(typeof(BitfinexWallet), streamId, "ws", "wu")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexWallet[]) obj);
        }
    }
}
