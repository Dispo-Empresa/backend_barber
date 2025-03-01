namespace Dispo.Barber.Application.Repository
{
    public interface IServiceRepository : IRepositoryBase<Domain.Entities.Service>
    {
        Task<List<Domain.Entities.Service>> GetListServiceAsync(List<long> serviceIds);
        Task<IList<Domain.Entities.Service>> GetServicesByCompanyAsync(long id, bool? activated, CancellationToken cancellationToken);
    }
}
