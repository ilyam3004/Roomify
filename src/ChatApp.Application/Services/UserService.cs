using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> GetUserById(string userId)
    {
        var user = await _userRepository.GetUserById(userId);
        return new UserResponse(
            user.UserId, 
            user.Username, 
            user.ConnectionId, 
            user.RoomId);
    }

    public async Task<UserResponse> AddUser(string username, string connectionId, string roomId)
    {
        var dbUser = await _userRepository.AddUser(new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = username,
            ConnectionId = connectionId,
            RoomId = roomId
        });

        return new UserResponse(
            dbUser.UserId,
            dbUser.Username,
            dbUser.ConnectionId,
            dbUser.RoomId);
    }
}