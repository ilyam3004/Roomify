using ChatApp.Domain.Entities;

namespace ChatApp.Application.Models.Responses;

public record RoomResponse(
    string RoomId,
    string roomName,
    List<User> Users);