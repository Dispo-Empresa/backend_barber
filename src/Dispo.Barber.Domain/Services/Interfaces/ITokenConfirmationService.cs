namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface ITokenConfirmationService
    {
        Task GenerateCodeConfirmation(string phone);
        Task<bool> ValidateCodeConfirmation(string phone, string sms);
    }
}