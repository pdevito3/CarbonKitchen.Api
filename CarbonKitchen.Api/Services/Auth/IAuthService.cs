namespace CarbonKitchen.Api.Services.Auth
{
    using CarbonKitchen.Api.Data.Auth;
    using CarbonKitchen.Api.Data.Entities;

    public interface IAuthService
    {
        SecurityToken Authenticate(User user);
    }
}
