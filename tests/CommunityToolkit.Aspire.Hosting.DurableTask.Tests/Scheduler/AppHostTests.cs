using Aspire.Components.Common.Tests;
using CommunityToolkit.Aspire.Testing;
using Projects;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CommunityToolkit.Aspire.Hosting.EventStore.Tests;

[RequiresDocker]
public class AppHostTests(AspireIntegrationTestFixture<CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_AppHost> fixture) : IClassFixture<AspireIntegrationTestFixture<CommunityToolkit_Aspire_Hosting_DurableTask_Scheduler_AppHost>>
{
    [Fact]
    public async Task ResourceStartsAndRespondsOk()
    {
        var resourceName = "scheduler";

        await fixture.ResourceNotificationService
            .WaitForResourceHealthyAsync(resourceName)
            .WaitAsync(TimeSpan.FromMinutes(1));

        var webApiResource = "webapi";

        var httpClient = fixture.CreateHttpClient(webApiResource);

        var response = await httpClient.PostAsJsonAsync(
            "/create", new CreateRequest
            {
                Text = "Hello, world!"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    sealed record CreateRequest
    {
        [JsonPropertyName("text")]
        public required string Text { get; init; }
    }
}