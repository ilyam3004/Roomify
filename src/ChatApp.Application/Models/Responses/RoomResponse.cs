using ChatApp.Domain.Entities;

namespace ChatApp.Application.Models;

public record RoomResponse(
    string RoomId,
    string roomName,
    List<User> Users);