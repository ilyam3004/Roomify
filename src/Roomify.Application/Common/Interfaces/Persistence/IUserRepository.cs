using Roomify.Domain.Entities;

namespace Roomify.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserById(string userId);
    Task<Room> CreateRoomIfNotExists(string roomName);
    Task<Room> GetRoomById(string roomId);
    Task<List<User>> GetRoomUsers(string roomId);
    Task<bool> UserExists(string username, string roomName);
    Task<bool> UserExists(string userId);
    Task<bool> RemoveRoomDataIfEmpty(string roomId, string userId);
    Task<User?> GetUserByConnectionIdOrNull(string connectionId);
}