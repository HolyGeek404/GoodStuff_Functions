namespace GoodStuff.Functions.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmail(string userEmail, Guid key);
}