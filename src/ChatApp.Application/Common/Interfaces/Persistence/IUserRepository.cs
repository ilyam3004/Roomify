using ChatApp.Domain.Entities;

namespace ChatApp.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserById(string userId);
    Task<Room> CreateRoomIfNotExists(string roomName);
    Task<List<User>> GetRoomUsers(string roomId);
    Task<bool> UserExists(string username, string roomName);
    Task<bool> UserExists(string userId);
    Task<bool> RoomExists(string roomId);
    Task<bool> RemoveUserFromRoom(string userId, string roomId);
}