using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects2;
using Bitfinex.Net.Objects.SocketObjets;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    public partial class BitfinexSocketClient2
    {
        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString(CultureInfo.InvariantCulture);
        private bool authenticated;
        private List<SubscriptionRegistration> registrations = new List<SubscriptionRegistration>();

        public override void SetApiCredentials(string apiKey, string apiSecret)
        {
            base.SetApiCredentials(apiKey, apiSecret);
            Authenticate();
        }

        private void Authenticate()
        {
            var n = nonce;
            var authentication = new BitfinexAuthentication()
            {
                Event = "auth",
                ApiKey = apiKey,
                Nonce = n,
                Payload = "AUTH" + n
            };
            authentication.Signature = ByteToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(authentication.Payload)));

            Send(JsonConvert.SerializeObject(authentication));
        }

        private void ProcessAuthenticationResponse(BitfinexAuthenticationResponse response)
        {
            if (response.Status == "OK")
            {
                authenticated = true;
                log.Write(LogVerbosity.Debug, "Authenticated");
            }
            else
            {
                authenticated = false;
                log.Write(LogVerbosity.Warning, "Authentication failed: " + response.ErrorMessage);
            }
        }

        public void SubscribeToWalletUpdates(Action<BitfinexWallet[]> handler)
        {
            registrations.Add(new WalletUpdateRegistration(handler));
        }
    }
}
