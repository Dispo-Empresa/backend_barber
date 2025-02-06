using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class TokenRepository : RepositoryBase<Token>, ITokenRepository
    {
        private readonly ApplicationContext _context;
        public TokenRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }
    }
}
