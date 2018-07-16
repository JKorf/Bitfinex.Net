using System;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public abstract class SubscriptionRegistration
    {
        public BitfinexEventType[] UpdateKeys { get; set; }
        public Type EventType { get; set; }
        public int StreamId { get; set; }

        protected SubscriptionRegistration(Type eventType, int streamId, params BitfinexEventType[] updateKeys)
        {
            EventType = eventType;
            UpdateKeys = updateKeys;
            StreamId = streamId;
        }

        public void Handle(BitfinexEventType type, JArray obj)
        {
            var data = obj[2];
            if (!data.HasValues)
                return;

            
            if (data[0] is JArray)
            {
                var typed = data.ToObject(EventType.MakeArrayType());
                Handle(type, typed);
            }
            else
            {
                var array = Array.CreateInstance(EventType, 1);
                array.SetValue(data.ToObject(EventType), 0);
                Handle(type, array);
            }
        }

        protected abstract void Handle(BitfinexEventType type, object obj);
    }
}
