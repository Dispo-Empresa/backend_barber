using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Service;

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

        public async Task<IList<ServiceListDTO>> GetServicesList(CancellationToken cancellationToken, long companyId)
        {
            var services = await repository.GetServicesByCompanyAsync(companyId, cancellationToken);
            return mapper.Map<IList<ServiceListDTO>>(services);
        }

        public async Task<IList<ServiceListDTO>> GetAllServicesList(CancellationToken cancellationToken)
        {
            var services = await repository.GetAllAsync(cancellationToken);
            return mapper.Map<IList<ServiceListDTO>>(services);
        }
    }
}
