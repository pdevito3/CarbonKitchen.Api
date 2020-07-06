namespace CarbonKitchen.Api.Services.Auth
{
    using CarbonKitchen.Api.Data.Auth;

    public interface IAuthService
    {
        SecurityToken Authenticate(string key);
    }
}
