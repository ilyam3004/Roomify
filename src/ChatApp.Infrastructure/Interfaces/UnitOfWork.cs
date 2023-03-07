using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Interfaces.Persistence;
using ChatApp.Application.Common.Interfaces;
using ChatApp.Infrastructure.Config;
using Microsoft.Extensions.Options;
using System.Data;

namespace ChatApp.Infrastructure.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private IUserRepository _users;
    private IMessageRepository _messages;

    public UnitOfWork(
        IDbConnection connection, 
        IOptions<CloudinarySettings> options)
    {
        _users = new UserRepository(connection);
        _messages = new MessageRepository(connection, options);
    }

    public IUserRepository Users => _users;
    public IMessageRepository Messages => _messages;
}