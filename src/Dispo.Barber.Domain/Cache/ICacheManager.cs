namespace Dispo.Barber.Domain.Cache
{
    public interface ICacheManager
    {
        void Add(string key, string obj, long expiration = 300);

        string Get(string key);

        void Remove(string key);
    }
}
