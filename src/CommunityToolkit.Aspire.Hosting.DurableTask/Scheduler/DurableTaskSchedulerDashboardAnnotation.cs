using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

sealed class DurableTaskSchedulerDashboardAnnotation(ParameterOrValue? subscriptionId, ParameterOrValue? dashboardEndpoint)
    : IResourceAnnotation
{
    public ParameterOrValue? DashboardEndpoint => dashboardEndpoint;

    public ParameterOrValue? SubscriptionId => subscriptionId;
}
