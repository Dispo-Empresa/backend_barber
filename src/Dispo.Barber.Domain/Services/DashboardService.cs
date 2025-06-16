using Dispo.Barber.Domain.Models;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;

namespace Dispo.Barber.Domain.Services
{
    public class DashboardService(IUserRepository repository) : IDashboardService
    {
        public async Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId)
        {
            var dashboard = new Dashboard();
            var user = await repository.GetWithAppointmentsAsync(cancellationToken, userId);
            if (user is null)
            {
                return dashboard;
            }

            dashboard.Itens = new List<DashboardItem>
            {
                new DashboardItem { Name = "Agendamentos", DisplayInformation = user.TodayAppointments() },
                new DashboardItem { Name = "Receita", DisplayInformation = user.EstimatedGains() },
                new DashboardItem { Name = "Horas Agendadas", DisplayInformation = user.ScheduledHours() },
                new DashboardItem { Name = "Aproveitamento", DisplayInformation = user.ChairUsage() }, // % de Aproveitamento de Cadeira
            };
            return dashboard;
        }
    }
}
