using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public abstract class SubscriptionRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonIgnore]
        public Type EventType { get; set; }
        [JsonIgnore]
        public bool EventTypeAnnounced { get; set; }
        [JsonIgnore]
        public int ChannelId { get; set; }
        [JsonIgnore]
        public ManualResetEvent ConfirmedEvent { get; }

        protected SubscriptionRequest()
        {
            Event = "subscribe";
            var attr = ((SubscriptionChannelAttribute) GetType().GetCustomAttributes(typeof(SubscriptionChannelAttribute), true)[0]);
            Channel = attr.ChannelName;
            EventType = attr.EventType;
            EventTypeAnnounced = attr.EventTypeAnnounced;
            ConfirmedEvent = new ManualResetEvent(false);
        }

        public string GetSubscriptionKey()
        {
            return Channel + GetSubscriptionSubKey();
        }
        
        protected abstract string GetSubscriptionSubKey();

        public void Handle(JArray obj)
        {
            JToken data;
            if (obj[1] is JArray)
                data = obj[1];
            else
                data = EventTypeAnnounced ? obj[2] : obj[1];

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

    public abstract class SubscriptionResponse
    {
        public string Event { get; set; }
        public string Channel { get; set; }
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }

        public string GetSubscriptionKey()
        {
            return Channel + GetSubsciptionSubKey();
        }

        protected abstract string GetSubsciptionSubKey();
    }

    public class SubscriptionChannelAttribute : Attribute
    {
        public string ChannelName { get; }
        public Type EventType { get; }
        public bool EventTypeAnnounced { get; }

        public SubscriptionChannelAttribute(string channelName)
        {
            ChannelName = channelName;
        }

        public SubscriptionChannelAttribute(string channelName, Type eventType, bool eventTypeAnnounced)
        {
            ChannelName = channelName;
            EventType = eventType;
            EventTypeAnnounced = eventTypeAnnounced;
        }
    }
}
