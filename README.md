# Chat App

Chat app based on the [TodoApp](https://github.com/marinasundstrom/todo-app) project.

Watch the [video](https://youtu.be/q6N_O9YRs1U)

## Screenshot

![Screenshot](/images/screenshot.png)

## Background

This project was created as part of an assignment for a Job interview process. Coincidentally, I had been thinking of renovating my old "[Messenger](https://github.com/marinasundstrom/YourBrand/tree/main/Messenger/Messenger.UI)" project the days before receiving the assignment.

This assignment gave me a scope and some motivation.

## Features

Here are the main features of the app:

* View and create channels.
* Post messages in specific channels. Edit and delete them.
* Admin commands - sent in channel.

Other features:
* User registration - aka "Welcome screen".
* Light and Dark mode
* Localization - English and Swedish

For more details, read the [design](/docs/design.md) document.

### Futures

* Incremental load of history
* Read receipt

## Project
The app consists of a Frontend built with Blazor WebAssembly, and a Backend with ASP.NET Core.

Major technical characteristics of the project are listed below.

###  Architecture
* Clean Architecture (CA) in app project, with Vertical-slices architecture (VSA)
  * Combining all layers into one project with focus on features.
  * Using Domain-driven design (DDD) practices
* Event-driven architecture - Domain Events
  
### Technologies
* ASP.NET Core
  * Endpoints - "Minimal API" with versioning
  * SignalR
  * OpenAPI

* Frontend/UI
  * Blazor
  * MudBlazor (component framework)

* Azure SQL Server
* IdentityServer for authentication - with seeded users Alice and Bob.

Unused but available technologies:
* RabbitMQ (for asynchronous messaging)
  * MassTransit 
* Redis (for distributed cache)

Other:
* Open Telemetry - with Zipkin
* Health checks

### Tests
* Application logic tests
* Domain model test
* Integration tests - with Test host and Testcontainers

## Running the project

### Tye

[Project Tye](https://github.com/dotnet/tye) is an experimental developer tool for .NET from Microsoft. It allows you to speed up development time by locally orchestrating your services, both projects and containers. Tye also helps with service discovery.

Install the Tye [CLI tool](https://github.com/dotnet/tye/blob/main/docs/getting_started.md). Make sure to have Docker Desktop installed.

To run the solution:

```
tye run
```

Dashboard: http://localhost:8000/

With Watch feature:

```
tye run --watch
```

### Docker Compose

TBA

```
docker-compose up
```

### Seeding the databases

In order for the databases to be created and for the app to function, you need to seed the databases for the ```Web``` and ```IdentityService``` projects.

The services, in particular the databases, have to be running for this to work. 

The seeding code target databases that have been defined in the ```appsettings.json``` files in each project.

#### Web

When in the Web project:

```sh
dotnet run -- --seed
```

#### IdentityService

When in the IdentityService project:

```sh
dotnet run -- /seed
```

### Services

These are the services:

* Frontend: https://localhost:5021/
* Backend: https://localhost:5001/
  * Swagger UI: https://localhost:5001/swagger/

### Login credentials

Here are the users available to login as, provided that you have seeded the database.

```
Username: alice 
Password: alice

Username: bob 
Password: bob
```

### Swagger UI

Hosted at: https://localhost:5001/swagger/
