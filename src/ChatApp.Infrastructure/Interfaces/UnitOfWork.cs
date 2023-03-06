using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Interfaces.Persistence;
using ChatApp.Application.Common.Interfaces;
using System.Data;

namespace ChatApp.Infrastructure.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private  IUserRepository _users;
    private  IMessageRepository _messages;
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    public IUserRepository Users => _users ??= new UserRepository(
        _connection, 
        _transaction);
    public IMessageRepository Messages => _messages;

    public void BeginTransaction()
    {
        _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
        _transaction?.Commit();
        _transaction = null;
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction = null;       
    }

    public void Dispose()
    {
        if(!_disposed)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}