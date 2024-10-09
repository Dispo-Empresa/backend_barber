using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService
{
    public class BusinessUnityAppService(IUnitOfWork unitOfWork) : IBusinessUnityAppService
    {
        public async Task<List<User>> GetUsersAsync(long id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<IBusinessUnityRepository>();
                return await companyRepository.GetUsersAsync(id);
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
