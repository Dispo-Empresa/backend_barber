namespace Dispo.Barber.Domain.Extension
{
    public static class ListExtension
    {
        public static void AddRange<T>(this List<T> source, List<T> values)
        {
            foreach (var value in values)
            {
                source.Add(value);
            }
        }

        public static void AddRange<T>(this ICollection<T> source, List<T> values)
        {
            if (source is null)
            {
                source = new List<T>();
            }

            foreach (var value in values)
            {
                source.Add(value);
            }
        }
    }
}
