using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService
{
    public class BusinessUnityAppService(IUnitOfWork unitOfWork) : IBusinessUnityAppService
    {
        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<IBusinessUnityRepository>();
                return await companyRepository.GetUsersAsync(cancellationToken, id);
            });
        }

        public async Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<IBusinessUnityRepository>();
                return await companyRepository.GetPendingUsersAsync(cancellationToken, id);
            });
        }
    }
}
