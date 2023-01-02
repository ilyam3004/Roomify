using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;

public interface IUserService
{
    Task<UserResponse> GetUserById(string userId);
    Task<ErrorOr<UserResponse>> AddUserToRoom(CreateUserRequest request);
}