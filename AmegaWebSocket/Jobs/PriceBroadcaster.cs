
using AmegaWebSocket.Controllers;
using AmegaWebSocket.Domain;
using Microsoft.AspNetCore.SignalR;

namespace AmegaWebSocket.Jobs
{
    public class PriceBroadcaster : BackgroundService
    {
        private readonly IHubContext<FinancialInstrumentsHub> _hubContext;
        private readonly IFinancialInstrumentsService _financialInstrumentsService;

        public PriceBroadcaster(
            IHubContext<FinancialInstrumentsHub> hubContext,
            IFinancialInstrumentsService financialInstrumentsService)
        {
            _hubContext = hubContext;
            _financialInstrumentsService = financialInstrumentsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var availableInstruments = _financialInstrumentsService.GetAvailableInstruments();
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var instrument in availableInstruments)
                {
                    var instrumentPrice = await _financialInstrumentsService.GetInstrumentPrice(instrument);
                    await _hubContext.Clients.Group(instrument).SendAsync("PriceBroadcast", instrument, instrumentPrice);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
