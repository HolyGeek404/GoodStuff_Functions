using GoodStuff.Functions.Models;

namespace GoodStuff.Functions.Interfaces;

public interface IValidatorService
{
    ApiRoute ValidateApi(string api);
}