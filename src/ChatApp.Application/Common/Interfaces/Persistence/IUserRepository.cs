using ChatApp.Domain.Entities;

namespace ChatApp.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserById(string userId);
}