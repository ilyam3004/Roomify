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
            Room? room = await _userRepository.CreateRoomIfNotExists(request.RoomName);
            if (room is null)
            {
                return Errors.Room.RoomNotCreated;
            }

            if (await _userRepository.UserExists(request.Username, room.RoomId))
            {
                return Errors.User.DuplicateUsername;
            }

            var dbUser = await _userRepository.AddUser(new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                RoomId = room.RoomId,
                ConnectionId = request.ConnectionId,
                HasLeft = false
            });

            return MapUserResponse(dbUser, room);
        }

        return ConvertValidationErrorToError(validateResult.Errors);
    }

    public async Task<ErrorOr<Deleted>> RemoveUserFromRoom(string connectionId)
    {
        User? user = await _userRepository.GetUserByConnectionIdOrNull(connectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room? room = await _userRepository.GetRoomById(user.RoomId);
        if (room is null)
        {
            return Errors.Room.RoomNotFound;
        }

        await _userRepository.RemoveRoomDataIfEmpty(user.RoomId, user.UserId);

        return Result.Deleted;
    }

    public async Task<ErrorOr<List<UserResponse>>> GetUserList(string roomId)
    {
        List<User> dbUsers = await _userRepository.GetRoomUsers(roomId);

        Room? room = await _userRepository.GetRoomById(roomId);

        if (room is null)
        {
            return Errors.Room.RoomNotFound;
        }

        return MapUserList(dbUsers, new Room
        {
            RoomId = room.RoomId,
            RoomName = room.RoomName
        });
    }

    public async Task<ErrorOr<UserResponse>> GetUserByConnectionId(string connectionId)
    {
        User? user = await _userRepository.GetUserByConnectionIdOrNull(connectionId);

        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room? room = await _userRepository.GetRoomById(user.RoomId);

        if (room is null)
        {
            return Errors.Room.RoomNotFound;
        }

        return MapUserResponse(user, room);
    }


    private static List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }

    private List<UserResponse> MapUserList(List<User> dbUsers, Room room)
    {
        List<UserResponse> userList = new();

        dbUsers.ForEach(user => { userList.Add(MapUserResponse(user, room)); });

        return userList;
    }


    private UserResponse MapUserResponse(User user, Room room)
    {
        return new UserResponse(
            user.UserId,
            user.Username,
            user.ConnectionId,
            room.RoomId,
            room.RoomName);
    }
}