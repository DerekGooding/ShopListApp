﻿using ShopListApp.Commands;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Responses;

namespace ShopListApp.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<LoginRegisterResponse> RegisterUser(RegisterUserCommand cmd);
        Task<LoginRegisterResponse> LoginUser(LoginUserCommand cmd);
        Task<string> RefreshAccessToken(RefreshTokenCommand cmd);
    }
}
