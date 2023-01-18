using FluentValidation;
using ChatApp.Application.Models.Requests;

namespace ChatApp.Application.Common.Validations;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(u => u.Username)
            .NotNull()
            .Length(3, 30)
            .WithMessage("The length of username should be in range between 3 and 30 characters");

        RuleFor(u => u.RoomName)
            .NotNull()
            .Length(4, 30)
            .WithMessage("The length of RoomName should be in range between 5 and 30 characters");

        RuleFor(u => u.ConnectionId)
            .NotNull()
            .NotEmpty()
            .WithMessage("ConnectionId should be not empty");
    }
}