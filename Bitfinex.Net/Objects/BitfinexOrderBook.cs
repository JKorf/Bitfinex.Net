using System;
using System.Collections.Generic;
using System.Text;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Objects
{
    public class BitfinexOrderBook: ICommonOrderBook
    {
        public IEnumerable<ISymbolOrderBookEntry> Bids { get; set; }
        public IEnumerable<ISymbolOrderBookEntry> Asks { get; set; }

        IEnumerable<ISymbolOrderBookEntry> ICommonOrderBook.CommonBids => Bids;
        IEnumerable<ISymbolOrderBookEntry> ICommonOrderBook.CommonAsks => Asks;
    }
}
