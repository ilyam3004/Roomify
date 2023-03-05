using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Interfaces;
using System.Data.Common;
using System.Data;
using ChatApp.Infrastructure.Interfaces.Persistence;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private  IUserRepository _users;
    private  IMessageRepository _messages;
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;
    private bool _disposed;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    public IUserRepository Users => _users ??= new UserRepository(_connection, _transaction);
    public IMessageRepository Messages => _messages ??= new MessageRepository(,_connection, _transaction);
}