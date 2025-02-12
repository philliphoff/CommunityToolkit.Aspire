var builder = DistributedApplication.CreateBuilder(args);

builder.AddDurableTaskScheduler("scheduler")
       .RunAsEmulator();

builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_Worker>("worker");

builder.AddProject<Projects.CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_WebApi>("webapi");

builder.Build().Run();
