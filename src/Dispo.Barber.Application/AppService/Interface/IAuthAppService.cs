namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IAuthAppService
    {
        Task<string> Authenticate(CancellationToken cancellationToken, string phone, string password);
    }
}
