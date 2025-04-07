﻿using ShopListApp.Models;

namespace ShopListApp.Interfaces.IRepositories;

public interface ITokenRepository
{
    Task<bool> AddToken(Token token);
    Task<Token?> GetToken(string refreshToken);
}
