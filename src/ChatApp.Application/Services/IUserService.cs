using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;

public interface IUserService
{
    Task<ErrorOr<UserResponse>> AddUserToRoom(CreateUserRequest request);
    Task<List<UserResponse>> GetUserList(string roomId);
    Task<ErrorOr<UserResponse>> GetUserByConnectionId(string connectionId);
    Task<ErrorOr<UserResponse>> RemoveUserFromRoom(string connectionId);
}