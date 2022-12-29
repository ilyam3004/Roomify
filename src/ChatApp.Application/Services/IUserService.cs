using ChatApp.Application.Models;

namespace ChatApp.Application.Services;

public interface IUserService
{
    Task<UserResponse> GetUserById(string userId);
    Task<UserResponse> AddUser(string username, string room, string connectionId);
}