using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.BusinessUnity;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service
{
    public class BusinessUnityService(IMapper mapper, IBusinessUnityRepository repository) : IBusinessUnityService
    {
        public async Task<long> CreateAsync(CancellationToken cancellationToken, CreateBusinessUnityDTO createBusinessUnityDTO)
        {
            var businessUnity = mapper.Map<BusinessUnity>(createBusinessUnityDTO);
            await repository.AddAsync(cancellationToken, businessUnity);
            await repository.SaveChangesAsync(cancellationToken);
            return businessUnity.Id;
        }

        public async Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetUsersAsync(cancellationToken, id);
        }

        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetUsersAsync(cancellationToken, id);
        }
    }
}
