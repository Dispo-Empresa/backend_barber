namespace Dispo.Barber.Domain.Utils
{
    public class LocalTime
    {
        public static DateTime Now
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            }
        }
    }
}
