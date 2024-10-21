using System.Data;
using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;
using Dispo.Barber.Domain.Extension;

namespace Dispo.Barber.Application.AppService
{
    public class UserAppService(IUnitOfWork unitOfWork, IMapper mapper) : IUserAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = mapper.Map<User>(createUserDTO);
                user.Schedules.AddRange(BuildNormalDays());
                if (user.IsPending())
                {
                    // TODO: Completar.
                }

                await userRepository.AddAsync(cancellationToken, user);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, AddServiceToUserDTO addServiceToUserDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetAsync(cancellationToken, id);
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
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                return await userRepository.GetAppointmentsAsync(cancellationToken, id);
            });
        }

        public async Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                return await userRepository.GetSchedulesAsync(cancellationToken, id);
            });
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetAsync(cancellationToken, id);
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
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetAsync(cancellationToken, id);
                if (user is null)
                {
                    throw new NotFoundException("Usuário não encontrado.");
                }

                user.Status = changeStatusDTO.Status;

                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetAsync(cancellationToken, id);
                if (user is null)
                {
                    throw new NotFoundException("Usuário não encontrado.");
                }

                user.Password = PasswordEncryptor.HashPassword(changePasswordDTO.Password);

                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationToken);
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