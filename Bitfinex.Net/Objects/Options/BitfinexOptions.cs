using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Bitfinex options
    /// </summary>
    public class BitfinexOptions : LibraryOptions<BitfinexRestOptions, BitfinexSocketOptions, ApiCredentials, BitfinexEnvironment>
    {
    }
}
