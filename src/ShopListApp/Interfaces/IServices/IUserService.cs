using ShopListApp.Commands.Auth;
using ShopListApp.Commands.Delete;
using ShopListApp.Commands.Update;
using ShopListApp.Responses;

namespace ShopListApp.Interfaces.IServices;

public interface IUserService
{
    Task CreateUser(RegisterUserCommand cmd);
    Task DeleteUser(string id, DeleteUserCommand cmd);
    Task UpdateUser(string id, UpdateUserCommand updatedUser);
    Task<UserResponse> GetUserById(string id);
}
