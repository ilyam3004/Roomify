using ChatApp.Application.Messages.Queries.GetRoomMessages;
using ChatApp.Application.Messages.Commands.SaveMessage;
using ChatApp.Application.Users.Queries.GetUserByConnId;
using ChatApp.Application.Messages.Commands.SaveImage;
using ChatApp.Application.Users.Queries.GetUserList;
using ChatApp.Application.Users.Commands.LeaveRoom;
using ChatApp.Application.Users.Commands.JoinRoom;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ChatApp.Application.Models.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Contracts.Rooms;
using MapsterMapper;
using ErrorOr;
using MediatR;

namespace ChatApp.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public ChatHub(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task JoinRoom(JoinRoomRequest request)
    {
        var command = _mapper.Map<JoinRoomCommand>((request, Context.ConnectionId));
            
        ErrorOr<UserResponse> result = await _mediator.Send(command);

        await result.Match(
            async onValue => await SendDataToRoomAboutAddingUser(onValue),
            async onError => await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveError", GenerateProblem(result.Errors)));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var comamnd = new LeaveRoomCommand(Context.ConnectionId);
        
        ErrorOr<UserResponse> result = await _mediator.Send(comamnd);

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
        var query = new GetUserByConnIdQuery(Context.ConnectionId);
        ErrorOr<UserResponse> result = await _mediator.Send(query);

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
        var query = new GetRoomMessagesQuery(roomId);
        List<MessageResponse> result = await _mediator.Send(query);

        await Clients.Client(Context.ConnectionId)
            .SendAsync("ReceiveRoomMessages", result);
    }

    public async Task SendImageToRoom(SendImageRequest request)
    {
        var command = _mapper.Map<SaveImageCommand>(request);
        ErrorOr<MessageResponse> result = await _mediator.Send(command);
        
        await result.Match(
            async onValue => await Clients.Group(onValue.RoomId)
                .SendAsync("ReceiveMessage", onValue),
            async onError => await Clients.Client(Context.ConnectionId)
                .SendAsync("ReceiveError", GenerateProblem(onError)));
    }

    private async Task SendMessageToRoom(SendMessageRequest request)
    {
        var command = _mapper.Map<SaveMessageCommand>(request);
        ErrorOr<MessageResponse> result = await _mediator.Send(command);

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
        var query = new GetUserListQuery(roomId);

        List<UserResponse> result = await _mediator.Send(query);

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