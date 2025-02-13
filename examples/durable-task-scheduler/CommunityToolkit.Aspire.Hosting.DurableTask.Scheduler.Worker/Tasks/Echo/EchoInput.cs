using System.Text.Json.Serialization;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler.Worker.Tasks.Echo;

public record EchoInput
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}
