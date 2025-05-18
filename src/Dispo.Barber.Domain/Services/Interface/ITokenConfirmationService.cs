namespace Dispo.Barber.Domain.Services.Interface
{
    public interface ITokenConfirmationService
    {
        Task GenerateCodeConfirmation(string phone);
        Task<bool> ValidateCodeConfirmation(string phone, string sms);
    }
}