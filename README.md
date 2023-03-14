# Roomify server following Clean Architecture, CQRS and Mediator patterns

Roomify server is a simple server that was build following Clean Architecture, CQRS and Mediator patters. The application is written in C# using ASP.NET Core 7.0 and SignalR. You can use this application as a template for your own ASP.NET Core Web API's following the principles of Clean Architecture. It is a server part of the chat application. The client part is [here](https://github.com/ilyam3004/Roomify).

<p align="center">
    <img src="https://img.shields.io/github/v/release/ilyam3004/Roomify-Server"/>
    <img src="https://img.shields.io/github/actions/workflow/status/ilyam3004/Roomify-Server/main.yml"/>
    <img src="https://img.shields.io/github/license/ilyam3004/Roomify-Server"/>
</p> 

## Technologies

- [ASP.NET 7](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/introduction-to-signalr) - for implementing websocket connection between server and client.
- [MediatR](https://github.com/jbogard/MediatR) - for implementing Mediator pattern.
- [Dapper](https://github.com/DapperLib/Dapper) - ORM
- [Mapster](https://github.com/MapsterMapper/Mapster) - for mapping between entities and DTOs.
- [FluentValidation](https://github.com/FluentValidation/FluentValidation) - for validating requests.
- [ErrorOr](https://www.nuget.org/packages/ErrorOr/0.1.0) - for handling errors in Application layer.
- [xUnit](https://github.com/xunit/xunit) - for unit testing.
- [Moq](https://github.com/moq/moq4) - for isolating dependencies in unit tests.
- [AutoFixture](https://github.com/AutoFixture/AutoFixture) - for generating test data in unit tests.

## Clean Architecture

![](/docs/img/clean-architecture-model.jpg "Clean Architecture model")

The architecture of the system is divided into four layers: Domain, Application, Infrastructure, and Presentation.

### Domain
The Domain layer encompasses all entities, enums, exceptions, errors and logic that are specific to the business domain of the system.

### Application
The Application layer contains all application logic. It depends on the Domain layer, but is independent of any other layer or project. This layer contains command and query handlers, defines interfaces that other layers can implement. For instance, if the application requires access to a user repository, a new interface is added to the Application layer and an implementation is created within the Infrastructure layer.

### Infrastructure
The infrastructure layer refers to the layer that handles low-level tasks such as network communication, database access, and file system operations. It serves as a bridge between the application layer and the underlying infrastructure, providing a clean separation of concerns. This includes managing connections to databases, caching data and handling security. One of the main benefits of using an infrastructure layer is that it allows for greater flexibility and scalability in the API. By separating the application logic from the underlying infrastructure, changes to one layer can be made without affecting the other. This makes it easier to modify, test, and deploy the API as needed.

### Presentation
Finally, the Presentation layer is an ASP.NET Web API which gives us an opportunity to build SPA applications, Mobile apps or Desktop clients and so far. This layer is responsible for all user interface logic and depends on the Application layer.

## CQRS and Mediator patterns
The Mediator pattern and CQRS (Command Query Responsibility Segregation) pattern are both software design patterns that promote loose coupling and separation of concerns in a system.
### Mediator
Mediator provides a simple and elegant way to implement communication between different components of a system without directly coupling them together. In this pattern, each component sends messages to a mediator, which then distributes those messages to other components that have registered to handle them. This approach allows for a decoupled and loosely coupled architecture, where components do not need to know about each other's existence, promoting scalability and maintainability. Overall, MediatR promotes the Single Responsibility Principle (SRP) and enhances the modularity and testability of the system.
### CQRS
CQRS (Command Query Responsibility Segregation) is a software design pattern that separates the operations that modify state from those that read state in a system. This separation allows for different optimizations and scaling strategies for the two types of operations. In a CQRS architecture, commands represent actions that change the state of the system, while queries represent requests for information about the system's current state. By separating these concerns, a CQRS system can be optimized for both high write throughput and fast query performance. However, implementing a CQRS architecture can be complex and may require significant changes to existing systems.

![](/docs/img/cqrs-model.jpg "CQRS model")

## Database configuration

This application is configured to use Azure SQL Server database in production. If you would like to use this application you need to prepare SQL Server or Azure SQL Server database. SQL script for creating database structure is [here](docs/Database/database.sql). After preparing database you need to configure connection string in [appsettings.json](./src/ChatApp.Api/appsettings.json) file in SqlConnection section: 

```json
{
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "SqlConnection": "my_sql_connection_string"
    }
}
```
In this application you can upload images to Cloudinary. If you want to use this feature you need to create Cloudinary account and configure Cloudinary account data in [appsettings.json](./src/ChatApp.Api/appsettings.json) file in Cloudinary section: 

```json
{
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "SqlConnection": "sql_connection_string"
    },
    "Cloudinary": {
      "CloudName": "cloud_name",
      "ApiKey": "api_key",
      "ApiSecret": "api_secret"
    }
}
```

## How to run
After preparing database you can run this application. You can run this application in Visual Studio or using dotnet CLI. To run this application using dotnet CLI you need to navigate to the root folder of the solution and run the following command:

```bash
dotnet run --project src/ChatApp.Api
```

Then, to open the websocket connection, simply make this request from your client application or testing platform with the request body containing the user's name,  chat room name and if you want you can add avatar link:
```http
http://localhost:{host}/chatHub
```
```json
{
    "Username": "user",
    "RoomName": "room",
    "Avatar": "link_to_avatar"
}
```
After opening connection you can invoke methods of [ChatHub](/src/ChatApp.Api/Hubs/ChatHub.cs) from the client side. An example of client application which show all functionality of this server is [here](https://www.rmify.com/lobby).

## Support

If you are having problems, please let me know by [raising a new issue](https://github.com/ilyam3004/Roomify-Server/issues).

## License

This project is licensed with the [MIT license](LICENSE).
