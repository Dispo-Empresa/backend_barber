namespace Dispo.Barber.Domain.Exceptions
{
    public class InternalServerError : Exception
    {
        public InternalServerError(string message) : base(message)
        {
        }
    }
}
