using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal class BitfinexSocketClientSpotApiMessageIdentifier : DynamicJsonConverter
    {
        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(BitfinexExchange._serializerContext);


        // Message types:
        // Welcome:      { "event": "info", "version":  VERSION,"platform": { "status": 1 } }
        // Pong:         { "event":"pong", "ts": 1511545528111, "cid": 1234}
        // Sub response: { "event": "subscribed", "channel": CHANNEL_NAME, "chanId": CHANNEL_ID }

        // Heartbeat:    [ CHANNEL_ID, "hb" ]                                               CHANNEL_ID + [1]
        // Ticker:       [ CHANNEL_ID,  [ 7616.5, etc ] ]                                   CHANNEL_ID + single
        // Trade snap:   [ CHANNEL_ID,  [ [ 401597393, etc ], [ 401597394, etc ] ] ]        CHANNEL_ID + array
        // Trade upd:    [ CHANNEL_ID, "te", [ 401597393, etc ] ]                           CHANNEL_ID + array

        // Account snap: [ 0, "os", [ [ 401597393, etc ] ] ]                                0 + [1]
        // Account upd:  [ 0, "on", [ 401597393, etc ] ]                                    0 + [1]

        // Account Mar 1: [ 0, "miu", [ "base", [ -13.014640000000007, etc ] ]              0 + [1] + [2].[0]
        // Account Mar 2: [ 0, "miu", [ "sym", "tETHUST", [ -13.014640000000007, etc ] ]    0 + [1] + [2].[0]


        protected override MessageEvaluator[] MessageEvaluators { get; } = [

            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference 
                    { 
                        Depth = 1, 
                        PropertyName = "event", 
                        Constraint = x => x == "info"
                    },
                ],
                MessageIdentifier = x => x["event"],
            },


            // Account Margin update
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference
                    {
                        PropertyName = "id",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 0,
                        Constraint = x => x == "0"
                    },
                    new MessageFieldReference
                    {
                        PropertyName = "topic",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 1,
                        Constraint = x => x == "miu"
                    },
                    new MessageFieldReference
                    {
                        PropertyName = "marginType",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 2,
                        ArrayIndex = 0,
                    },
                ],
                MessageIdentifier = x =>  x["id"] + x["topic"]+ x["topic"]
            },

            // Account update
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference 
                    { 
                        SearchName = "id", 
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 0,
                        Constraint = x => x == "0"
                    },
                    new MessageFieldReference 
                    {
                        SearchName = "type",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1, 
                        ArrayIndex = 1,
                        Constraint = x => x != "miu"
                    },
                ],
                MessageIdentifier = x =>  x["id"] + x["type"]
            },

            // Heartbeat/checksum
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference
                    {
                        SearchName = "id",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 0,
                        Constraint = (x) => x != "0"
                    },
                    new MessageFieldReference
                    {
                        SearchName = "type",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 1,
                        Constraint = x => x == "hb" || x == "cs"
                    },
                ],
                MessageIdentifier = x =>  x["id"] + x["type"]
            },

            // Trade update
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference
                    {
                        SearchName = "id",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 0,
                        Constraint = (x) => x != "0"
                    },
                    new MessageFieldReference
                    {
                        SearchName = "type",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 1,
                        Constraint = x => x == "te"
                    },
                ],
                MessageIdentifier = x =>  x["id"] + "single"
            },

            // Single update
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference
                    {
                        SearchName = "id",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 0,
                        Constraint = (x) => x != "0"
                    },
                    new MessageFieldReference
                    {
                        SearchName = "type",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 2,
                        ArrayIndex = 0
                    },
                ],
                MessageIdentifier = x =>  x["id"] + "single"
            },

            // Array update
            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new MessageFieldReference
                    {
                        SearchName = "id",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 1,
                        ArrayIndex = 0,
                        Constraint = (x) => x != "0"
                    },
                    new MessageFieldReference
                    {
                        SearchName = "type",
                        FieldType = FieldType.ArrayIndex,
                        Depth = 3,
                        ArrayIndex = 0,
                    },
                ],
                MessageIdentifier = x =>  x["id"] + "array"
            },
        ];
    }
}
