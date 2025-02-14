using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="subscriptionId"></param>
/// <param name="schedulerEndpoint"></param>
/// <param name="dashboardEndpoint"></param>
sealed class ExistingDurableTaskSchedulerAnnotation(ParameterWrapper name, ParameterWrapper subscriptionId, ParameterWrapper schedulerEndpoint, ParameterWrapper? dashboardEndpoint) : IResourceAnnotation
{
    /// <summary>
    /// 
    /// </summary>
    public ParameterWrapper DashboardEndpoint => dashboardEndpoint ?? ParameterWrapper.Create(Constants.Scheduler.Dashboard.Endpoint);

    /// <summary>
    ///
    /// 
    /// </summary>
    public ParameterWrapper Name => name;

    /// <summary>
    /// 
    /// </summary>
    public ParameterWrapper SchedulerEndpoint => schedulerEndpoint;

    /// <summary>
    /// 
    /// </summary>
    public ParameterWrapper SubscriptionId => subscriptionId;
}