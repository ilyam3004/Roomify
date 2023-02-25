using ErrorOr;

namespace ChatApp.Domain.Common.Errors;

public abstract class Errors
{
    public abstract class User
    {
        public static Error DuplicateUsername => Error.Conflict(
            "User.DuplicateUsername",
            "User with the same username already exists in this room");

        public static Error UserNotFound => Error.NotFound(
                "User.UserNotFound",
                "User not found");
    }

    public abstract class Message 
    {
        public static Error MessageIsNotRemoved => Error.Failure(
            "Message.MessageIsNotRemoved",
            "Message is not removed because you didn't create this message");

        public static Error MessageNotFound => Error.NotFound(
            "Message.NotFound",
            "Message not found");

        public static Error ImageFileIsCorrupted => Error.Conflict(
            "Message.ImageFileIsCorrupted",
            "Image file is corrupted");

        public static Error CantUploadImage => Error.Failure(
            "Message.CantUploadImage",
            "Cant upload image because of server error");
    }

    public abstract class Room
    {
        public static Error RoomIsEmpty => Error.Conflict(
            "Room.RoomIsEmpty",
            "Room is empty");
    }
}