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
    private EndpointReference EmulatorEndpoint => new(this, "http2");

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
    public Uri Endpoint { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public bool IsEmulator => this.IsContainer();

    /// <summary>
    /// 
    /// </summary>
    public string? TaskHubName { get; set; }

    private ReferenceExpression CreateConnectionString(string? applicationName = null)
    {
        if (this.IsEmulator)
        {
            return this.CreateEmulatorConnectionString(applicationName);
        }
        else
        {
            IEnumerable<KeyValuePair<string, string>> properties =
            [
                new("Endpoint", this.Endpoint.ToString())
            ];

            if (this.Authentication is not null)
            {
                properties = properties.Append(new("Authentication", this.Authentication));
            }

            if (this.TaskHubName is not null)
            {
                properties = properties.Append(new("TaskHub", this.TaskHubName));
            }

            return ReferenceExpression.Create($"{this.CreateConnectionString(properties)}");
        }
    }

    private ReferenceExpression CreateEmulatorConnectionString(string? applicationName)
    {
        IEnumerable<KeyValuePair<string, string>> taskHubProperties =
        [
            new("Endpoint", $"http://host.docker.internal:{Endpoint.Port}")
        ];

        if (this.Authentication is not null)
        {
            taskHubProperties = taskHubProperties.Append(new("Authentication", this.Authentication));
        }

        if (this.TaskHubName is not null)
        {
            taskHubProperties = taskHubProperties.Append(new("TaskHub", this.TaskHubName));
        }

        return ReferenceExpression.Create($"{this.CreateConnectionString(taskHubProperties)}");
    }

    private string CreateConnectionString(IEnumerable<KeyValuePair<string, string>> properties)
    {
        return String.Join(';', properties.Select(property => $"{property.Key}={property.Value}"));
    }
}
