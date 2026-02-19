using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pBiblioteca.Services;

public class ReservationExpirationHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    // roda a cada 30 minutos (pode ajustar)
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

    public ReservationExpirationHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                var today = DateOnly.FromDateTime(DateTime.Now);
                reservationService.ExpireOverdueReservations(today);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
