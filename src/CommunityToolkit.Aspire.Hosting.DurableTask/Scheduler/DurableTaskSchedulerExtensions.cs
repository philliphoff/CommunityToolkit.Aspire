using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.DurableTask;
using CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;
using System.Diagnostics;

namespace Aspire.Hosting;

/// <summary>
/// 
/// </summary>
public static class DurableTaskSchedulerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> AddDurableTaskScheduler(this IDistributedApplicationBuilder builder, string name, Action<IResourceBuilder<DurableTaskSchedulerResource>>? configure = null)
    {
        DurableTaskSchedulerResource resource = new(name);

        var resourceBuilder = builder.AddResource(resource);
        
        configure?.Invoke(resourceBuilder);

        resourceBuilder.WithOpenDashboardCommand();

        return resourceBuilder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureContainer"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsEmulator(this IResourceBuilder<DurableTaskSchedulerResource> builder, Action<IResourceBuilder<DurableTaskSchedulerEmulatorResource>>? configureContainer = null)
    {
        if (builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            return builder;
        }

        builder
            .WithEndpoint(name: Constants.Scheduler.Emulator.Endpoints.Worker, scheme: "http", targetPort: 8080)
            .WithEndpoint(name: Constants.Scheduler.Emulator.Endpoints.Dashboard, scheme: "http", targetPort: 8082)
            .WithAnnotation(
                new EnvironmentCallbackAnnotation(
                    (EnvironmentCallbackContext context) =>
                    {
                        var taskHubNames =
                            builder
                                .ApplicationBuilder
                                .Resources
                                .OfType<DurableTaskHubResource>()
                                .Where(r => r.Parent == builder.Resource)
                                .Select(r => r.TaskHubName ?? r.Name)
                                .Distinct()
                                .ToList();

                        if (taskHubNames.Any())
                        {
                            context.EnvironmentVariables.Add("DTS_TASK_HUB_NAMES", String.Join(",", taskHubNames));
                        }
                    })
            )
            .WithAnnotation(
                new ContainerImageAnnotation
                {
                    Image = Constants.Scheduler.Emulator.Container.Image,
                    Tag = Constants.Scheduler.Emulator.Container.Tag
                });

        if (configureContainer is not null)
        {
            var surrogate = new DurableTaskSchedulerEmulatorResource(builder.Resource);

            var surrogateBuilder = builder.ApplicationBuilder.CreateResourceBuilder(surrogate);

            configureContainer(surrogateBuilder);

            if (surrogate.UseDynamicTaskHubs)
            {
                builder.WithAnnotation(
                    new EnvironmentCallbackAnnotation(
                        (EnvironmentCallbackContext context) =>
                        {
                            context.EnvironmentVariables.Add("DTS_USE_DYNAMIC_TASK_HUBS", "true");
                        })
                );
            }
        }

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="subscriptionId"></param>
    /// <param name="schedulerEndpoint"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsExisting(this IResourceBuilder<DurableTaskSchedulerResource> builder, string name, string subscriptionId, string schedulerEndpoint)
    {
        return RunAsExisting(builder, name, subscriptionId, schedulerEndpoint, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="subscriptionId"></param>
    /// <param name="schedulerEndpoint"></param>
    /// <param name="dashboardEndpoint"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsExisting(this IResourceBuilder<DurableTaskSchedulerResource> builder, string name, string subscriptionId, string schedulerEndpoint, string? dashboardEndpoint)
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.WithAnnotation(new ExistingDurableTaskSchedulerAnnotation(
                ParameterOrValue.Create(name),
                ParameterOrValue.Create(subscriptionId),
                ParameterOrValue.Create(schedulerEndpoint),
                dashboardEndpoint is not null ? ParameterOrValue.Create(dashboardEndpoint) : null));
            
            builder.Resource.Authentication = DurableTaskSchedulerAuthentication.Default;
        }

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="subscriptionId"></param>
    /// <param name="schedulerEndpoint"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsExisting(this IResourceBuilder<DurableTaskSchedulerResource> builder, IResourceBuilder<ParameterResource> name, IResourceBuilder<ParameterResource> subscriptionId, IResourceBuilder<ParameterResource> schedulerEndpoint)
    {
        return RunAsExisting(builder, name, subscriptionId, schedulerEndpoint, null);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="subscriptionId"></param>
    /// <param name="schedulerEndpoint"></param>
    /// <param name="dashboardEndpoint"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsExisting(this IResourceBuilder<DurableTaskSchedulerResource> builder, IResourceBuilder<ParameterResource> name, IResourceBuilder<ParameterResource> subscriptionId, IResourceBuilder<ParameterResource> schedulerEndpoint, IResourceBuilder<ParameterResource>? dashboardEndpoint)
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.WithAnnotation(new ExistingDurableTaskSchedulerAnnotation(
                ParameterOrValue.Create(name.Resource),
                ParameterOrValue.Create(subscriptionId.Resource),
                ParameterOrValue.Create(schedulerEndpoint.Resource),
                dashboardEndpoint is not null ? ParameterOrValue.Create(dashboardEndpoint.Resource) : null));

            builder.Resource.Authentication = DurableTaskSchedulerAuthentication.Default;
        }

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskHubResource> AddTaskHub(this IResourceBuilder<DurableTaskSchedulerResource> builder, string name, Action<IResourceBuilder<DurableTaskHubResource>>? configure = null)
    {
        DurableTaskHubResource taskHubResource = new(name, builder.Resource);

        var taskHubResourceBuilder = builder.ApplicationBuilder.AddResource(taskHubResource);
        
        configure?.Invoke(taskHubResourceBuilder);

        taskHubResourceBuilder.WithOpenDashboardCommand();

        return taskHubResourceBuilder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskHubResource> WithTaskHubName(this IResourceBuilder<DurableTaskHubResource> builder, string name)
    {
        builder.Resource.TaskHubName = name;

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IResourceBuilder<DurableTaskHubResource> WithTaskHubName(this IResourceBuilder<DurableTaskHubResource> builder, IResourceBuilder<ParameterResource> name)
    {
        builder.Resource.TaskHubName = name.Resource.Value;

        return builder;
    }
    
    static IResourceBuilder<T> WithOpenDashboardCommand<T>(this IResourceBuilder<T> builder) where T : IResourceWithDashboard
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.WithCommand(
                builder.Resource.IsTaskHub ? "durabletask-hub-open-dashboard" : "durabletask-scheduler-open-dashboard",
                "Open Dashboard",
                async context =>
                {
                    var dashboardEndpoint = await builder.Resource.DashboardEndpointExpression.GetValueAsync(context.CancellationToken);

                    Process.Start(new ProcessStartInfo { FileName = dashboardEndpoint, UseShellExecute = true });

                    return CommandResults.Success();
                },
                new()
                {
                    Description = "Open the Durable Task Scheduler Dashboard",
                    IconName = "GlobeArrowForward",
                    IsHighlighted = builder.Resource.IsTaskHub,
                });
        }

        return builder;
    }
}
