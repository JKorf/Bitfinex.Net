using Bitfinex.Net;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.SystemTextJson.MessageConverters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Binance.Net.Clients.MessageHandlers
{
    internal class BitfinexRestMessageHandler : JsonRestMessageHandler
    {
        private readonly ErrorMapping _errorMapping;

        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(BitfinexExchange._serializerContext);

        public BitfinexRestMessageHandler(ErrorMapping errorMapping)
        {
            _errorMapping = errorMapping;
        }

        public override async ValueTask<Error> ParseErrorResponse(
            int httpStatusCode,
            HttpResponseHeaders responseHeaders,
            Stream responseStream)
        {
            var (error, document) = await GetJsonDocument(responseStream).ConfigureAwait(false);
            if (error != null)
                return error;

            if (document!.RootElement.ValueKind == JsonValueKind.Array)
            {
                var code = document.RootElement[1].GetInt32();
                var msg = document.RootElement[2].GetString();
                return new ServerError(code.ToString(), _errorMapping.GetErrorInfo(code.ToString(), msg));
            }
            else
            {
                var errorMsg = document.RootElement.TryGetProperty("msg", out var msgProp) ? msgProp.GetString() : null;
                int? errorCode = document!.RootElement.TryGetProperty("code", out var codeProp) ? codeProp.GetInt32() : null;
                var errorDesc = document.RootElement.TryGetProperty("error_description", out var descProp) ? descProp.GetString() : null;
                if (error != null && errorCode != null && errorDesc != null)
                    return new ServerError(errorCode.Value.ToString(), _errorMapping.GetErrorInfo(errorCode.Value.ToString(), $"{error}: {errorDesc}"));

                var message = document.RootElement.TryGetProperty("message", out var messageProp) ? messageProp.GetString() : null;
                if (message != null)
                    return new ServerError(ErrorInfo.Unknown with { Message = message });

                return new ServerError(ErrorInfo.Unknown);
            }
        }
    }
}
