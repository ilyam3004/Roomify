using ErrorOr;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Services;
using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
         Console.WriteLine($"Connection {Context.ConnectionId} was opened");
         ErrorOr<UserResponse> result = await _userService.AddUserToRoom(
             new CreateUserRequest(
                 request.Username, 
                 Context.ConnectionId, 
                 request.RoomName));

         if (!result.IsError)
         {
             var user = result.Value;
             
             await Groups.AddToGroupAsync(Context.ConnectionId, user.RoomId);
             
             await Clients.Client(Context.ConnectionId)
                 .SendAsync("ReceiveUserData", user);
             
             await SendUserListToRoom(user);

             await SendMessageToRoom(new SendMessageRequest(
                 user.UserId,
                 user.RoomId,
                 $"User { user.Username } has joined the room",
                 DateTime.UtcNow,
                 false));
         }
         else
         {
             await Clients.Client(Context.ConnectionId)
                 .SendAsync("ReceiveError", Problem(result.Errors));
         }
     }
     
     public override async Task OnDisconnectedAsync(Exception? exception)
     {
         await base.OnDisconnectedAsync(exception);
     }
     
     public async Task SendMessageToRoom(SendMessageRequest request)
     {
         ErrorOr<MessageResponse> result = await _messageService.SaveMessage(
             new SaveMessageRequest(
                 request.UserId,
                 request.RoomId,
                 request.Text,
                 request.Date,
                 request.FromUser));

         if (!result.IsError)
         {
             await Clients.Group(result.Value.RoomId)
                 .SendAsync("ReceiveMessage", result.Value);
         }
         else
         {
             await Clients.Client(Context.ConnectionId)
                 .SendAsync("ReceiveMessage", Problem(result.Errors));
         }
     }

     public async Task SendUserListToRoom(UserResponse response)
     {
         ErrorOr<List<UserResponse>> result = await _userService.GetUserList();

         if (!result.IsError)
         {
             await Clients.Groups(roomId).SendAsync("UserList", result.Value);
         }
     }

     private ProblemDetails Problem(List<Error> errors)
     {
         if (errors.All(error => error.Type == ErrorType.Validation))
         {
             return GetValidationProblem(errors);
         }

         if (errors.Count is 0)
         {
             return new ProblemDetails();
         }

         return Problem(errors[0]);
     }

     private ProblemDetails Problem(Error error)
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
             Type = type
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