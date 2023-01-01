using ErrorOr;

namespace ChatApp.Domain.Common.Errors;

public partial class Errors
{
    public class User
    {
        public static Error DuplicateUsername => Error.Conflict(
            "User.DuplicateUsername",
            "User with the same username already exists in this room");
    }
}