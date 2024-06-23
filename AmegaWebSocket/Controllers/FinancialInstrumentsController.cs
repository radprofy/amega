using AmegaWebSocket.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AmegaWebSocket.Controllers
{
    [ApiController]
    [Route("financial-instruments")]
    public class FinancialInstrumentsController : ControllerBase
    {
        private readonly IFinancialInstrumentsService _financialInstrumentsService;

        public FinancialInstrumentsController(
            IFinancialInstrumentsService financialInstrumentsService)
        {
            _financialInstrumentsService = financialInstrumentsService;
        }

        [HttpGet()]
        public async Task<IEnumerable<string>> Get()
        {
            return _financialInstrumentsService.GetAvailableInstruments();
        }

        [HttpGet("{instrument}/current-price")]
        public async Task<string> Get(string instrument)
        {
            return await _financialInstrumentsService.GetInstrumentPrice(instrument);
        }
    }
}
