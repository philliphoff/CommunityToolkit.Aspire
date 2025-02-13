# CommunityToolkit.Aspire.Hosting.DurableTask library

Provides extension methods and resource definitions for a .NET Aspire AppHost to configure Azure Durable Task resources.

## Getting started

### Install the package

In your AppHost project, install the .NET Aspire Durable Task Hosting library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.DurableTask
```

## Usage example

Then, in the _Program.cs_ file of `AppHost`, add Dapr resources and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var scheduler = builder.AddDurableTaskScheduler("scheduler");

var taskHub = scheduler.AddDurableTaskHub("taskhub");

builder.AddProject<Projects.MyApp>("myapp")
       .WithReference(taskHub);

builder.Build().Run();
```

## Additional documentation

https://github.com/microsoft/durabletask-dotnet

## Feedback & contributing

https://github.com/dotnet/aspire
