using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

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
    public static IResourceBuilder<DurableTaskSchedulerResource> AddDurableTaskScheduler(this IDistributedApplicationBuilder builder, string name, Action<DurableTaskSchedulerResource>? configure = null)
    {
        DurableTaskSchedulerResource resource = new(name);

        configure?.Invoke(resource);

        return builder.AddResource(resource);
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
            .WithEndpoint(name: "worker", scheme: "http", targetPort: 8080)
            .WithEndpoint(name: "dashboard", scheme: "http", targetPort: 8082)
            .WithAnnotation(new ContainerRuntimeArgsCallbackAnnotation(
                args =>
                {
                    args.Add("--label");
                    args.Add("com.microsoft.tooling=aspire");
                }))
            .WithAnnotation(
                new ContainerImageAnnotation
                {
                    Image = "dts-emulator",
                    Tag = "latest-amd64"
                });

        if (configureContainer is not null)
        {
            var surrogate = new DurableTaskSchedulerEmulatorResource(builder.Resource);

            var surrogateBuilder = builder.ApplicationBuilder.CreateResourceBuilder(surrogate);

            configureContainer(surrogateBuilder);
        }

        return builder;
    }
}
