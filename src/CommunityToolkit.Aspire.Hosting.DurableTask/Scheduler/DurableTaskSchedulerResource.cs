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
    
    ReferenceExpression IResourceWithConnectionString.ConnectionStringExpression =>
        this.CreateConnectionString();

    /// <summary>
    /// 
    /// </summary>
    public bool IsEmulator => this.IsContainer();

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression DashboardEndpoint =>
        this.CreateDashboardEndpoint();

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression SchedulerEndpoint =>
        this.CreateSchedulerEndpoint();

    internal ReferenceExpression ResolveSubscriptionId(ReferenceExpression defaultValue)
    {
        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.SubscriptionId}");
        }

        return defaultValue;
    }
    
    internal ReferenceExpression ResolveSchedulerName(ReferenceExpression defaultValue)
    {
        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.Name}");
        }

        return defaultValue;
    }

    private ReferenceExpression CreateConnectionString(string? applicationName = null)
    {
        string connectionString = $"Authentication={this.Authentication ?? DurableTaskSchedulerAuthentication.None}";
        
        if (this.ClientId is not null)
        {
            connectionString += $";ClientId={this.ClientId}";
        }
        
        return ReferenceExpression.Create($"Endpoint={this.SchedulerEndpoint};{connectionString}");
    }
    
    private ReferenceExpression CreateDashboardEndpoint()
    {
        if (this.IsEmulator)
        {
            return ReferenceExpression.Create($"{this.EmulatorDashboardEndpoint}");
        }

        if (!this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{Constants.Scheduler.Dashboard.Endpoint.ToString()}");
        }

        return ReferenceExpression.Create($"{annotation.DashboardEndpoint}");
    }
    
    private ReferenceExpression CreateSchedulerEndpoint()
    {
        if (this.IsEmulator)
        {
            return ReferenceExpression.Create($"{this.EmulatorSchedulerEndpoint}");
        }

        if (!this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            throw new InvalidOperationException("Scheduler endpoint is not set.");
        }

        return ReferenceExpression.Create($"{annotation.SchedulerEndpoint}");
    }
}
