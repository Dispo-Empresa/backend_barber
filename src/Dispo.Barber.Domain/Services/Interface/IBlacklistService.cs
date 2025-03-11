namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IBlacklistService
    {
        bool PutInBlacklist(object token);
        bool IsBlacklisted(object token);
    }
}
