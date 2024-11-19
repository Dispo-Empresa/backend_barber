using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Model;

namespace Dispo.Barber.Application.Service
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
                new DashboardItem { Name = "Agendamentos Hoje", DisplayInformation = user.TodayAppointments() },
                new DashboardItem { Name = "Ganhos Estimados", DisplayInformation = user.EstimatedGains() },
                new DashboardItem { Name = "Horas Agendadas", DisplayInformation = user.ScheduledHours() },
                new DashboardItem { Name = "Aproveitamento", DisplayInformation = user.ChairUsage() }, // % de Aproveitamento de Cadeira
            };
            return dashboard;
        }
    }
}
