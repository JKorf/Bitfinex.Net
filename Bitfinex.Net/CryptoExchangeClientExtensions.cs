using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Interfaces.CommonClients;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoExchange.Net.Clients
{
    public static class CryptoExchangeClientExtensions
    {
        public static IBitfinexRestClient Bitfinex(this ICryptoExchangeClient baseClient) => baseClient.TryGet<IBitfinexRestClient>() ?? new BitfinexRestClient();
    }
}
