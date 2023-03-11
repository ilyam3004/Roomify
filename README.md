# Chat Application following Clean Architecture and CQRS patterns

This is a chat application that follows the Clean Architecture and CQRS pattern. The application is written in C# using ASP.NET Core 7.0 and SignalR. You can use this application as a template for your own ASP.NET Core Web API's following the principles of Clean Architecture. It is a server part of the chat application. The client part is [here](https://github.com/ilyam3004/ChatApp).
## Technologies

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

## Overview
---

![](/docs/img/clean-architecture-model.jpg "Clean Architecture model")

The architecture of the system is divided into four layers: Domain, Application, Infrastructure, and Presentation.

### Domain
The Domain layer encompasses all entities, enums, exceptions, interfaces, types, and logic that are specific to the business domain of the system.

### Application
The Application layer contains all application logic. It depends on the Domain layer, but is independent of any other layer or project. This layer defines interfaces that other layers can implement. For instance, if the application requires access to a notification service, a new interface is added to the Application layer and an implementation is created within the Infrastructure layer.

### Infrastructure
The Infrastructure layer includes classes that allow the system to access external resources such as file systems, web services, SMTP, and so on. These classes should be based on interfaces defined within the Application layer.

### Presentation
Finally, the Presentation layer is an ASP.NET Web API which gives us an opportunity to build SPA applications, Mobile apps or Desktop clients and so far. This layer is responsible for all user interface logic and depends on the Application layer.


## Database configuration
---
This application is configured to use Azure SQL Server database in production. If you would like to use this application you need to prepare SQL Server or Azure SQL Server database by 

  "UseInMemoryDatabase": false,

Verify that the DefaultConnection connection string within appsettings.json points to a valid SQL Server instance.

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.
## CQRS pattern
---
CQRS (Command Query Responsibility Segregation) is a software design pattern that separates the operations that modify state from those that read state in a system. This separation allows for different optimizations and scaling strategies for the two types of operations. In a CQRS architecture, commands represent actions that change the state of the system, while queries represent requests for information about the system's current state. By separating these concerns, a CQRS system can be optimized for both high write throughput and fast query performance. However, implementing a CQRS architecture can be complex and may require significant changes to existing systems.

![](/docs/img/cqrs-model.jpg "CQRS model")

## Support
---
If you are having problems, please let me know by [raising a new issue](https://github.com/ilyam3004/ChatAppServer/issues).
