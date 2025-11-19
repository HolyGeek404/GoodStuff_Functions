namespace GoodStuff.Functions.Services;

public interface ITokenProviderService
{
    Task<string> GetAccessToken(string scope);
}