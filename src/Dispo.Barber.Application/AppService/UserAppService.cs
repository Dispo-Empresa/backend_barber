using System.Data;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Extension;

namespace Dispo.Barber.Application.AppService
{
    public class UserAppService(IUnitOfWork unitOfWork) : IUserAppService
    {
        public async Task AddServiceToUserAsync(long id, AddServiceToUserDTO addServiceToUserDTO)
        {
            var cancellationTokenSource = new CancellationTokenRegistration();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetAsync(id);
                if (user is null)
                {
                    throw new VersionNotFoundException();
                }

                user.ServicesUser.AddRange(addServiceToUserDTO.Services.Select(s => new ServiceUser
                {
                    UserId = user.Id,
                    ServiceId = s
                }).ToList());

                userRepository.Update(user);

                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            var cancellationTokenSource = new CancellationTokenRegistration();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                return await userRepository.GetAppointmentsAsync(cancellationTokenSource.Token, id);
            });
        }

    }
}
