using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

/// <summary>
/// Represents a Durable Task Scheduler resource.
/// </summary>
/// <param name="name">The name of the resource.</param>
public sealed class DurableTaskSchedulerResource(string name)
    : Resource(name), IResourceWithConnectionString, IResourceWithEndpoints, IResourceWithDashboard
{
    EndpointReference EmulatorDashboardEndpoint => new(this, Constants.Scheduler.Emulator.Endpoints.Dashboard);
    EndpointReference EmulatorSchedulerEndpoint => new(this, Constants.Scheduler.Emulator.Endpoints.Worker);

    /// <summary>
    /// Gets or sets the authentication type used to access the scheduler.
    /// </summary>
    /// <remarks>
    /// The value should be from <see cref="DurableTaskSchedulerAuthentication" />.
    /// The default value is <see cref="DurableTaskSchedulerAuthentication.None" />.
    /// </remarks>
    public string? Authentication { get; set; }

    /// <summary>
    /// Gets or sets the client ID used to access the scheduler, when using managed identity for authentication.
    /// </summary>
    public string? ClientId { get; set; }

    /// <inheritdoc />
    public ReferenceExpression ConnectionStringExpression =>
        this.CreateConnectionString();

    /// <summary>
    /// Gets or sets the endpoint used to access the scheduler's dashboard.
    /// </summary>
    public Uri? DashboardEndpoint { get; set; }

    /// <summary>
    /// Gets a value indicating whether the scheduler is running as a local emulator.
    /// </summary>
    public bool IsEmulator => this.IsContainer();

    /// <summary>
    /// Gets or sets the endpoint used by applications to access the scheduler.
    /// </summary>
    public Uri? SchedulerEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the name of the scheduler (if different from the resource name).
    /// </summary>
    public string? SchedulerName { get; set; }

    ReferenceExpression IResourceWithDashboard.DashboardEndpointExpression =>
        this.CreateDashboardEndpoint();

    internal ReferenceExpression SchedulerEndpointExpression =>
        this.CreateSchedulerEndpoint();

    internal ReferenceExpression? SubscriptionIdExpression =>
        this.ResolveSubscriptionId();

    internal ReferenceExpression SchedulerNameExpression =>
        this.ResolveSchedulerName();

    ReferenceExpression? ResolveSubscriptionId()
    {
        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.SubscriptionId}");
        }
        
        return null;
    }
    
    ReferenceExpression ResolveSchedulerName()
    {
        if (this.TryGetLastAnnotation(out ExistingDurableTaskSchedulerAnnotation? annotation))
        {
            return ReferenceExpression.Create($"{annotation.Name}");
        }

        if (this.SchedulerName is not null)
        {
            return ReferenceExpression.Create($"{this.SchedulerName}");
        }

        return ReferenceExpression.Create($"{this.Name}");
    }

    ReferenceExpression CreateConnectionString(string? applicationName = null)
    {
        string connectionString = $"Authentication={this.Authentication ?? DurableTaskSchedulerAuthentication.None}";
        
        if (this.ClientId is not null)
        {
            connectionString += $";ClientID={this.ClientId}";
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
