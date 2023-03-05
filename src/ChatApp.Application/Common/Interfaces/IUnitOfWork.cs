using ChatApp.Application.Common.Interfaces.Persistence;

namespace ChatApp.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMessageRepository Messages { get; }
    IUserRepository Users { get; } 
    void BeginTransaction();
    void Commit();
    void Rollback();
}