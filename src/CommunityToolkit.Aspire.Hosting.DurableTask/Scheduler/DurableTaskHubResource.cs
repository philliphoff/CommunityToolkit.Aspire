using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="parent"></param>
public class DurableTaskHubResource(string name, DurableTaskSchedulerResource parent)
    : Resource(name), IResourceWithConnectionString, IResourceWithEndpoints, IResourceWithParent<DurableTaskSchedulerResource>, IResourceWithDashboard
{
    /// <inheritdoc />
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{this.Parent.ConnectionStringExpression};TaskHub={this.ResolveTaskHubName()}");

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression DashboardEndpointExpression =>
        this.GetDashboardEndpoint();

    /// <inheritdoc />
    public DurableTaskSchedulerResource Parent => parent;

    /// <summary>
    /// 
    /// </summary>
    public string? TaskHubName { get; set; }

    ReferenceExpression GetDashboardEndpoint()
    {
        var defaultValue = ReferenceExpression.Create($"default");

        // NOTE: The endpoint is expected to have the trailing slash.
        return ReferenceExpression.Create(
            $"{this.Parent.DashboardEndpointExpression}subscriptions/{this.Parent.SubscriptionIdExpression ?? defaultValue}/schedulers/{this.Parent.SchedulerNameExpression ?? defaultValue}/taskhubs/{this.ResolveTaskHubName()}?endpoint={QueryParameterReference.Create(this.Parent.SchedulerEndpointExpression)}");
    }

    string ResolveTaskHubName()
    {
        if (TaskHubName is not null)
        {
            return TaskHubName;
        }

        if (this.Parent.IsEmulator)
        {
            return Constants.Scheduler.TaskHub.DefaultName;
        }

        return this.Name;
    }
}
