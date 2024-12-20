using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Domain.Exception;

namespace Dispo.Barber.Application.Service
{
    public class ServiceService(IMapper mapper, IServiceRepository repository) : IServiceService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO)
        {
            var service = mapper.Map<Domain.Entities.Service>(createServiceDTO);
            await repository.AddAsync(cancellationToken, service);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<IList<ServiceInformationDTO>> GetServicesList(CancellationToken cancellationToken, long companyId)
        {
            var services = await repository.GetServicesByCompanyAsync(companyId, cancellationToken);
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
