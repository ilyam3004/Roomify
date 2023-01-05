using ErrorOr;
using Newtonsoft.Json;
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
     public ChatHub(IUserService userService)
     {
         _userService = userService;
     }
     
     public async Task JoinRoom(JoinUserRequest request)
     {
         ErrorOr<UserResponse> result = await _userService.AddUserToRoom(
             new CreateUserRequest(
                 request.Username, 
                 request.ConnectionId, 
                 request.RoomName));

         Clients.Group("testroom").SendAsync("ReceiveMessage", result);
     }
     
     public override async Task<Task> OnDisconnectedAsync(Exception? exception)
     {
         return base.OnDisconnectedAsync(exception);
     }
     
     public async Task SendMessageToRoom(MessageRequest messageRequest)
     {

     }

     private ProblemDetails Problem(List<Error> errors)
     {
         if (errors.All(error => error.Type == ErrorType.Validation))
         {
             return ValidationProblem(errors);
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

         return new ProblemDetails
         {
             Status = statusCode,
             Detail = error.Description
         };
     }

     private ValidationProblemDetails ValidationProblem(List<Error> errors)
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