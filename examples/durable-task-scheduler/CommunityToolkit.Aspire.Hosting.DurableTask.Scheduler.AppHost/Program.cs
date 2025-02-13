var builder = DistributedApplication.CreateBuilder(args);

var scheduler = builder.AddDurableTaskScheduler("scheduler")
       .RunAsEmulator(
           options =>
           {
               options.WithImage("mcr.microsoft.com/durable-task/dts-emulator", "latest-linux-arm64");
           });

var taskHub = scheduler.AddTaskHub("taskhub");

var webApi =
    builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_WebApi>("webapi")
           .WithReference(taskHub);

builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_Worker>("worker")
       .WithReference(webApi)
       .WithReference(taskHub);


builder.Build().Run();
