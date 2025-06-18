using AutoMapper;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Providers;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;
using Dispo.Barber.Domain.Utils;
using Microsoft.AspNetCore.Http;

namespace Dispo.Barber.Domain.Services
{
    public class ServiceService(IMapper mapper,
                                IHttpContextAccessor httpContextAccessor,
                                IServiceRepository repository, 
                                ICompanyRepository companyRepository, 
                                IBusinessUnityRepository businessUnityRepository, 
                                IUserRepository userRepository,
                                INotificationSenderProvider notificationService,
                                IServiceUserRepository serviceUserRepository) : IServiceService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO)
        {
            var service = mapper.Map<Service>(createServiceDTO);
            var loggedUserId = long.Parse(httpContextAccessor.HttpContext?.User.FindFirst("id").Value);
            await repository.AddAsync(cancellationToken, service);
            await repository.SaveChangesAsync(cancellationToken);

            var companyService = await companyRepository.GetWithServicesAsync(cancellationToken, createServiceDTO.CompanyId) ?? throw new NotFoundException("Barbearia não encontrado.");
            companyService.ServicesCompany.Add(new ServiceCompany()
            {
                CompanyId = createServiceDTO.CompanyId,
                ServiceId = service.Id
            });

            companyRepository.Update(companyService);
            await companyRepository.SaveChangesAsync(cancellationToken);

            var businessUnities = await companyRepository.GetBusinessUnitiesAsync(cancellationToken, createServiceDTO.CompanyId);

            foreach (var businessUnity in businessUnities)
            {
                var users = await businessUnityRepository.GetActiveUsersAsync(cancellationToken, businessUnity.Id);

                foreach (var user in users)
                {
                    user.ServicesUser.Add(new ServiceUser
                    {
                        UserId = user.Id,
                        ServiceId = service.Id,
                        ProvidesUntil = createServiceDTO.AutoEnableToUsers ? null : LocalTime.Now
                    });

                    userRepository.Update(user);
                    await userRepository.SaveChangesAsync(cancellationToken);

                    if (user.Id != loggedUserId)
                        await notificationService.NotifyAsync(cancellationToken, user.DeviceToken, "Novo serviço!", $"O serviço {createServiceDTO.Description} foi adicionado na barbearia!", NotificationType.NewService);
                }
            }
        }

        public async Task<IList<ServiceInformationDTO>> GetServicesList(CancellationToken cancellationToken, long companyId, bool? activated)
        {
            var services = await repository.GetServicesByCompanyAsync(companyId, activated, cancellationToken);
            return mapper.Map<IList<ServiceInformationDTO>>(services);
        }

        public async Task<IList<ServiceInformationDTO>> GetAllServicesList(CancellationToken cancellationToken)
        {
            var services = await repository.GetAllAsNoTrackingAsync(cancellationToken);
            return mapper.Map<IList<ServiceInformationDTO>>(services);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateServiceDTO updateServiceDTO)
        {
            var service = await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Serviço não encontrado.");

            if (service.Price != updateServiceDTO.Price)
            {
                service.Price = updateServiceDTO.Price;
            }

            if (service.Description != updateServiceDTO.Description)
            {
                service.Description = updateServiceDTO.Description;
            }

            if (service.Duration != updateServiceDTO.Duration)
            {
                service.Duration = updateServiceDTO.Duration;
            }

            repository.Update(service);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ServiceStatus status)
        {
            var service = await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Serviço não encontrado.");
            var loggedUserId = long.Parse(httpContextAccessor.HttpContext?.User.FindFirst("id").Value);
            var businessUnityId = await userRepository.GetBusinessUnityIdByIdAsync(cancellationToken, loggedUserId);
            var users = await businessUnityRepository.GetUsersAsync(cancellationToken, businessUnityId);

            // Regra discutível
            foreach (var user in users)
            {
                if (status == ServiceStatus.Inactive)
                {
                    var serviceUser = await serviceUserRepository.GetByUserIdAndServiceId(cancellationToken, user.Id, service.Id);

                    if (user.Role == UserRole.Manager)
                    {
                        //await userRepository.StopProvidingServiceAsync(cancellationToken, user.Id, service.Id);
                    }
                    else
                    {
                        serviceUserRepository.Delete(serviceUser);
                        await serviceUserRepository.SaveChangesAsync(cancellationToken);
                    }
                }
                else
                {
                    if (user.Role == UserRole.Manager)
                    {
                        //await userRepository.StartProvidingServiceAsync(cancellationToken, user.Id, service.Id);
                    }
                    else
                    {
                        user.ServicesUser.Add(new ServiceUser
                        {
                            UserId = user.Id,
                            ServiceId = service.Id,
                        });

                        userRepository.Update(user);
                        await userRepository.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            service.Status = status;
            repository.Update(service);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
