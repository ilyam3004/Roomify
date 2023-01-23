using FluentValidation;
using ChatApp.Application.Models.Requests;

namespace ChatApp.Application.Common.Validations;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(u => u.Username)
            .NotNull()
            .Length(3, 10)
            .WithMessage("Username should be in range between 3 and 10 characters")
            .Matches(@"^[A-Za-z\d]+$")
            .WithMessage("Username must not contain any special characters or spaces.")
            .Matches("[a-z]")
            .WithMessage("Username must contain one or more lowercase letters.");

        RuleFor(u => u.RoomName)
            .NotNull()
            .Length(3, 10)
            .WithMessage("RoomName should be in range between 3 and 30 characters")
            .Matches(@"^[A-Za-z\d]+$")
            .WithMessage("RoomName must not contain any special characters or spaces.")
            .Matches("[a-z]")
            .WithMessage("RoomName must contain one or more lowercase letters.");
        
        RuleFor(u => u.ConnectionId)
            .NotNull()
            .NotEmpty()
            .WithMessage("ConnectionId should be not empty");
    }
}