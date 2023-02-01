using ErrorOr;
using System.Net.NetworkInformation;

namespace ChatApp.Domain.Common.Errors;

public partial class Errors
{
    public class User
    {
        public static Error DuplicateUsername => Error.Conflict(
            "User.DuplicateUsername",
            "User with the same username already exists in this room");

        public static Error UserNotFound => Error.NotFound(
                "User.UserNotFound",
                "User with this userId not found in this room");

        public static Error UserNotRemoved => Error.Failure(
            "User.UserNotRemoved",
            "User not remove because of database error");
    }

    public class Message 
    {
        public static Error MessageIsNotRemoved => Error.Failure(
            "Message.MessageIsNotRemoved",
            "Message is not removed because you didn't create this message");
        
        public static Error MessagesIsNotRemoved => Error.Failure(
            "Message.MessagesIsNotRemoved",
            "All messages from this chat is not removed because of database error");

        public static Error MessageNotFound => Error.NotFound(
            "Message.NotFound",
            "Message not found");
    }

    public class Room
    {
        public static Error RoomNotFound => Error.NotFound(
            "Room.RoomNotFound",
            "Room not found");

        public static Error RoomIsEmpty => Error.Conflict(
            "Room.RoomIsEmpty",
            "Room is empty");
    }
}