using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.DurableTask;
using CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;
using System.Diagnostics;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding and configuring Durable Task Scheduler resources to the application model.
/// </summary>
public static class DurableTaskSchedulerExtensions
{
    /// <summary>
    /// Adds a Durable Task Scheduler resource to the application model.
    /// </summary>
    /// <param name="builder">The builder for the distributed application.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="configure">(Optional) Callback that exposes the resource allowing for customization.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskSchedulerResource}" />.</returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> AddDurableTaskScheduler(this IDistributedApplicationBuilder builder, [ResourceName] string name, Action<IResourceBuilder<DurableTaskSchedulerResource>>? configure = null)
    {
        DurableTaskSchedulerResource resource = new(name);

        var resourceBuilder = builder.AddResource(resource);
        
        configure?.Invoke(resourceBuilder);

        resourceBuilder.WithOpenDashboardCommand();

        return resourceBuilder;
    }

    /// <summary>
    /// Configures a Durable Task Scheduler resource to be emulated.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler resource builder.</param>
    /// <param name="configureContainer">Callback that exposes the underlying container used for emulation allowing for customization.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskSchedulerResource}" />.</returns>
    /// <remarks>
    /// This version of the package defaults to the <inheritdoc cref="Constants.Scheduler.Emulator.Container.Tag" /> tag of the <inheritdoc cref="Constants.Scheduler.Emulator.Container.Image" /> container image.
    /// </remarks>
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

        builder.Resource.Authentication ??= DurableTaskSchedulerAuthentication.None;

        return builder;
    }

    /// <summary>
    /// Marks the resource as an existing Durable Task Scheduler instance when the application is running.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler resource builder.</param>
    /// <param name="name">The name of the Durable Task Scheduler instance.</param>
    /// <param name="subscriptionId">The ID of the Azure subscription in which the Durable Task Scheduler instance resides.</param>
    /// <param name="schedulerEndpoint">The endpoint of the Durable Task Scheduler instance.</param>
    /// <param name="dashboardEndpoint">(Optional) The endpoint of the dashboard for the Durable Task Scheduler instance.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskSchedulerResource}" />.</returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsExisting(this IResourceBuilder<DurableTaskSchedulerResource> builder, string name, string subscriptionId, Uri schedulerEndpoint, Uri? dashboardEndpoint = null)
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.WithAnnotation(new ExistingDurableTaskSchedulerAnnotation(
                ParameterOrValue.Create(name),
                ParameterOrValue.Create(subscriptionId),
                ParameterOrValue.Create(schedulerEndpoint.ToString()),
                dashboardEndpoint is not null ? ParameterOrValue.Create(dashboardEndpoint.ToString()) : null));
            
            builder.Resource.Authentication ??= DurableTaskSchedulerAuthentication.Default;
        }

        return builder;
    }
   
    /// <summary>
    /// Marks the resource as an existing Durable Task Scheduler instance when the application is running.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler resource builder.</param>
    /// <param name="name">The name of the Durable Task Scheduler instance.</param>
    /// <param name="subscriptionId">The ID of the Azure subscription in which the Durable Task Scheduler instance resides.</param>
    /// <param name="schedulerEndpoint">The endpoint of the Durable Task Scheduler instance.</param>
    /// <param name="dashboardEndpoint">(Optional) The endpoint of the dashboard for the Durable Task Scheduler instance.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskSchedulerResource}" />.</returns>
    public static IResourceBuilder<DurableTaskSchedulerResource> RunAsExisting(this IResourceBuilder<DurableTaskSchedulerResource> builder, IResourceBuilder<ParameterResource> name, IResourceBuilder<ParameterResource> subscriptionId, IResourceBuilder<ParameterResource> schedulerEndpoint, IResourceBuilder<ParameterResource>? dashboardEndpoint = null)
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.WithAnnotation(new ExistingDurableTaskSchedulerAnnotation(
                ParameterOrValue.Create(name.Resource),
                ParameterOrValue.Create(subscriptionId.Resource),
                ParameterOrValue.Create(schedulerEndpoint.Resource),
                dashboardEndpoint is not null ? ParameterOrValue.Create(dashboardEndpoint.Resource) : null));

            builder.Resource.Authentication ??= DurableTaskSchedulerAuthentication.Default;
        }

        return builder;
    }

    /// <summary>
    /// Adds a Durable Task Scheduler task hub resource to the application model.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler resource builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="configure">(Optional) Callback that exposes the resource allowing for customization.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskHubResource}" />.</returns>
    public static IResourceBuilder<DurableTaskHubResource> AddTaskHub(this IResourceBuilder<DurableTaskSchedulerResource> builder, [ResourceName] string name, Action<IResourceBuilder<DurableTaskHubResource>>? configure = null)
    {
        DurableTaskHubResource taskHubResource = new(name, builder.Resource);

        var taskHubResourceBuilder = builder.ApplicationBuilder.AddResource(taskHubResource);
        
        configure?.Invoke(taskHubResourceBuilder);

        taskHubResourceBuilder.WithOpenDashboardCommand();

        return taskHubResourceBuilder;
    }

    /// <summary>
    /// Sets the name of the task hub if different from the resource name.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler task hub resource builder.</param>
    /// <param name="name">The name of the task hub.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskHubResource}" />.</returns>
    public static IResourceBuilder<DurableTaskHubResource> WithTaskHubName(this IResourceBuilder<DurableTaskHubResource> builder, string name)
    {
        builder.Resource.TaskHubName = name;

        return builder;
    }

    /// <summary>
    /// Sets the name of the task hub if different from the resource name.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler task hub resource builder.</param>
    /// <param name="name">The name of the task hub.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskHubResource}" />.</returns>
    public static IResourceBuilder<DurableTaskHubResource> WithTaskHubName(this IResourceBuilder<DurableTaskHubResource> builder, IResourceBuilder<ParameterResource> name)
    {
        builder.Resource.TaskHubName = name.Resource.Value;

        return builder;
    }
    
    /// <summary>
    /// Enables the use of dynamic task hubs for the emulator.
    /// </summary>
    /// <param name="builder">The Durable Task Scheduler emulator resource builder.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{DurableTaskSchedulerEmulatorResource}" />.</returns>
    /// <remarks>
    /// Using dynamic task hubs eliminates the requirement that they be pre-defined,
    /// which can be useful when the same emulator instance is used across sessions.
    /// </remarks>
    public static IResourceBuilder<DurableTaskSchedulerEmulatorResource> WithDynamicTaskHubs(this IResourceBuilder<DurableTaskSchedulerEmulatorResource> builder)
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsPublishMode)
        {
            builder.Resource.UseDynamicTaskHubs = true;
        }

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
