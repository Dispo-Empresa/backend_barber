using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace Dispo.Barber.Infrastructure.Repositories
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public long GetLoggedUserId()
        {
            var httpContext = httpContextAccessor.HttpContext 
                ?? throw new InvalidOperationException("Não foi possível obter o contexto da requisição HTTP.");
            
            var userIdValue = httpContext.User?.FindFirst("id")?.Value;

            if (long.TryParse(userIdValue, out var userId))
                return userId;

            throw new BusinessException("Não foi possível identificar o usuário logado a partir do token.");
        }
    }
}
