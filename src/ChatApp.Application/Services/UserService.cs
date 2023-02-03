using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Server.HttpSys;

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
            Room room = await _userRepository.CreateRoomIfNotExists(request.RoomName);

            if (await _userRepository.UserExists(request.Username, room.RoomId))
            {
                return Errors.User.DuplicateUsername;
            }

            var userToAdd = new User()
            {   
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                ConnectionId = request.ConnectionId,
                RoomId = room.RoomId,
                HasLeft = false
            };

            var dbUser = await _userRepository
                .AddUser(userToAdd);
    
            return MapUserResponse(dbUser, room);
        }

        return ConvertValidationErrorToError(validateResult.Errors);
    }

    public async Task<ErrorOr<UserResponse>> RemoveUserFromRoom(string connectionId)
    {
        User? user = await _userRepository.GetUserByConnectionIdOrNull(connectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room room = await _userRepository.GetRoomById(user.RoomId);
        
        bool isRoomEmpty = await _userRepository
            .RemoveRoomDataIfEmpty(user.RoomId, user.UserId);

        if (isRoomEmpty) 
        {
            return Errors.Room.RoomIsEmpty;
        }

        return MapUserResponse(user, room);
    }

    public async Task<List<UserResponse>> GetUserList(string roomId)
    {
        Room room = await _userRepository.GetRoomById(roomId);

        List<User> dbUsers = await _userRepository.GetRoomUsers(roomId);

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

        Room room = await _userRepository.GetRoomById(user.RoomId);

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