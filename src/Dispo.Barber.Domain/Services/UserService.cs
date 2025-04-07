using System.Data;
using AutoMapper;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.BusinessUnity;
using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Extension;
using Dispo.Barber.Domain.Integration;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Domain.Services
{
    public class UserService(IMapper mapper,
                             IUserRepository repository,
                             ICompanyService companyService,
                             IBusinessUnityService businessUnityService,
                             IServiceRepository serviceRepository,
                             IBusinessUnityRepository businessUnityRepository,
                             IBlacklistService blacklistService,
                             IHubIntegration hubIntegration) : IUserService
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
                throw new NotFoundException("Usuário não encontrado.");

            PasswordValidator.Validate(changePasswordDTO.Password);
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

            if (user.Status == UserStatus.Inactive)
            {
                blacklistService.PutInBlacklist(id);
            }
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

            //user.Slug = user.Name.ToLowerInvariant().Replace(" ", "-");

            await repository.AddAsync(cancellationToken, user);
            await repository.SaveChangesAsync(cancellationToken);

            if (createUserDTO.Services.Any())
                await AddServiceToUserAsync(cancellationToken, user.Id, createUserDTO.Services);
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await repository.GetAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO);
        }

        public async Task<UserDTO?> GetUserInfoByPhone(CancellationToken cancellationToken, string phone, UserStatus status = UserStatus.Active)
        {
            return await repository.GetUserInfoByPhone(cancellationToken, phone, status);
        }

        public async Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetValidDaysSchedulesAsync(cancellationToken, id);
        }

        public async Task<List<UserSchedule>> GetUserBreaksAsync(CancellationToken cancellationToken, long id, DayOfWeek dayOfWeek)
        {
            return await repository.GetBreaksAsync(cancellationToken, id, dayOfWeek);
        }

        public async Task<List<UserSchedule>> GetUserDaysOffAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetDaysOffAsync(cancellationToken, id);
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
                PasswordValidator.Validate(updateUserDTO.Password);
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

        public async Task<List<ServiceInformationDTO>> GetEnabledServicesAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetEnabledServicesAsync(cancellationToken, id);
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

        public async Task CreateOwnerUserAsync(CancellationToken cancellationToken, CreateOwnerUserDTO createOwnerUserDto)
        {
            var user = mapper.Map<User>(createOwnerUserDto);

            if (!string.IsNullOrEmpty(createOwnerUserDto.Password))
            {
                PasswordValidator.Validate(createOwnerUserDto.Password);
                user.Password = PasswordEncryptor.HashPassword(createOwnerUserDto.Password);
            }

            user.Schedules.AddRange(BuildNormalDays());
            user.Slug = user.Name.ToLowerInvariant().Replace(" ", "-");
            user.Status = UserStatus.Active;
            user.Role = UserRole.Manager;

            await repository.AddAsync(cancellationToken, user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateEmployeeUserAsync(CancellationToken cancellationToken, CreateEmployeeUserDTO createEmployeeUser)
        {
            var user = mapper.Map<User>(createEmployeeUser);

            user.Name = "";
            user.BusinessUnityId = createEmployeeUser.BusinessUnityId;
            user.Role = createEmployeeUser.Role;
            user.Status = UserStatus.Pending;
            user.Slug = "";
            user.DeviceToken = "";

            await repository.AddAsync(cancellationToken, user);
            await repository.SaveChangesAsync(cancellationToken);

            var companyId = await businessUnityRepository.GetCompanyIdAsync(cancellationToken, createEmployeeUser.BusinessUnityId);
            var servicesList = await serviceRepository.GetServicesByCompanyAsync(companyId, null, cancellationToken);

            user.ServicesUser.AddRange(servicesList.Select(s => new ServiceUser
            {
                UserId = user.Id,
                ServiceId = s.Id
            }).ToList());

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);

            if (servicesList != null && servicesList.Count != 0)
            {
                foreach (var serviceId in servicesList)
                    await StopProvidingServiceAsync(cancellationToken, user.Id, serviceId.Id);
            }
        }

        public async Task FinalizeEmployeeUserRegistrationAsync(CancellationToken cancellationToken, long id, FinalizeEmployeeUserDTO finalizeEmployeeUserDto)
        {
            var user = await repository.GetFirstAsync(cancellationToken, id) ?? throw new NotFoundException("Usuário não encontrado.");

            if (await ExistsByPhone(cancellationToken, user.Phone))
            {
                throw new AlreadyExistsException("Este número já está cadastrado e ativado em uma barbearia.");
            }

            if (!string.IsNullOrEmpty(finalizeEmployeeUserDto.Password))
            {
                PasswordValidator.Validate(finalizeEmployeeUserDto.Password);
                user.Password = PasswordEncryptor.HashPassword(finalizeEmployeeUserDto.Password);
            }

            user.CreatedAt = LocalTime.Now;
            user.Schedules.AddRange(BuildNormalDays());
            user.Name = finalizeEmployeeUserDto.Name;
            user.Photo = finalizeEmployeeUserDto.Photo;
            user.DeviceToken = finalizeEmployeeUserDto.DeviceToken;
            user.Status = UserStatus.Active;
            user.Slug = user.Name.ToLowerInvariant().Replace(" ", "-");

            if (finalizeEmployeeUserDto.EnabledServicesId != null && finalizeEmployeeUserDto.EnabledServicesId.Any())
            {
                foreach (var serviceId in finalizeEmployeeUserDto.EnabledServicesId)
                    await StartProvidingServiceAsync(cancellationToken, id, serviceId);
            }

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateBarbershopSchemeAsync(CancellationToken cancellationToken, CreateBarbershopSchemeDto createBarbershopSchemeDto)
        {
            var createdCompanyId = await companyService.CreateAsync(cancellationToken, createBarbershopSchemeDto.Company);
            var createdBusinessUnityId = await businessUnityService.CreateAsync(cancellationToken, new CreateBusinessUnityDTO
            {
                CompanyId = createdCompanyId,
                Country = "",
                City = "",
                District = "",
                CEP = "",
                Street = ""
            });

            var user = mapper.Map<User>(createBarbershopSchemeDto.OwnerUser);

            if (await ExistsByPhone(cancellationToken, user.Phone))
            {
                throw new AlreadyExistsException("Este número já está cadastrado e ativado em uma barbearia.");
            }

            if (!string.IsNullOrEmpty(createBarbershopSchemeDto.OwnerUser.Password))
            {
                PasswordValidator.Validate(createBarbershopSchemeDto.OwnerUser.Password);
                user.Password = PasswordEncryptor.HashPassword(createBarbershopSchemeDto.OwnerUser.Password);
            }

            user.CreatedAt = LocalTime.Now;
            user.Schedules.AddRange(BuildNormalDays());
            user.Slug = user.Name.ToLowerInvariant().Replace(" ", "-");
            user.Status = UserStatus.Active;
            user.Role = UserRole.Manager;
            user.BusinessUnityId = createdBusinessUnityId;

            if (createBarbershopSchemeDto.Company.Services != null && createBarbershopSchemeDto.Company.Services.Count != 0)
            {
                var servicesList = await serviceRepository.GetServicesByCompanyAsync(createdCompanyId, null, cancellationToken);
                user.ServicesUser.AddRange(servicesList.Select(s => new ServiceUser
                {
                    UserId = user.Id,
                    ServiceId = s.Id
                }).ToList());
            }

            await repository.AddAsync(cancellationToken, user);
            await repository.SaveChangesAsync(cancellationToken);

            await companyService.UpdateOwner(cancellationToken, createdCompanyId, user.Id);

            await hubIntegration.CreateHubLicense(new LicenseRequestDTO
            {
                CompanyId = createdCompanyId,
                PlanType = createBarbershopSchemeDto.PlanType,
                Expiration = createBarbershopSchemeDto.PlanType == PlanType.BarberPremiumTrial ? LocalTime.Now.AddDays(7) : null
            }, cancellationToken);
        }

        private async Task<bool> ExistsByPhone(CancellationToken cancellationToken, string phone)
        {
            return await repository.ExistsAsync(cancellationToken, w => w.Phone == phone && w.Status == UserStatus.Active);
        }

        public async Task UpdateAllFromCompany(CancellationToken cancellationToken, long companyId, UserStatus status)
        {
            var users = await repository.GetAsync(cancellationToken, w => w.BusinessUnity != null && w.BusinessUnity.CompanyId == companyId && w.Status != status, "BusinessUnity.Company");
            foreach (var user in users)
            {
                if (user.BusinessUnity?.Company.OwnerId == user.Id)
                {
                    continue;
                }

                user.Status = status;
            }

            repository.UpdateRange(users);
            await repository.SaveChangesAsync(cancellationToken);
        }

        private List<UserSchedule> BuildNormalDays() => [
            new(DayOfWeek.Monday, "08:00", "18:00", false, false, true),
            new(DayOfWeek.Monday, "12:00", "13:30", true, false, true),

            new(DayOfWeek.Tuesday, "08:00", "18:00", false, false, true),
            new(DayOfWeek.Tuesday, "12:00", "13:30", true, false, true),

            new(DayOfWeek.Wednesday, "08:00", "18:00", false, false, true),
            new(DayOfWeek.Wednesday, "12:00", "13:30", true, false, true),

            new(DayOfWeek.Thursday, "08:00", "18:00", false, false, true),
            new(DayOfWeek.Thursday, "12:00", "13:30", true, false, true),

            new(DayOfWeek.Friday, "08:00", "18:00", false, false, true),
            new(DayOfWeek.Friday, "12:00", "13:30", true, false, true),

            new(DayOfWeek.Saturday, "08:00", "18:00", false, false, false),
            new(DayOfWeek.Saturday, "12:00", "13:30", true, false, true),

            new(DayOfWeek.Sunday, "08:00", "18:00", false, false, false),
            new(DayOfWeek.Sunday, "12:00", "13:30", true, false, true),
        ];
    }
}
