namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface IBlacklistService
    {
        bool PutInBlacklist(object token);
        bool IsBlacklisted(object token);
    }
}
