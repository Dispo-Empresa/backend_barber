using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Service;

namespace Dispo.Barber.Application.AppService
{
    public class ServiceAppService(IUnitOfWork unitOfWork, IMapper mapper) : IServiceAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var serviceRepository = unitOfWork.GetRepository<IServiceRepository>();
                var service = mapper.Map<Domain.Entities.Service>(createServiceDTO);
                await serviceRepository.AddAsync(cancellationToken, service);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task<IList<ServiceListDTO>> GetServicesList(long companyId, CancellationToken cancellationToken)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var serviceRepository = unitOfWork.GetRepository<IServiceRepository>();
                var services = await serviceRepository.GetServicesByCompanyAsync(companyId, cancellationToken);
                return mapper.Map<IList<ServiceListDTO>>(services);
            });
        }
    }
}
