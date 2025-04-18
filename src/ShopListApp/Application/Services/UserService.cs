﻿using ShopListApp.Commands.Auth;
using ShopListApp.Commands.Delete;
using ShopListApp.Commands.Update;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces.Identity;
using ShopListApp.Interfaces.ILogger;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Responses;

namespace ShopListApp.Application.Services;

public class UserService(IDbLogger<UserDto> logger, IUserManager userManager) : IUserService
{
    private IDbLogger<UserDto> _logger = logger;
    private IUserManager _userManager = userManager;

    public async Task CreateUser(RegisterUserCommand cmd)
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
            await _userManager.CreateAsync(user, cmd.Password);
            await _logger.Log(Operation.Create, user);
        }
        catch 
        {
            throw new DatabaseErrorException();
        }
    }

    public async Task DeleteUser(string id, DeleteUserCommand cmd)
    {
        _ = id ?? throw new ArgumentNullException(nameof(id));
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new UnauthorizedAccessException();
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, cmd.Password);
            if (!isPasswordCorrect)
                throw new UnauthorizedAccessException();
            var result = await _userManager.DeleteAsync(user);
            if (!result)
                throw new UnauthorizedAccessException();
            await _logger.Log(Operation.Delete, user);
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

    public async Task UpdateUser(string id, UpdateUserCommand cmd)
    {
        _ = id ?? throw new ArgumentNullException(nameof(id));
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        try
        {
            var user = await _userManager.FindByIdAsync(id) ?? throw new UnauthorizedAccessException();
            user.UserName = cmd.UserName ?? user.UserName;
            user.Email = cmd.Email ?? user.Email;
            var result = await _userManager.UpdateAsync(user);
            var passwordResult = await _userManager.ChangePasswordAsync(user, 
                cmd.CurrentPassword, 
                cmd.NewPassword ?? cmd.CurrentPassword);
            if (!result || !passwordResult)
                throw new UnauthorizedAccessException();
            var propertiesCount = typeof(UpdateUserCommand).GetProperties().Length;
            var nullCount = GetLengthOfNullProperties(cmd);
            if (nullCount < propertiesCount - 1)
                await _logger.Log(Operation.Update, user);
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

    private int GetLengthOfNullProperties(UpdateUserCommand cmd)
    {
        var propertiesCount = typeof(UpdateUserCommand).GetProperties().Length;
        var nullCount = typeof(UpdateUserCommand)
                            .GetProperties()
                            .Count(p => p.GetValue(cmd) == null);
        return nullCount;
    }

    public async Task<UserResponse> GetUserById(string id)
    {
        _ = id ?? throw new ArgumentNullException(nameof(id));
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new UnauthorizedAccessException();
            var view = new UserResponse
            {
                UserName = user.UserName!,
                Email = user.Email!
            };
            return view;
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
