using ErrorOr;
using FluentValidation.Results;

namespace Roomify.Application.Common.Errors;

public static class ErrorConverter
{
    public static List<Error> ConvertValidationErrors(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }
}