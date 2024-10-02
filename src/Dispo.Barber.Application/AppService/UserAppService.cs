using System.Data;
using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Extension;

namespace Dispo.Barber.Application.AppService
{
    public class UserAppService(IUnitOfWork unitOfWork, IMapper mapper) : IUserAppService
    {
        public async Task CreateAsync(CreateUserDTO createUserDTO)
        {
            var cancellationTokenSource = new CancellationTokenRegistration();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = mapper.Map<User>(createUserDTO);
                user.Password = PasswordEncryptor.HashPassword(user.Password);
                user.Schedules.AddRange(BuildNormalDays());
                await userRepository.AddAsync(user);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }

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

        public async Task<List<UserSchedule>> GetUserSchedulesAsync(long id)
        {
            var cancellationTokenSource = new CancellationTokenRegistration();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                return await userRepository.GetSchedulesAsync(cancellationTokenSource.Token, id);
            });
        }

        public async Task DisableUserAsync(long id)
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

                user.Active = false;

                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }

        public async Task UpdateAsync(long id, UpdateUserDTO updateUserDTO)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetAsync(id);
                if (user is null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(updateUserDTO.Name))
                {
                    user.Name = updateUserDTO.Name;
                }

                if (!string.IsNullOrEmpty(updateUserDTO.Surname))
                {
                    user.Surname = updateUserDTO.Surname;
                }

                if (!string.IsNullOrEmpty(updateUserDTO.Phone))
                {
                    user.Phone = updateUserDTO.Phone;
                }

                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }

        private List<UserSchedule> BuildNormalDays() => [
            new(DayOfWeek.Monday, "08:00", "18:00", false, false),
                new(DayOfWeek.Monday, "12:00", "13:30", true, false),
                new(DayOfWeek.Tuesday, "08:00", "18:00", false, false),
                new(DayOfWeek.Tuesday, "12:00", "13:30", true, false),
                new(DayOfWeek.Wednesday, "08:00", "18:00", false, false),
                new(DayOfWeek.Wednesday, "12:00", "13:30", true, false),
                new(DayOfWeek.Thursday, "08:00", "18:00", false, false),
                new(DayOfWeek.Thursday, "12:00", "13:30", true, false),
                new(DayOfWeek.Friday, "08:00", "18:00", false, false),
                new(DayOfWeek.Friday, "12:00", "13:30", true, false),
                new(DayOfWeek.Saturday, "08:00", "18:00", false, false),
                new(DayOfWeek.Saturday, "12:00", "13:30", true, false),
                new(DayOfWeek.Sunday, "08:00", "18:00", false, false),
                new(DayOfWeek.Sunday, "12:00", "13:30", true, false),
            ];
    }
}
