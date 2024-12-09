namespace Dispo.Barber.Domain.Exception
{
    public class InternalServerError : System.Exception
    {
        public InternalServerError(string message) : base(message)
        {
        }
    }
}
