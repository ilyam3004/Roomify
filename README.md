# Chat Application following Clean Architecture and CQRS
---
Bla bla bla
### Technologies
---
- [ASP.NET 7](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/introduction-to-signalr)
- [MediatR](https://github.com/jbogard/MediatR)
- [Dapper](https://github.com/DapperLib/Dapper) 
- [Mapster](https://github.com/MapsterMapper/Mapster)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [ErrorOr](https://www.nuget.org/packages/ErrorOr/0.1.0)
- [xUnit](https://github.com/xunit/xunit) 
- [Moq](https://github.com/moq/moq4)
- [AutoFixture](https://github.com/AutoFixture/AutoFixture) 

### CQRS pattern
---
CQRS (Command Query Responsibility Segregation) is a software design pattern that separates the operations that modify state from those that read state in a system. This separation allows for different optimizations and scaling strategies for the two types of operations. In a CQRS architecture, commands represent actions that change the state of the system, while queries represent requests for information about the system's current state. By separating these concerns, a CQRS system can be optimized for both high write throughput and fast query performance. However, implementing a CQRS architecture can be complex and may require significant changes to existing systems.

![](http://res.cloudinary.com/drlrr8vpy/image/upload/v1678569111/ickfjfwiohubww6hzkvi.jpg "CQRS model")

### Support
---
If you are having problems, please let me know by [raising a new issue](https://github.com/ilyam3004/ChatAppServer/issues).
