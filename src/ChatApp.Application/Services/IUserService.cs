using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;

public interface IUserService
{
    Task<ErrorOr<UserResponse>> AddUserToRoom(CreateUserRequest request);
    Task<ErrorOr<List<UserResponse>>> GetUserList(string roomId);
    Task<ErrorOr<UserResponse>> GetUserByConnectionId(string connectionId);
    Task<ErrorOr<Deleted>> RemoveUserFromRoom(string userId);
}