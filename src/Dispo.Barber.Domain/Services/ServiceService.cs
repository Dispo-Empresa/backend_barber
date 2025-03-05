using AutoMapper;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;

namespace Dispo.Barber.Domain.Services
{
    public class ServiceService(IMapper mapper, IServiceRepository repository, ICompanyRepository companyRepository, IBusinessUnityRepository businessUnityRepository, IUserRepository userRepository) : IServiceService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO)
        {
            var service = mapper.Map<Service>(createServiceDTO);
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
                var users = await businessUnityRepository.GetUsersAsync(cancellationToken, businessUnity.Id);

                foreach (var user in users)
                {
                    user.ServicesUser.Add(new ServiceUser
                    {
                        UserId = user.Id,
                        ServiceId = service.Id
                    });

                    userRepository.Update(user);
                    await userRepository.SaveChangesAsync(cancellationToken);
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
            var services = await repository.GetAllAsync(cancellationToken);
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
            service.Status = status;
            repository.Update(service);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
