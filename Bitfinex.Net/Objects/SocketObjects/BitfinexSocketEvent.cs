using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class BitfinexSocketEvent<T>
    {
        public BitfinexEventType EventType { get; }
        public T Data { get; }

        public BitfinexSocketEvent(BitfinexEventType type, T data)
        {
            EventType = type;
            Data = data;
        }
    }
}
