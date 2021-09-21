using CryptoExchange.Net.Interfaces;
using System;

namespace Bitfinex.Net.Objects
{
    internal class BitfinexNonceProvider : INonceProvider
    {
        private static readonly object nonceLock = new object();
        private static long? lastNonce;

        /// <inheritdoc />
        public long GetNonce()
        {
            lock (nonceLock)
            {
                long nonce;
                if (lastNonce == null)
                    nonce = (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000);
                else
                    nonce = lastNonce.Value + 1;
                lastNonce = nonce;
                return nonce;
            }
        }
    }
}
