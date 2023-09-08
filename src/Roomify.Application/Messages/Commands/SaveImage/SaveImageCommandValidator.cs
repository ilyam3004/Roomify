using FluentValidation;

namespace Roomify.Application.Messages.Commands.SaveImage;

public class SaveImageCommandValidator : 
    AbstractValidator<SaveImageCommand>
{
    public SaveImageCommandValidator()
    {
        RuleFor(m => m.UserId)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.RoomId)
            .NotNull();

        RuleFor(m => m.ImageUrl)
            .NotNull()
            .NotEmpty();
    }
}