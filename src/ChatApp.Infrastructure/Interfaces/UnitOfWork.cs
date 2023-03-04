using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Interfaces;

namespace ChatApp.Infrastructure.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(
        IMessageRepository messageRepository, 
        IUserRepository userRepository)
    {
        Messages = messageRepository;
        Users = userRepository;
    }

    public IMessageRepository Messages { get; }

    public IUserRepository Users { get; }
}