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

        builder.WithOpenDashboardCommand(
            builder.Resource.DashboardEndpointExpression);

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

        if (builder.Resource.IsEmulator)
        {
            taskHubResourceBuilder.WithOpenDashboardCommand(
                taskHubResource.DashboardEndpointExpression,
                isTaskHub: true);
        }

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

    static IResourceBuilder<T> WithOpenDashboardCommand<T>(this IResourceBuilder<T> builder, ReferenceExpression dashboardEndpointExpression, bool isTaskHub = false) where T : IResource
    {
        return builder.WithCommand(
            isTaskHub ? "durabletask-hub-open-dashboard" : "durabletask-scheduler-open-dashboard",
            "Open Dashboard",
            async context =>
            {
                var dashboardEndpoint = await dashboardEndpointExpression.GetValueAsync(context.CancellationToken);

                Process.Start(new ProcessStartInfo { FileName = dashboardEndpoint, UseShellExecute = true });

                return CommandResults.Success();
            },
            iconName: "GlobeArrowForward",
            isHighlighted: isTaskHub);
    }
}
