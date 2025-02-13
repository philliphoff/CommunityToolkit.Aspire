using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="schedulerEndpoint"></param>
/// <param name="dashboardEndpoint"></param>
public sealed class ExistingDurableTaskSchedulerAnnotation(object name, object schedulerEndpoint, object? dashboardEndpoint) : IResourceAnnotation
{
    /// <summary>
    /// 
    /// </summary>
    public object DashboardEndpoint => dashboardEndpoint ?? Constants.Scheduler.Dashboard.Endpoint;

    /// <summary>
    /// 
    /// </summary>
    public object Name => name;

    /// <summary>
    /// 
    /// </summary>
    public object SchedulerEndpoint => schedulerEndpoint;
}