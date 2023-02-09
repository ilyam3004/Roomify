using ChatApp.Application.Models.Requests;
using FluentValidation;

namespace ChatApp.Application.Common.Validations;

public class SaveImageRequestValidator : AbstractValidator<SaveImageRequest>
{
    public SaveImageRequestValidator()
    {
        RuleFor(m => m.UserId)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.Username)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.RoomId)
            .NotNull();

        RuleFor(m => m.ImageUrl)
            .NotNull()
            .NotEmpty();
    }
}