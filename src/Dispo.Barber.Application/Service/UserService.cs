using System.ComponentModel.Design;
using System.Data;
using System.Threading;
using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;
using Dispo.Barber.Domain.Extension;

namespace Dispo.Barber.Application.Service
{
    public class UserService(IMapper mapper, IUserRepository repository) : IUserService
    {
        public async Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, List<long> services)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                throw new VersionNotFoundException();
            }

            user.ServicesUser.AddRange(services.Select(s => new ServiceUser
            {
                UserId = user.Id,
                ServiceId = s
            }).ToList());

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            user.Password = PasswordEncryptor.HashPassword(changePasswordDTO.Password);

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            user.Status = changeStatusDTO.Status;

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO)
        {
            var user = mapper.Map<User>(createUserDTO);
            user.Password = "";
            user.Schedules.AddRange(BuildNormalDays());
            if (user.IsPending())
            {
                // TODO: Completar.
            }

            user.Slug = user.Name.ToLowerInvariant().Replace(" ", "-");

            await repository.AddAsync(cancellationToken, user);
            await repository.SaveChangesAsync(cancellationToken);

            if (createUserDTO.Services.Any())
                await AddServiceToUserAsync(cancellationToken, user.Id, createUserDTO.Services);
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await repository.GetAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO);
        }

        public async Task<long> GetUserIdByPhone(CancellationToken cancellationToken, string phone)
        {
            return await repository.GetIdByPhone(cancellationToken, phone);
        }

        public async Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetValidDaysSchedulesAsync(cancellationToken, id);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO)
        {
            var user = await repository.GetAsync(cancellationToken, id);
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

            if (!string.IsNullOrEmpty(updateUserDTO.Password))
            {
                user.Password = PasswordEncryptor.HashPassword(updateUserDTO.Password);
            }

            if (!string.IsNullOrEmpty(updateUserDTO.DeviceToken))
            {
                user.DeviceToken = updateUserDTO.DeviceToken;
            }

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug)
        {
            return await repository.GetByCompanyAndUserSlugAsync(cancellationToken, companySlug, userSlug);
        }

        public async Task<List<UserDTO>> GetByCompanyId(CancellationToken cancellationToken, long companyId)
        {
            return await repository.GetByCompanyId(cancellationToken, companyId);
        }

        public async Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            var user = await repository.GetByIdAsync(cancellationToken, id) ?? throw new NotFoundException("Usuário não encontrado.");
            if (!user.Services.Any())
            {
                return user;
            }

            var services = user.Services;
            var servicesGroup = user.Services.GroupBy(g => g.Id);
            var totalRealized = user.Services.Count();
            user.Services = new List<ServiceDetailDTO>();
            foreach (var service in servicesGroup)
            {
                user.Services.Add(new ServiceDetailDTO
                {
                    Description = service.First().Description,
                    Id = service.First().Id,
                    Realized = service.Count(),
                    RealizedPercentage = decimal.Round(service.Count() * 100m / totalRealized, 1),
                });
            }
            return user;
        }

        public async Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetServicesAsync(cancellationToken, id);
        }

        public async Task UploadImageAsync(CancellationToken cancellationToken, long id, byte[]? photo)
        {
            var user = await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Usuário não encontrado.");
            user.Photo = photo;
            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> StopProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId)
        {
            return await repository.StopProvidingServiceAsync(cancellationToken, id, serviceId);
        }

        public async Task<bool> StartProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId)
        {
            return await repository.StartProvidingServiceAsync(cancellationToken, id, serviceId);
        }

        public async Task ChangeDeviceToken(CancellationToken cancellationToken, long id, string deviceToken)
        {
            var user = await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Usuário não encontrado.");
            user.DeviceToken = deviceToken;
            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<AppointmentDetailDTO>> GetAppointmentsAsyncV2(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await repository.GetAppointmentsAsyncV2(cancellationToken, id, getUserAppointmentsDTO);
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
