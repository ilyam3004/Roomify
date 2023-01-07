using ErrorOr;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Services;
using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

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
         
         if (!result.IsError)
         {
             await Clients.Group(result.Value.RoomId)
                 .SendAsync("ReceiveMessage", result);
         }
         else
         {
             await Clients.Client(Context.ConnectionId)
                 .SendAsync("ReceiveMessage", Problem(result.Errors));
         }
     }
     
     public override async Task OnDisconnectedAsync(Exception? exception)
     {
         Console.WriteLine($"Connection {Context.ConnectionId} was closed");
         await base.OnDisconnectedAsync(exception);
     }
     
     public async Task SendMessageToRoom(MessageRequest messageRequest)
     {
         ErrorOr<MessageResponse> result = await _messageService.SaveMessage(
             new SaveMessageRequest(
                 messageRequest.UserId,
                 messageRequest.RoomId,
                 messageRequest.Text,
                 messageRequest.Date,
                 messageRequest.FromUser));

         if (!result.IsError)
         {
             await Clients.Group(result.Value.RoomId)
                 .SendAsync("ReceiveMessage", result);
         }
         else
         {
             await Clients.Client(Context.ConnectionId)
                 .SendAsync("ReceiveMessage", Problem(result.Errors));
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
             ErrorType.Validation => StatusCodes.Status400BadRequest,
             ErrorType.NotFound => StatusCodes.Status404NotFound,
             _ => StatusCodes.Status500InternalServerError
         };

         var type = error.Type switch
         {
             ErrorType.Conflict => "",
             ErrorType.Failure => "",
             _ => ""
         };

         return new ProblemDetails
         {
             Status = statusCode,
             Detail = error.Description,
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

         return new ValidationProblemDetails(modelStateDictionary);
     }
}