using AmegaWebSocket.Domain;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace AmegaWebSocket.Controllers
{
    public class FinancialInstrumentsHub : Hub
    {
        private readonly ILogger<FinancialInstrumentsController> _logger;
        private readonly IFinancialInstrumentsService _financialInstrumentsService;

        public FinancialInstrumentsHub(
            ILogger<FinancialInstrumentsController> logger,
            IFinancialInstrumentsService financialInstrumentsService)
        {
            _logger = logger;
            _financialInstrumentsService = financialInstrumentsService;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("New connection established.");
        }

        public async Task JoinBroadcast(string financialInstrument)
        {
            var availableInstruments = _financialInstrumentsService.GetAvailableInstruments();
            if (!availableInstruments.Contains(financialInstrument))
                await Clients.Caller.SendAsync("ReceiveMessage", $"{financialInstrument} not available");
            await Groups.AddToGroupAsync(Context.ConnectionId, financialInstrument);
            _logger.LogInformation($"User subscribed to {financialInstrument} stream.");
        }
    }
}
