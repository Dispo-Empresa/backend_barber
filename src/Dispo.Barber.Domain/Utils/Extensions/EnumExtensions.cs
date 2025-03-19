namespace Dispo.Barber.Domain.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }
    }
}
