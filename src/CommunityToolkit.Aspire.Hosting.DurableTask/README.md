# CommunityToolkit.Aspire.Hosting.DurableTask library

Provides extension methods and resource definitions for a .NET Aspire AppHost to configure Azure Durable Task resources.

## Getting started

### Install the package

In your AppHost project, install the .NET Aspire Durable Task Hosting library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.DurableTask
```

## Durable Task Scheduler usage example

### Using the emulator

In the _Program.cs_ file of `AppHost`, add Durable Task Scheduler resources and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var scheduler = builder.AddDurableTaskScheduler("scheduler")
                       .RunAsEmulator();

var taskHub = scheduler.AddDurableTaskHub("taskhub");

builder.AddProject<Projects.MyApp>("myapp")
       .WithReference(taskHub);

builder.Build().Run();
```

### Using an existing Durable Task Scheduler

In the _Program.cs_ file of `AppHost`, add Durable Task Scheduler resources and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var scheduler = builder.AddDurableTaskScheduler("scheduler")
                       .RunAsExisting(
                           name: "myscheduler",
                           subscriptionId: "mysubscription",
                           schedulerEndpoint: "https://myscheduler.durabletask.io");

var taskHub = scheduler.AddDurableTaskHub("taskhub")
                       .WithTaskHubName("mytaskhub");

builder.AddProject<Projects.MyApp>("myapp")
       .WithReference(taskHub);

builder.Build().Run();
```

## Additional documentation

https://github.com/microsoft/durabletask-dotnet

## Feedback & contributing

https://github.com/dotnet/aspire
