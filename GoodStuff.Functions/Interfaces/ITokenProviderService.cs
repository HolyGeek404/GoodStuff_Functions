namespace GoodStuff.Functions.Interfaces;

public interface ITokenProviderService
{
    Task<string> GetAccessToken(string scope);
}