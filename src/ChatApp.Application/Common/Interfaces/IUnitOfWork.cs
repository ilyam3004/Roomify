using ChatApp.Application.Common.Interfaces.Persistence;

namespace ChatApp.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IMessageRepository Messages { get; }
    IUserRepository Users { get; } 
}