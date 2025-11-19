namespace GoodStuff.Functions.Models;

public record ApiRoute
{
    public string BaseUrl { get; init; }
    public string Scope { get; init; }
}