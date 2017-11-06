using System;

namespace Bitfinex.Net.Converters
{
    public class BitfinexPropertyAttribute: Attribute
    {
        public int Index { get; }

        public BitfinexPropertyAttribute(int index)
        {
            Index = index;
        }
    }
}
