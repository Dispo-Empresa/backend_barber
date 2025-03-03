using AutoMapper;
using Dispo.Barber.Domain.DTOs.BusinessUnity;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;

namespace Dispo.Barber.Domain.Services
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
