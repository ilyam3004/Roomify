using ChatApp.Domain.Entities;

namespace ChatApp.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserById(string userId);
    Task<Room> CreateRoomIfNotExists(string roomName);
    Task<List<User>> GetRoomUsers(string roomId);
    Task<bool> UserExists(string username);
    Task<bool> RoomExists(string roomName);
    Task<Room> GetRoomByRoomName(string roomName);
}