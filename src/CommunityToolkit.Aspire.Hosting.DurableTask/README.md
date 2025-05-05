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

> NOTE: When referencing the taskhub resource, the connection string will include the task hub name whereas the connection string for a scheduler resource will not. Use the latter when an application specifies the task hub name separately from the connection string (e.g. Azure Durable Functions).

### Using an existing Durable Task Scheduler

In the _Program.cs_ file of `AppHost`, add Durable Task Scheduler resources and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var scheduler =
    builder.AddDurableTaskScheduler("scheduler")
           .RunAsExisting(builder.AddParameter("scheduler-connection-string"));

var taskHub =
    scheduler.AddTaskHub("taskhub")
             .WithTaskHubName(builder.AddParameter("taskhub-name"));

builder.AddProject<Projects.MyApp>("myapp")
       .WithReference(taskHub);

builder.Build().Run();
```

## Additional documentation

https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-task-scheduler/durable-task-scheduler

## Feedback & contributing

https://github.com/CommunityToolkit/Aspire
