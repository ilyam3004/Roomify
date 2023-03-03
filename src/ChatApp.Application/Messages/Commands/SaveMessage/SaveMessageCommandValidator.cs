using FluentValidation;

namespace ChatApp.Application.Messages.Commands.SaveMessage;

public class SaveMessageCommandValidator : AbstractValidator<SaveMessageCommand>
{
    public SaveMessageCommandValidator()
    {
        RuleFor(m => m.Text)
            .NotNull()
            .Length(1, 150);
        
        RuleFor(m => m.UserId)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.FromUser)
            .NotNull();

        RuleFor(m => m.RoomId)
            .NotNull()
            .NotEmpty();
    }
}