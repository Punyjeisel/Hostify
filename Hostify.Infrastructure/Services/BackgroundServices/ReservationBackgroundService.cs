using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hostify.Application.Features.Reservations.Interfaces;
using Hostify.Infrastructure.Repository;
using Hostify.Domain.Enums;



namespace Hostify.Infrastructure.Services.BackgroundServices
{
    public class ReservationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public ReservationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var reservationRepository =
                        scope.ServiceProvider.GetRequiredService<IReservationRepository>();

                    var reservations = await reservationRepository.GetAllAsync();

                    var now = DateTime.UtcNow;

                    foreach (var r in reservations)
                    {
                        if (r.Status == ReservationStatus.Confirmed &&
                            r.CheckOut <= now)
                        {
                            r.Status = ReservationStatus.Completed;
                            await reservationRepository.UpdateAsync(r);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
