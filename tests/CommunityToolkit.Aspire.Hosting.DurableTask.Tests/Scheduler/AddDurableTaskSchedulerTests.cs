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

    const string TestAuthentication = "TestAuthentication";
    const string TestClientId = "TestClientId";
    static readonly Uri TestDashboardEndpoint = new Uri("https://dashboard.test.io");
    static readonly Uri TestSchedulerEndpoint = new Uri("https://scheduler.test.io");
    const string TestSchedulerName = "TestSchedulerName";

    [Fact]
    public async Task AddDurableTaskSchedulerWithOptions()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder.AddDurableTaskScheduler(
            "scheduler",
            options =>
            {
                options.Resource.Authentication = TestAuthentication;
                options.Resource.ClientId = TestClientId;
                options.Resource.DashboardEndpoint = TestDashboardEndpoint;
                options.Resource.SchedulerEndpoint = TestSchedulerEndpoint;
                options.Resource.SchedulerName = TestSchedulerName;
            });

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        var resource = model.Resources.Single();

        Assert.NotNull(resource);

        Assert.IsType<DurableTaskSchedulerResource>(resource);

        var scheduler = (DurableTaskSchedulerResource)resource;

        Assert.False(scheduler.IsEmulator);

        Assert.Equal(TestAuthentication, scheduler.Authentication);
        Assert.Equal(TestClientId, scheduler.ClientId);
        Assert.Equal(TestDashboardEndpoint, scheduler.DashboardEndpoint);
        Assert.Equal(TestSchedulerEndpoint, scheduler.SchedulerEndpoint);
        Assert.Equal(TestSchedulerName, scheduler.SchedulerName);

        Assert.NotNull(scheduler.SubscriptionIdExpression);
        Assert.Equal("default", await scheduler.SubscriptionIdExpression.GetValueAsync(CancellationToken.None));
    }

    [Fact]
    public async Task AddDurableTaskSchedulerWithRunExisting()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder
            .AddDurableTaskScheduler(
            "scheduler")
            .RunAsExisting(
                "scheduler-name",
                "subscription-id",
                "scheduler-endpoint",
                "dashboard-endpoint");

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        var resource = model.Resources.Single();

        Assert.NotNull(resource);

        Assert.IsType<DurableTaskSchedulerResource>(resource);

        var scheduler = (DurableTaskSchedulerResource)resource;

        Assert.False(scheduler.IsEmulator);

        Assert.NotNull(scheduler.SubscriptionIdExpression);
        Assert.Equal("subscription-id", await scheduler.SubscriptionIdExpression.GetValueAsync(CancellationToken.None));
    }
}