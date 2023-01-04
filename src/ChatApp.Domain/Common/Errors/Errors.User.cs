using ErrorOr;

namespace ChatApp.Domain.Common.Errors;

public partial class Errors
{
    public class User
    {
        public static Error DuplicateUsername => Error.Conflict(
            "User.DuplicateUsername",
            "User with the same username already exists in this room");

        public static Error UserNotFound => Error.Conflict(
                "User.UserNotFound",
                "User with this userId not found in this room");

        public static Error UserNotRemoved => Error.Unexpected(
            "User.UserNotRemoved",
            "User not remove because of database error");
    }

    public class Message 
    {
        public static Error MessageIsNotRemoved => Error.Unexpected(
            "Message.MessageIsNotRemoved",
            "Message is not removed because of database error");
        
        public static Error MessagesIsNotRemoved => Error.Unexpected(
            "Message.MessagesIsNotRemoved",
            "All messages from this chat is not removed because of database error");
    }

    public class Room
    {
        public static Error RoomNotFound => Error.Conflict(
            "Room.RoomNotFound",
            "Room not found");
    }
}