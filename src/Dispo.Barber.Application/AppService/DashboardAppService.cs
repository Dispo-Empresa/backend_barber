using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Model;

namespace Dispo.Barber.Application.AppService
{
    public class DashboardAppService(IUnitOfWork unitOfWork) : IDashboardAppService
    {
        public async Task<Dashboard> BuildDashboardForUser(long userId)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var dashboard = new Dashboard();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetWithAppointmentsAsync(cancellationTokenSource.Token, userId);
                if (user is null)
                {
                    return;
                }

                dashboard.Itens = new List<DashboardItem>
                {
                    new DashboardItem { Name = "Agendamentos Hoje", DisplayInformation = user.TodayAppointments() },
                    new DashboardItem { Name = "Ganhos Estimados", DisplayInformation = user.EstimatedGains() },
                    new DashboardItem { Name = "Horas Agendadas", DisplayInformation = user.ScheduledHours() },
                };
            });
            return dashboard;
        }

    }
}
