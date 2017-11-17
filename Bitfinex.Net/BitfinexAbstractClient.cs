using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects;

namespace Bitfinex.Net
{
    public abstract class BitfinexAbstractClient: IDisposable
    {
        protected string apiKey;
        protected HMACSHA384 encryptor;
        internal Log log;

        protected BitfinexAbstractClient()
        {
            log = new Log();

            if (BitfinexDefaults.LogWriter != null)
                SetLogOutput(BitfinexDefaults.LogWriter);

            if (BitfinexDefaults.LogVerbosity != null)
                SetLogVerbosity(BitfinexDefaults.LogVerbosity.Value);

            if (BitfinexDefaults.ApiKey != null && BitfinexDefaults.ApiSecret != null)
                SetApiCredentials(BitfinexDefaults.ApiKey, BitfinexDefaults.ApiSecret);
        }

        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetApiKey(apiKey);
            SetApiSecret(apiSecret);
        }

        /// <summary>
        /// Sets the API Key. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiKey">The api key</param>
        public void SetApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Api key empty");

            this.apiKey = apiKey;
        }

        /// <summary>
        /// Sets the API Secret. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiSecret(string apiSecret)
        {
            if (string.IsNullOrEmpty(apiSecret))
                throw new ArgumentException("Api secret empty");

            encryptor = new HMACSHA384(Encoding.ASCII.GetBytes(apiSecret));
        }

        /// <summary>
        /// Sets the verbosity of the log messages
        /// </summary>
        /// <param name="verbosity">Verbosity level</param>
        public void SetLogVerbosity(LogVerbosity verbosity)
        {
            log.Level = verbosity;
        }

        /// <summary>
        /// Sets the log output
        /// </summary>
        /// <param name="writer">The output writer</param>
        public void SetLogOutput(TextWriter writer)
        {
            log.TextWriter = writer;
        }

        protected BitfinexApiResult<T> ThrowErrorMessage<T>(BitfinexError error)
        {
            return ThrowErrorMessage<T>(error, null);
        }

        protected BitfinexApiResult<T> ThrowErrorMessage<T>(BitfinexError error, string extraInformation)
        {
            log.Write(LogVerbosity.Warning, $"Call failed: {error.ErrorMessage}");
            var result = (BitfinexApiResult<T>)Activator.CreateInstance(typeof(BitfinexApiResult<T>));
            result.Error = error;
            if (extraInformation != null)
                result.Error.ErrorMessage += Environment.NewLine + extraInformation;
            return result;
        }

        protected BitfinexApiResult<T> ReturnResult<T>(T data)
        {
            var result = (BitfinexApiResult<T>)Activator.CreateInstance(typeof(BitfinexApiResult<T>));
            result.Result = data;
            result.Success = true;
            return result;
        }

        protected string ByteToString(byte[] buff)
        {
            var sbinary = "";
            foreach (byte t in buff)
                sbinary += t.ToString("x2"); /* hex format */
            return sbinary;
        }

        public void Dispose()
        {
            encryptor?.Dispose();
        }
    }
}
