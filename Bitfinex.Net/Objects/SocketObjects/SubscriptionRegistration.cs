using System;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public abstract class SubscriptionRegistration
    {
        public string[] UpdateKeys { get; set; }
        public Type EventType { get; set; }
        public int StreamId { get; set; }

        public SubscriptionRegistration(Type eventType, int streamId, params string[] updateKeys)
        {
            EventType = eventType;
            UpdateKeys = updateKeys;
            StreamId = streamId;
        }

        public void Handle(JArray obj)
        {
            var data = obj[2];
            if (!data.HasValues)
                return;
            
            if (data[0] is JArray)
            {
                var typed = data.ToObject(EventType.MakeArrayType());
                Handle(typed);
            }
            else
            {
                var array = Array.CreateInstance(EventType, 1);
                array.SetValue(data.ToObject(EventType), 0);
                Handle(array);
            }
        }

        protected abstract void Handle(object obj);
    }
}
