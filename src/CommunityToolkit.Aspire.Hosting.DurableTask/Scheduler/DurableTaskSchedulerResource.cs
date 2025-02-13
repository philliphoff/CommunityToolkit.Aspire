using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
public sealed class DurableTaskSchedulerResource(string name)
    : Resource(name), IResourceWithConnectionString, IResourceWithEndpoints
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
    
    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
        this.CreateConnectionString();

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression DashboardEndpointExpression =>
        this.CreateDashboardEndpoint();

    /// <summary>
    /// 
    /// </summary>
    public Uri SchedulerEndpoint { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public bool IsEmulator => this.IsContainer();

    private ReferenceExpression CreateConnectionString(string? applicationName = null)
    {
        string connectionString = $"Authentication={this.Authentication ?? DurableTaskSchedulerAuthentication.None}";
        
        if (this.ClientId is not null)
        {
            connectionString += $";ClientId={this.ClientId}";
        }
        
        if (this.IsEmulator)
        {
            return ReferenceExpression.Create($"Endpoint={this.EmulatorSchedulerEndpoint};{connectionString}");
        }
        else
        {
            return ReferenceExpression.Create($"Endpoint={this.SchedulerEndpoint.ToString()};{connectionString}");
        }
    }
    
    private ReferenceExpression CreateDashboardEndpoint()
    {
        if (this.IsEmulator)
        {
            return ReferenceExpression.Create($"{this.EmulatorDashboardEndpoint}");
        }
        else
        {
            return ReferenceExpression.Create($"{Constants.Scheduler.Dashboard.Endpoint.ToString()}");
        }
    }
}
