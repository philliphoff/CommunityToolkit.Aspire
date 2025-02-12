using CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
