
using AmegaWebSocket.Controllers;
using AmegaWebSocket.Domain;
using AmegaWebSocket.Integrations;
using AmegaWebSocket.Jobs;

namespace AmegaWebSocket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSignalR();
            builder.Services.AddSwaggerGen();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            builder.Services.AddOptions();
            builder.Services.Configure<Config>(config.GetSection("Config"));
            builder.Services.AddSingleton<IFinancialInstrumentsService, FinancialInstrumentsService>();
            builder.Services.AddSingleton<IBinanceRepository, BinanceRepository>();
            builder.Services.AddHostedService<PriceUpdater>();
            builder.Services.AddHostedService<PriceBroadcaster>();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseWebSockets();
            app.MapHub<FinancialInstrumentsHub>("/ws");
            app.Run();
        }
    }
}
