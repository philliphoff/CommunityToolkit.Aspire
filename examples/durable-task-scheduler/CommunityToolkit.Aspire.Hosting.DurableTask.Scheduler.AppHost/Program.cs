var builder = DistributedApplication.CreateBuilder(args);

var scheduler =
    builder.AddDurableTaskScheduler("scheduler")
           .RunAsEmulator(
                options =>
                {
                    options.Resource.UseDynamicTaskHubs = true;

                    options.WithImage("mcr.microsoft.com/dts/dts-emulator", "v0.0.6");
                });

var taskHub = scheduler.AddTaskHub("taskhub");

var webApi =
    builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_WebApi>("webapi")
           .WithReference(taskHub);

builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_Worker>("worker")
       .WithReference(webApi)
       .WithReference(taskHub);

builder.Build().Run();
