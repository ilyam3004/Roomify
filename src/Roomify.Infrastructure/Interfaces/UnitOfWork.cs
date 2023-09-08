using System.Data;
using ChatApp.Infrastructure.Interfaces.Persistence;
using Microsoft.Extensions.Options;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Common.Interfaces.Persistence;
using Roomify.Infrastructure.Config;
using Roomify.Infrastructure.Interfaces.Persistence;

namespace Roomify.Infrastructure.Interfaces;

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