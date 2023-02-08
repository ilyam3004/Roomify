using ChatApp.Application.Models.Requests;
using FluentValidation;

namespace ChatApp.Application.Common.Validations;

public class SaveMessageRequestValidator : AbstractValidator<SaveMessageRequest>
{
    public SaveMessageRequestValidator()
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