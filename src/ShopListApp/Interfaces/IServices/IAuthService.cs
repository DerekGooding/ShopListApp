using ShopListApp.Commands.Auth;
using ShopListApp.Responses;

namespace ShopListApp.Interfaces.IServices;

public interface IAuthService
{
    Task<LoginRegisterResponse> RegisterUser(RegisterUserCommand cmd);
    Task<LoginRegisterResponse> LoginUser(LoginUserCommand cmd);
    Task<string> RefreshAccessToken(RefreshTokenCommand cmd);
}
