using ShopListApp.Commands.Auth;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces.Identity;
using ShopListApp.Interfaces.ILogger;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Models;
using ShopListApp.Responses;

namespace ShopListApp.Application.Services;

public class AuthService(IUserManager userManager,
    IDbLogger<UserDto> logger,
    ITokenManager tokenManager,
    ITokenRepository tokenRepository) : IAuthService
{
    private IUserManager _userManager = userManager;
    private IDbLogger<UserDto> _logger = logger;
    private ITokenManager _tokenManager = tokenManager;
    private ITokenRepository _tokenRepository = tokenRepository;

    public async Task<LoginRegisterResponse> RegisterUser(RegisterUserCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        var user = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            UserName = cmd.UserName,
            Email = cmd.Email,
        };
        try
        {
            var userByEmail = await _userManager.FindByEmailAsync(cmd.Email);
            var userByName = await _userManager.FindByNameAsync(cmd.UserName);
            if (userByEmail != null)
                throw new UserWithEmailAlreadyExistsException();
            if (userByName != null)
                throw new UserWithUserNameAlreadyExistsException();
            await _userManager.CreateAsync(user, cmd.Password);
            var identityToken = _tokenManager.GenerateAccessToken(user);
            var refreshToken = _tokenManager.GenerateRefreshToken();
            var hashRefreshToken = _tokenManager.GetHashRefreshToken(refreshToken)
                ?? throw new DatabaseErrorException();
            await CreateRefreshTokenInDb(hashRefreshToken, user);
            await _logger.Log(Operation.Register, user);
            return new LoginRegisterResponse { IdentityToken = identityToken, RefreshToken = refreshToken };
        }
        catch (UserAlreadyExistsException)
        {
            throw;
        }
        catch
        {
            throw new DatabaseErrorException();
        }
    }

    private async Task CreateRefreshTokenInDb(string refreshToken, UserDto user)
    {
        var token = new Token
        {
            UserId = user.Id,
            RefreshTokenHash = refreshToken,
            ExpirationDate = DateTime.Now.AddDays(_tokenManager.GetRefreshTokenExpirationDays())
        };
        var result = await _tokenRepository.AddToken(token);
        if (!result)
            throw new DatabaseErrorException();
    }

    public async Task<LoginRegisterResponse> LoginUser(LoginUserCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        try
        {
            var user = await _userManager.FindByEmailAsync(cmd.UserIdentifier);
            if (user == null)
                user = await _userManager.FindByNameAsync(cmd.UserIdentifier);
            if (user == null)
                throw new UnauthorizedAccessException();
            var result = await _userManager.CheckPasswordAsync(user, cmd.Password);
            if (!result)
                throw new UnauthorizedAccessException();
            var identityToken = _tokenManager.GenerateAccessToken(user);
            var refreshToken = _tokenManager.GenerateRefreshToken();
            var hashRefreshToken = _tokenManager.GetHashRefreshToken(refreshToken) 
                ?? throw new UnauthorizedAccessException();
            await CreateRefreshTokenInDb(hashRefreshToken, user);
            await _logger.Log(Operation.Login, user);
            return new LoginRegisterResponse { IdentityToken = identityToken, RefreshToken = refreshToken };
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            throw new DatabaseErrorException();
        }
    }

    public async Task<string> RefreshAccessToken(RefreshTokenCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        try
        {
            var hash = _tokenManager.GetHashRefreshToken(cmd.RefreshToken) 
                                                    ?? throw new UnauthorizedAccessException();
            var token = await _tokenRepository.GetToken(hash)
                                                    ?? throw new UnauthorizedAccessException();
            var user = await _userManager.FindByIdAsync(token.UserId)
                                                    ?? throw new UnauthorizedAccessException();
            var identityToken = _tokenManager.GenerateAccessToken(user);
            return identityToken;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            throw new DatabaseErrorException();
        }
    }
}
