using System.Data;
using Dapper;
using Roomify.Application.Common.Interfaces.Persistence;
using Roomify.Domain.Entities;
using Roomify.Infrastructure.Queries;

namespace ChatApp.Infrastructure.Interfaces.Persistence;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public UserRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<User> AddUser(User user)
    {
        await _connection.ExecuteAsync(
            UserQueries.AddUser, 
            user);

        return user;
    }

    public async Task<User> GetUserById(string userId)
    {
        var user = await _connection.QueryFirstOrDefaultAsync<User>(
            UserQueries.GetUserById,
            new { userId }
        );

        return user;
    }

    public async Task<Room> CreateRoomIfNotExists(string roomName)
    {
        if (await RoomExistsByRoomName(roomName))
        {
            return await GetRoomByRoomName(roomName);
        }

        var room = new Room { RoomId = Guid.NewGuid().ToString(), RoomName = roomName };

        await _connection.ExecuteAsync(UserQueries.CreateRoom, room);

        return room;
    }

    public async Task<List<User>> GetRoomUsers(string roomId)
    {
        IEnumerable<User> users = await _connection.QueryAsync<User>(
            UserQueries.GetRoomUsers,
            new { RoomId = roomId }
        );

        return users.ToList();
    }

    public async Task<Room> GetRoomById(string roomId)
    {
        Room room = await _connection.QueryFirstOrDefaultAsync<Room>(
            UserQueries.GetRoomById,
            new { roomId }
        );

        return room;
    }

    public async Task<bool> UserExists(string username, string roomId)
    {
        int count = await _connection.QueryFirstOrDefaultAsync<int>(
            UserQueries.CountOfUsersByUsername,
            new { RoomId = roomId, Username = username }
        );

        return count != 0;
    }

    public async Task<User?> GetUserByConnectionIdOrNull(string connectionId)
    {
        User? user = await _connection.QueryFirstOrDefaultAsync<User>(
            UserQueries.GetUserByConnectionId,
            new { ConnectionId = connectionId }
        );

        return user;
    }

    public async Task<bool> UserExists(string userId)
    {
        int count = await _connection.QueryFirstOrDefaultAsync<int>(
            UserQueries.CountOfUsersByUserId,
            new { UserId = userId }
        );

        return count != 0;
    }


    public async Task<bool> RemoveRoomDataIfEmpty(string roomId, string userId)
    {
        int count = await _connection.QueryFirstOrDefaultAsync<int>(
            UserQueries.CountOfUsersInRoom,
            new { RoomId = roomId }
        );

        if (count == 1)
        {
            await RemoveAllMessagesFromRoom(roomId);
            await RemoveAllUsersFromRoom(roomId);
            await RemoveRoom(roomId);
            return true;
        }

        await UpdateUserStatusToHasLeft(userId);
        return false;
    }

    private async Task UpdateUserStatusToHasLeft(string userId)
    {
        await _connection.ExecuteAsync(UserQueries.UpdateUserStatus, new { UserId = userId });
    }
    
    private async Task RemoveAllUsersFromRoom(string roomId)
    {
        await _connection.ExecuteAsync(
            UserQueries.RemoveAllUsersFromRoom, 
            new { RoomId = roomId });
    }

    private async Task RemoveAllMessagesFromRoom(string roomId)
    {
        await _connection.ExecuteAsync(
            MessageQueries.RemoveAllMessagesFromRoom, 
            new { RoomId = roomId });
    }

    private async Task RemoveRoom(string roomId)
    {
        await _connection.ExecuteAsync(
            UserQueries.RemoveRoom, 
            new { RoomId = roomId });
    }

    private async Task<Room> GetRoomByRoomName(string roomName)
    {
        Room room = await _connection.QueryFirstOrDefaultAsync<Room>(
            UserQueries.GetRoomByRoomName,
            new { RoomName = roomName });

        return room;
    }

    private async Task<bool> RoomExistsByRoomName(string roomName)
    {
        int count = await _connection.QueryFirstOrDefaultAsync<int>(
            UserQueries.CountOfRooms,
            new { RoomName = roomName });

        return count != 0;
    }
}
