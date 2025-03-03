using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Infrastructure.Context;

namespace Dispo.Barber.Infrastructure.Repositories
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
