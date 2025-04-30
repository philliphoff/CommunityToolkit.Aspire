using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="innerResource"></param>
public sealed class DurableTaskSchedulerEmulatorResource(DurableTaskSchedulerResource innerResource)
    : ContainerResource(innerResource.Name), IResource
{
    /// <inheritdoc />
    public override ResourceAnnotationCollection Annotations => innerResource.Annotations;

    /// <inheritdoc />
    public override string Name => innerResource.Name;

    /// <summary>
    /// Gets or sets whether the emulator should use dynamic task hubs.
    /// </summary>
    public bool UseDynamicTaskHubs { get; set; }
}
