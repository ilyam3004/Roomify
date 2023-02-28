using FluentValidation.Results;
using ErrorOr;

namespace ChatApp.Application.Common.Errors;

public static class ErrorConverter
{
    public static List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }
}