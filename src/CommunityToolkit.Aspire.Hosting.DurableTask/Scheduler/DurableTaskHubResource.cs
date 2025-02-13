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
    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{this.Parent.ConnectionStringExpression};TaskHub={this.TaskHubName ?? Constants.Scheduler.TaskHub.DefaultName}");

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression DashboardEndpointExpression =>
        this.Parent.DashboardEndpointExpression;

    /// <summary>
    /// 
    /// </summary>
    public DurableTaskSchedulerResource Parent => parent;

    /// <summary>
    /// 
    /// </summary>
    public string? TaskHubName { get; set; }
}
