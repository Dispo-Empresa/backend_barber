namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface ITokenConfirmationAppService
    {
        Task GenerateCodeConfirmation(string phone);
        Task<bool> ValidateCodeConfirmation(string phone, string sms);
    }
}