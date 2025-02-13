using Aspire.Hosting;
using Aspire.Hosting.Utils;
using CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Tests.Scheduler;

public class AddDurableTaskSchedulerTests
{
    [Fact]
    public void AddDurableTaskScheduler()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder.AddDurableTaskScheduler("scheduler");

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        Assert.Contains(
            model.Resources,
            r => r is DurableTaskSchedulerResource);
    }
}