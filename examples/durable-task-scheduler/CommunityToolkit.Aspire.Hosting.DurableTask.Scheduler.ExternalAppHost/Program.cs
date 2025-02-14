using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var scheduler =
    builder.AddDurableTaskScheduler("scheduler")
           .RunAsExisting(
                builder.AddParameter("scheduler-name"),
                builder.AddParameter("scheduler-subscription"),
                builder.AddParameter("scheduler-endpoint"));

var taskHub =
    scheduler.AddTaskHub("taskhub")
             .WithTaskHubName(builder.AddParameter("taskhub-name"));

var webApi =
    builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_WebApi>("webapi")
           .WithReference(taskHub);

builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_Worker>("worker")
       .WithReference(webApi)
       .WithReference(taskHub);

builder.Build().Run();
