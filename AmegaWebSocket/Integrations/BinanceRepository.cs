using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace AmegaWebSocket.Integrations
{
    public interface IBinanceRepository
    {
        Task<string> GetPrice(string streamName);
    }

    public class BinanceRepository : IBinanceRepository
    {
        private readonly ILogger<BinanceRepository> _logger;
        private ConcurrentDictionary<string, string?> _prices = new();
        public BinanceRepository(ILogger<BinanceRepository> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetPrice(string streamName)
        {
            if (!_prices.ContainsKey(streamName))
            {
                _prices.AddOrUpdate(streamName, string.Empty, (key, oldvalue) => string.Empty);
                Task.Run(async () => await ListenStream(streamName));
            }
            return _prices.GetValueOrDefault(streamName);
        }

        private async Task ListenStream(string streamName)
        {
            var ws = new ClientWebSocket();
            var buffer = new byte[2048];
            while (true)
            {
                try
                {
                    if (ws.State != WebSocketState.Open)
                    {
                        ws = new ClientWebSocket();
                        await ws.ConnectAsync(new Uri($"wss://stream.binance.com:443/ws/{streamName}"), CancellationToken.None);
                        var bytes = Encoding.UTF8.GetBytes($"{{\"method\": \"SUBSCRIBE\",\"params\": [\"{streamName}@aggTrade\"]}}");
                        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
                    }
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var pricesText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var prices = JsonConvert.DeserializeObject<BinanceMessage>(pricesText);
                        _prices.AddOrUpdate(streamName, prices.p, (key, oldvalue) => prices.p);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _prices.AddOrUpdate(streamName, (string)null, (key, oldvalue) => null);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        private class BinanceMessage
        {
            public string p { get; set; }
        }
    }
}
