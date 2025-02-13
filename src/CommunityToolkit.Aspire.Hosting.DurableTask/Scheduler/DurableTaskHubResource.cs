using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="parent"></param>
public class DurableTaskHubResource(string name, DurableTaskSchedulerResource parent)
    : Resource(name), IResourceWithConnectionString, IResourceWithEndpoints, IResourceWithParent<DurableTaskSchedulerResource>
{
    ReferenceExpression IResourceWithConnectionString.ConnectionStringExpression =>
        ReferenceExpression.Create($"{(this.Parent as IResourceWithConnectionString).ConnectionStringExpression};TaskHub={this.ResolveTaskHubName()}");

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression DashboardEndpointExpression =>
        this.GetDashboardEndpoint();

    /// <summary>
    /// 
    /// </summary>
    public DurableTaskSchedulerResource Parent => parent;

    /// <summary>
    /// 
    /// </summary>
    public string? TaskHubName { get; set; }

    ReferenceExpression GetDashboardEndpoint()
    {
        return ReferenceExpression.Create(
            $"{this.Parent.DashboardEndpoint}/subscriptions/default/schedulers/default/taskhubs/{this.ResolveTaskHubName()}?endpoint={QueryParameterReference.Create(this.Parent.DashboardEndpoint)}");
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
