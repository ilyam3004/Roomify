using Microsoft.AspNetCore.Mvc.ModelBinding;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Models.Requests;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Contracts.Rooms;
using ErrorOr;

namespace ChatApp.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;

    public ChatHub(IUserService userService, IMessageService messageService)
    {
        _userService = userService;
        _messageService = messageService;
    }

    public async Task JoinRoom(JoinUserRequest request)
    {
        ErrorOr<UserResponse> result = await _userService.AddUserToRoom(
            new CreateUserRequest(
                request.Username,
                Context.ConnectionId,
                request.RoomName));

        await result.Match(
            async onValue => await SendDataToRoomAboutAddingUser(onValue),
            async onError => await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveError", GenerateProblem(result.Errors)));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ErrorOr<UserResponse> result = await _userService
            .RemoveUserFromRoom(Context.ConnectionId);

        await result.Match(
            async onValue => await SendDataToRoomAboutUserLeaving(onValue),
            async onError => await SendRemovingErrorToClientIfErrorTypeIsNotFound(onError[0]));
    }

    private async Task SendDataToRoomAboutAddingUser(UserResponse response)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, response.RoomId);
        await SendUserData(response);
        await SendUserList(response.RoomId);
        await SendMessageToRoom(new SendMessageRequest(
            response.UserId,
            response.Username,
            response.RoomId,
            $"User {response.Username} has joined the room",
            false));
        await SendAllRoomMessages(response.RoomId);
    }

    private async Task SendDataToRoomAboutUserLeaving(UserResponse response)
    {
        await SendUserList(response.RoomId);
        await SendMessageToRoom(new SendMessageRequest(
            response.UserId,
            response.Username,
            response.RoomId,
            $"User {response.Username} has left the room",
            false));
    }
    
    public async Task SendUserMessage(string message)
    {
        ErrorOr<UserResponse> result = await _userService
            .GetUserByConnectionId(Context.ConnectionId);

        await result.Match(
            async onValue => await SendMessageToRoom(new SendMessageRequest(
                onValue.UserId,
                onValue.Username,
                onValue.RoomId,
                message,
                true)),
            async onError => await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveError", GenerateProblem(onError)));
    }

    public async Task SendAllRoomMessages(string roomId)
    {
        List<MessageResponse> result = await _messageService.GetAllRoomMessages(roomId);

        await Clients.Client(Context.ConnectionId)
            .SendAsync("ReceiveRoomMessages", result);
    }

    public async Task SendImageToRoom(SendImageRequest request)
    {
        ErrorOr<MessageResponse> result = await _messageService.SaveImage(
            new SaveImageRequest(
                request.UserId,
                request.RoomId,
                request.ImageUrl));

        await Clients.Group(result.Value.RoomId)
            .SendAsync("ReceiveMessage", result.Value);
        
        // await result.Match(
        //     async onValue => await Clients.Group(onValue.RoomId)
        //         .SendAsync("ReceiveMessage", onValue),
        //     async onError => await Clients.Client(Context.ConnectionId)
        //         .SendAsync("ReceiveError", GenerateProblem(onError)));
    }

    private async Task SendMessageToRoom(SendMessageRequest request)
    {
        ErrorOr<MessageResponse> result = await _messageService.SaveMessage(
            new SaveMessageRequest(
                request.UserId,
                request.Username,
                request.RoomId,
                request.Text,
                request.FromUser));

        await result.Match(
            async onValue => await Clients.Group(onValue.RoomId)
                .SendAsync("ReceiveMessage", onValue),
            async onError => await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveError", GenerateProblem(onError)));
    }

    private async Task SendRemovingErrorToClientIfErrorTypeIsNotFound(Error error)
    {
        if (error.Type != ErrorType.Unexpected)
        {
            await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveError", error);
        }
    }

    private async Task SendUserList(string roomId)
    {
        List<UserResponse> result = await _userService.GetUserList(roomId);

        await Clients.Group(roomId)
            .SendAsync("ReceiveUserList", result);
    }

    private async Task SendUserData(UserResponse response)
    {
        await Clients.Client(Context.ConnectionId)
            .SendAsync("ReceiveUserData", response);
    }

    private ProblemDetails GenerateProblem(List<Error> errors)
    {
        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return GetValidationProblem(errors);
        }

        if (errors.Count is 0)
        {
            return new ProblemDetails();
        }

        return GenerateProblem(errors[0]);
    }

    private ProblemDetails GenerateProblem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var type = error.Type switch
        {
            ErrorType.Conflict => "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.8",
            ErrorType.NotFound => "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.4",
            _ => "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1"
        };

        return new ProblemDetails
        {
            Status = statusCode,
            Title = error.Description,
            Type = type,
        };
    }

    private ValidationProblemDetails GetValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(
                error.Code,
                error.Description);
        }

        return new ValidationProblemDetails(modelStateDictionary)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Title = "One or more validation errors occured"
        };
    }
}