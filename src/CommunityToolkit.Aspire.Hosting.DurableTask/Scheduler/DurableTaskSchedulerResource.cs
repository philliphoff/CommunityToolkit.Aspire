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
    private EndpointReference EmulatorSchedulerEndpoint => new(this, "grpc");

    /// <summary>
    /// 
    /// </summary>
    public string? Authentication { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
        this.CreateConnectionString();

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
        if (this.IsEmulator)
        {
            return ReferenceExpression.Create($"Endpoint={this.EmulatorSchedulerEndpoint};Authentication={this.Authentication}");
        }
        else
        {
            return ReferenceExpression.Create($"Endpoint={this.SchedulerEndpoint.ToString()};Authentication={this.Authentication}");
        }
    }

    private string CreateConnectionString(IEnumerable<KeyValuePair<string, string>> properties)
    {
        return String.Join(';', properties.Select(property => $"{property.Key}={property.Value}"));
    }
}
