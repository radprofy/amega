using AmegaWebSocket.Domain;

namespace AmegaWebSocket.Jobs
{
    public class PriceUpdater : BackgroundService
    {
        private readonly IFinancialInstrumentsService _financialInstrumentsService;

        public PriceUpdater(IFinancialInstrumentsService financialInstrumentsService)
        {
            _financialInstrumentsService = financialInstrumentsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _financialInstrumentsService.UpdatePrices();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
