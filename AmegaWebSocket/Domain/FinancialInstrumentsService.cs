using AmegaWebSocket.Integrations;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AmegaWebSocket.Domain
{
    public interface IFinancialInstrumentsService
    {
        IEnumerable<string> GetAvailableInstruments();
        Task<string> GetInstrumentPrice(string instrument);
        Task UpdatePrices();
    }

    public class FinancialInstrumentsService : IFinancialInstrumentsService
    {
        private readonly IOptions<Config> _config;
        private readonly IBinanceRepository _binanceRepository;
        private static ConcurrentDictionary<string, string> _instrumentPrices = new();

        public FinancialInstrumentsService(
            IOptions<Config> config,
            IBinanceRepository binanceRepository)
        {
            _config = config;
            _binanceRepository = binanceRepository;
            foreach (var financialInstrument in config.Value.FinancialInstruments)
                _instrumentPrices.TryAdd(financialInstrument, "0");
        }

        public IEnumerable<string> GetAvailableInstruments()
        {
            return _config.Value.FinancialInstruments;
        }

        public async Task<string> GetInstrumentPrice(string instrument)
        {
            return _instrumentPrices.GetValueOrDefault(instrument);
        }

        public async Task UpdatePrices()
        {
            foreach (var instrument in _instrumentPrices.Keys)
            {
                var price = await _binanceRepository.GetPrice(instrument);
                _instrumentPrices.AddOrUpdate(instrument, price, (key, oldValue) => price);
            }
        }
    }


}
