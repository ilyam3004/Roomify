using Roomify.Application.Common.Interfaces.Persistence;

namespace Roomify.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IMessageRepository Messages { get; }
    IUserRepository Users { get; }
}