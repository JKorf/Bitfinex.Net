using System;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public abstract class SubscriptionRegistration
    {
        public string[] UpdateKeys { get; set; }
        public Type EventType { get; set; }

        public SubscriptionRegistration(Type eventType, params string[] updateKeys)
        {
            EventType = eventType;
            UpdateKeys = updateKeys;
        }

        public void Handle(JArray obj)
        {
            var data = obj[2];
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
