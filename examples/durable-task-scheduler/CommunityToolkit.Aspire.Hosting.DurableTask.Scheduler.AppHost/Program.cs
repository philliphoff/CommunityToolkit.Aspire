using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

bool useExisting = builder.Configuration.GetValue("Parameters:use-existing", false);

var scheduler = builder.AddDurableTaskScheduler("scheduler");

if (useExisting)
{
    scheduler.RunAsExisting(
    builder.AddParameter("scheduler-name"),
builder.AddParameter("scheduler-endpoint"));
}
else
{
    scheduler.RunAsEmulator(
        options =>
        {
            options.WithImage("mcr.microsoft.com/durable-task/dts-emulator", "latest-linux-arm64");
        });
}

var taskHub = scheduler.AddTaskHub("taskhub");

if (useExisting)
{
    taskHub.WithTaskHubName(builder.AddParameter("taskhub-name").Resource.Value);
}

var webApi =
    builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_WebApi>("webapi")
           .WithReference(taskHub);

builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_Worker>("worker")
       .WithReference(webApi)
       .WithReference(taskHub);


builder.Build().Run();
