using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
public sealed class DurableTaskSchedulerResource(string name)
    : Resource(name), IResourceWithConnectionString, IResourceWithEndpoints, IResourceWithDashboard
{
    private EndpointReference EmulatorDashboardEndpoint => new(this, Constants.Scheduler.Emulator.Endpoints.Dashboard);
    private EndpointReference EmulatorSchedulerEndpoint => new(this, Constants.Scheduler.Emulator.Endpoints.Worker);

    /// <summary>
    /// 
    /// </summary>
    public string? Authentication { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ClientId { get; set; }

    /// <inheritdoc />
    public ReferenceExpression ConnectionStringExpression =>
        this.CreateConnectionString();

    /// <summary>
    /// 
    /// </summary>
    public Uri? DashboardEndpoint { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression DashboardEndpointExpression =>
        this.CreateDashboardEndpoint();

    /// <summary>
    /// 
    /// </summary>
    public bool IsEmulator => this.IsContainer();

    /// <summary>
    /// 
    /// </summary>
    public Uri? SchedulerEndpoint { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression SchedulerEndpointExpression =>
        this.CreateSchedulerEndpoint();

    /// <summary>
    /// 
    /// </summary>
    public string? SchedulerName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression? SchedulerNameExpression =>
        this.ResolveSchedulerName();

    /// <summary>
    /// 
    /// </summary>
    public string? SubscriptionId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression? SubscriptionIdExpression =>
        this.ResolveSubscriptionId();

    ReferenceExpression? ResolveSubscriptionId()
    {
        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.SubscriptionId}");
        }
        
        if (this.SubscriptionId is not null)
        {
            return ReferenceExpression.Create($"{this.SubscriptionId}");
        }

        return null;
    }
    
    ReferenceExpression? ResolveSchedulerName()
    {
        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.Name}");
        }

        if (this.SchedulerName is not null)
        {
            return ReferenceExpression.Create($"{this.SchedulerName}");
        }

        return null;
    }

    ReferenceExpression CreateConnectionString(string? applicationName = null)
    {
        string connectionString = $"Authentication={this.Authentication ?? DurableTaskSchedulerAuthentication.None}";
        
        if (this.ClientId is not null)
        {
            connectionString += $";ClientId={this.ClientId}";
        }
        
        return ReferenceExpression.Create($"Endpoint={this.SchedulerEndpointExpression};{connectionString}");
    }
    
    ReferenceExpression CreateDashboardEndpoint()
    {
        if (this.IsEmulator)
        {
            // NOTE: Container endpoints do not include the trailing slash.
            return ReferenceExpression.Create($"{this.EmulatorDashboardEndpoint}/");
        }

        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.DashboardEndpoint}");
        }

        if (this.DashboardEndpoint is not null)
        {
            return ReferenceExpression.Create($"{this.DashboardEndpoint.ToString()}");
        }

        return ReferenceExpression.Create($"{Constants.Scheduler.Dashboard.Endpoint.ToString()}");
    }
    
    ReferenceExpression CreateSchedulerEndpoint()
    {
        if (this.IsEmulator)
        {
            // NOTE: Container endpoints do not include the trailing slash.
            return ReferenceExpression.Create($"{this.EmulatorSchedulerEndpoint}/");
        }

        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.SchedulerEndpoint}");
        }

        if (this.SchedulerEndpoint is not null)
        {
            return ReferenceExpression.Create($"{this.SchedulerEndpoint.ToString()}");
        }

        throw new InvalidOperationException("Scheduler endpoint is not set.");
    }
}
