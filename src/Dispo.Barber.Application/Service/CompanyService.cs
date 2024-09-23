using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service
{
    public class CompanyService : ServiceBase<Company>, ICompanyService
    {
        public CompanyService(IUnitOfWork unitOfWork)
            : base(unitOfWork.GetRepository<ICompanyRepository>())
        {
        }
    }
}
