using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

sealed class ExistingDurableTaskSchedulerAnnotation(ParameterOrValue name, ParameterOrValue subscriptionId, ParameterOrValue schedulerEndpoint, ParameterOrValue? dashboardEndpoint) : IResourceAnnotation
{
    public ParameterOrValue DashboardEndpoint => dashboardEndpoint ?? ParameterOrValue.Create(Constants.Scheduler.Dashboard.Endpoint);

    public ParameterOrValue Name => name;

    public ParameterOrValue SchedulerEndpoint => schedulerEndpoint;

    public ParameterOrValue SubscriptionId => subscriptionId;
}
