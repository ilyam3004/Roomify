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
    private readonly IValidator<CreateUserRequest> _validator;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IValidator<CreateUserRequest> validator)
    {
        _validator = validator;
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

    public async Task<ErrorOr<UserResponse>> AddUser(CreateUserRequest request)
    {
        //TODO CHANGE IN THE REPOSITORY IF USER EXISTS IN THE CURRENT ROOM NOT IN DATABASE
        if (await _userRepository.UserExists(request.Username))
        {
            return Errors.User.DuplicateUsername;
        }
        
        var room = await _userRepository.CreateRoomIfNotExists(request.RoomName);
        
        var validateResult = await _validator.ValidateAsync(request);
        if (validateResult.IsValid)
        {
            var dbUser = await _userRepository.AddUser(new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                RoomId = room.RoomId,
                ConnectionId = request.ConnectionId
            });

            return new UserResponse(
                dbUser.UserId,
                dbUser.Username,
                dbUser.ConnectionId,
                dbUser.RoomId);
        }
        
        return ConvertValidationErrorToError(validateResult.Errors);
    }

    private List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }
}