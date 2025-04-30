using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.DurableTask;
using CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;
using Microsoft.AspNetCore.Authentication;
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

        resourceBuilder.WithOpenDashboardCommand(
            resourceBuilder.Resource.DashboardEndpointExpression);

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
            .WithAnnotation(new ContainerRuntimeArgsCallbackAnnotation(
                args =>
                {
                    args.Add("--label");
                    args.Add("com.microsoft.tooling=aspire");
                }))
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
                ParameterWrapper.Create(name),
                ParameterWrapper.Create(subscriptionId),
                ParameterWrapper.Create(schedulerEndpoint),
                dashboardEndpoint is not null ? ParameterWrapper.Create(dashboardEndpoint) : null));
            
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
                ParameterWrapper.Create(name.Resource),
                ParameterWrapper.Create(subscriptionId.Resource),
                ParameterWrapper.Create(schedulerEndpoint.Resource),
                dashboardEndpoint is not null ? ParameterWrapper.Create(dashboardEndpoint.Resource) : null));

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

        taskHubResourceBuilder.WithOpenDashboardCommand(
            taskHubResource.DashboardEndpointExpression,
            isTaskHub: true);

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
    
    static IResourceBuilder<T> WithOpenDashboardCommand<T>(this IResourceBuilder<T> builder, ReferenceExpression dashboardEndpointExpression, bool isTaskHub = false) where T : IResourceWithDashboard
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.WithCommand(
                isTaskHub ? "durabletask-hub-open-dashboard" : "durabletask-scheduler-open-dashboard",
                "Open Dashboard",
                async context =>
                {
                    var dashboardEndpoint = await builder.Resource.DashboardEndpointExpression.GetValueAsync(context.CancellationToken);

                    Process.Start(new ProcessStartInfo { FileName = dashboardEndpoint, UseShellExecute = true });

                    return CommandResults.Success();
                },
                new()
                {
                    IconName = "GlobeArrowForward",
                    IsHighlighted = isTaskHub
                });
        }

        return builder;
    }
}
