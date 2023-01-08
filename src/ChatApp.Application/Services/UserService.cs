using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using ErrorOr;
using FluentValidation.Results;

namespace ChatApp.Application.Services;

public class UserService : IUserService
{
    private readonly IValidator<CreateUserRequest> _userValidator;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IValidator<CreateUserRequest> userValidator)
    {
        _userValidator = userValidator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserResponse>> AddUserToRoom(CreateUserRequest request)
    {
        var validateResult = await _userValidator.ValidateAsync(request);
        
        if (validateResult.IsValid)
        {   
            var room = await _userRepository.CreateRoomIfNotExists(request.RoomName);
        
            if (await _userRepository.UserExists(request.Username, room.RoomId))
            {
                return Errors.User.DuplicateUsername;
            }
            
            var dbUser = await _userRepository.AddUser(new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                RoomId = room.RoomId,
                ConnectionId = request.ConnectionId
            });

            return MapUserResponse(dbUser);
        }
        
        return ConvertValidationErrorToError(validateResult.Errors);
    }

    public async Task<ErrorOr<string>> RemoveUserFromRoom(string userId)
    {
        if (!await _userRepository.UserExists(userId))
        {
            return Errors.User.UserNotFound;
        }

        bool userRemoved = await _userRepository.RemoveUserFromRoom(userId);

        return userRemoved ? "User removed successfully" : Errors.User.UserNotRemoved;
    }
    
    private List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }
    
    

    private UserResponse MapUserResponse(User user)
    {
        return new UserResponse(
            user.UserId,
            user.Username,
            user.ConnectionId,
            user.RoomId);
    }
}