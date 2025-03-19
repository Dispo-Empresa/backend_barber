namespace Dispo.Barber.Domain.Utils.Extensions
{
    public static class IdExtensions
    {
        public static bool IsValid(this long id)
        {
            return id > 0;
        }

        public static bool IsInvalid(this long id)
        {
            return !id.IsValid();
        }
    }
}
