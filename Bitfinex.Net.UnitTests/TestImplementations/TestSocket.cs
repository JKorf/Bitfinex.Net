using System;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace Binance.Net.UnitTests.TestImplementations
{
    public class TestSocket: IWebsocket
    {
        public bool CanConnect { get; set; }
        public bool Connected { get; set; }

        public event Action OnClose;
        public event Action<string> OnMessage;
        public event Action<Exception> OnError;
        public event Action OnOpen;

        public int Id { get; }
        public bool ShouldReconnect { get; set; }
        public Func<string, string> DataInterpreterString { get; set; }
        public Func<byte[], string> DataInterpreterBytes { get; set; }
        public DateTime? DisconnectTime { get; set; }
        public string Url { get; set; }
        public bool IsClosed => !Connected;
        public bool IsOpen => Connected;
        public bool PingConnection { get; set; }
        public TimeSpan PingInterval { get; set; }
        public SslProtocols SSLProtocols { get; set; }
        public TimeSpan Timeout { get; set; }
        public string Origin { get; set; }
        public bool Reconnecting { get; set; }
        public Encoding Encoding { get; set; }

        public TimeSpan CloseTime { get; set; }
        public TimeSpan OpenTime { get; set; }
        public int? RatelimitPerSecond { get; set; }

        public async Task<bool> ConnectAsync()
        {
            await Task.Delay(OpenTime);
            Connected = CanConnect;
            OnOpen?.Invoke();
            return true;
        }

        public void Send(string data)
        {
            if(!Connected)
                throw new Exception("Socket not connected");
        }

        public void Reset()
        {
            
        }

        public async Task CloseAsync()
        {
            await Task.Delay(CloseTime);

            Connected = false;
            DisconnectTime = DateTime.UtcNow;
            OnClose?.Invoke();
        }

        public void SetProxy(ApiProxy proxy)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
        }

        public void InvokeClose()
        {
            Connected = false;
            DisconnectTime = DateTime.UtcNow;
            OnClose?.Invoke();
        }

        public void InvokeOpen()
        {
            OnOpen?.Invoke();
        }

        public void InvokeMessage(string data)
        {
            OnMessage?.Invoke(data);
        }

        public void InvokeMessage<T>(T data)
        {
            OnMessage?.Invoke(JsonConvert.SerializeObject(data));
        }
    }
}
