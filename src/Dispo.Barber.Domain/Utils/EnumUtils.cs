namespace Dispo.Barber.Domain.Utils
{
    public static class EnumUtils
    {
        public static T ToEnum<T>(this string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }
    }
}
