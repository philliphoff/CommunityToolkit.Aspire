using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="subscriptionId"></param>
/// <param name="schedulerEndpoint"></param>
/// <param name="dashboardEndpoint"></param>
sealed class ExistingDurableTaskSchedulerAnnotation(ParameterOrValue name, ParameterOrValue subscriptionId, ParameterOrValue schedulerEndpoint, ParameterOrValue? dashboardEndpoint) : IResourceAnnotation
{
    /// <summary>
    /// 
    /// </summary>
    public ParameterOrValue DashboardEndpoint => dashboardEndpoint ?? ParameterOrValue.Create(Constants.Scheduler.Dashboard.Endpoint);

    /// <summary>
    ///
    /// 
    /// </summary>
    public ParameterOrValue Name => name;

    /// <summary>
    /// 
    /// </summary>
    public ParameterOrValue SchedulerEndpoint => schedulerEndpoint;

    /// <summary>
    /// 
    /// </summary>
    public ParameterOrValue SubscriptionId => subscriptionId;
}