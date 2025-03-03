namespace Dispo.Barber.Domain.Repositories
{
    public interface IServiceRepository : IRepositoryBase<Entities.Service>
    {
        Task<List<Entities.Service>> GetListServiceAsync(List<long> serviceIds);
        Task<IList<Entities.Service>> GetServicesByCompanyAsync(long id, bool? activated, CancellationToken cancellationToken);
    }
}
