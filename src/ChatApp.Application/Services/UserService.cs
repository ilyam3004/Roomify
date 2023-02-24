using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using FluentValidation.Results;
using ChatApp.Domain.Entities;
using FluentValidation;
using MapsterMapper;
using ErrorOr;

namespace ChatApp.Application.Services;

public class UserService : IUserService
{
    private readonly IValidator<CreateUserRequest> _userValidator;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, 
        IValidator<CreateUserRequest> userValidator,
        IMapper mapper)
    {
        _mapper = mapper;
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
                Avatar = request.Avatar,
                HasLeft = false
            };

            var dbUser = await _userRepository
                .AddUser(userToAdd);
    
            return _mapper.Map<UserResponse>((dbUser, room));
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

        return _mapper.Map<UserResponse>((user, room));
    }

    public async Task<List<UserResponse>> GetUserList(string roomId)
    {
        Room room = await _userRepository.GetRoomById(roomId);

        List<User> dbUsers = await _userRepository.GetRoomUsers(roomId);

        return dbUsers.Select(user => _mapper.Map<UserResponse>((user, room))).ToList();
    }

    public async Task<ErrorOr<UserResponse>> GetUserByConnectionId(string connectionId)
    {
        User? user = await _userRepository.GetUserByConnectionIdOrNull(connectionId);

        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room room = await _userRepository.GetRoomById(user.RoomId);

        return _mapper.Map<UserResponse>((user, room));
    }


    private static List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }
}