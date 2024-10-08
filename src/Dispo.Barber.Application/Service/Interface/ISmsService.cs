using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface ISmsService
    {
        Task<string> SendMessageAsync(string phoneNumber, string messageBody, string messageType);
    }
}
