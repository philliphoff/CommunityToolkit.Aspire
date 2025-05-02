using Aspire.Hosting;
using Aspire.Hosting.Utils;
using CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Tests.Scheduler;

public class AddDurableTaskSchedulerTests
{
    [Fact]
    public async Task AddDurableTaskScheduler()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder.AddDurableTaskScheduler("scheduler");

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        var scheduler = model.Resources.OfType<DurableTaskSchedulerResource>().Single();

        Assert.Null(scheduler.Authentication);
        Assert.Null(scheduler.ClientId);
        Assert.Null(scheduler.DashboardEndpoint);        
        Assert.False(scheduler.IsEmulator);
        Assert.Null(scheduler.SchedulerEndpoint);
        Assert.Null(scheduler.SchedulerName);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await scheduler.ConnectionStringExpression.GetValueAsync(CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await scheduler.SchedulerEndpointExpression.GetValueAsync(CancellationToken.None));

        Assert.Null(scheduler.SubscriptionIdExpression);
        
        Assert.Equal(Constants.Scheduler.Dashboard.Endpoint.ToString(), await (scheduler as IResourceWithDashboard).DashboardEndpointExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("scheduler", await scheduler.SchedulerNameExpression.GetValueAsync(CancellationToken.None));
    }

    [Fact]
    public async Task AddDurableTaskSchedulerWithConfiguration()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder.AddDurableTaskScheduler(
            "scheduler",
            options =>
            {
                options.Resource.Authentication = "TestAuthentication";
                options.Resource.ClientId = "TestClientId";
                options.Resource.DashboardEndpoint = new Uri("https://dashboard.test.io");
                options.Resource.SchedulerEndpoint = new Uri("https://scheduler.test.io");
                options.Resource.SchedulerName = "TestSchedulerName";                
            });

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        var scheduler = model.Resources.OfType<DurableTaskSchedulerResource>().Single();

        Assert.Equal("TestAuthentication", scheduler.Authentication);
        Assert.Equal("TestClientId", scheduler.ClientId);
        Assert.Equal(new Uri("https://dashboard.test.io"), scheduler.DashboardEndpoint);        
        Assert.False(scheduler.IsEmulator);
        Assert.Equal(new Uri("https://scheduler.test.io"), scheduler.SchedulerEndpoint);
        Assert.Equal("TestSchedulerName", scheduler.SchedulerName);

        Assert.Equal("Endpoint=https://scheduler.test.io/;Authentication=TestAuthentication;ClientID=TestClientId", await scheduler.ConnectionStringExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("https://scheduler.test.io/", await scheduler.SchedulerEndpointExpression.GetValueAsync(CancellationToken.None));

        Assert.Null(scheduler.SubscriptionIdExpression);
        
        Assert.Equal("https://dashboard.test.io/".ToString(), await (scheduler as IResourceWithDashboard).DashboardEndpointExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("TestSchedulerName", await scheduler.SchedulerNameExpression.GetValueAsync(CancellationToken.None));
    }

    [Fact]
    public async Task AddDurableTaskSchedulerAsExisting()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder
            .AddDurableTaskScheduler("scheduler")
            .RunAsExisting(
                "TestSchedulerName",
                "TestSubscriptionId",
                new Uri("https://scheduler.test.io"),
                new Uri("https://dashboard.test.io"));

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        var scheduler = model.Resources.OfType<DurableTaskSchedulerResource>().Single();

        Assert.Equal("DefaultAzure", scheduler.Authentication);
        Assert.Null(scheduler.ClientId);
        Assert.Null(scheduler.DashboardEndpoint);        
        Assert.False(scheduler.IsEmulator);
        Assert.Null(scheduler.SchedulerEndpoint);
        Assert.Null(scheduler.SchedulerName);

        Assert.Equal("Endpoint=https://scheduler.test.io/;Authentication=DefaultAzure", await scheduler.ConnectionStringExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("https://scheduler.test.io/", await scheduler.SchedulerEndpointExpression.GetValueAsync(CancellationToken.None));

        Assert.NotNull(scheduler.SubscriptionIdExpression);
        Assert.Equal("TestSubscriptionId", await scheduler.SubscriptionIdExpression.GetValueAsync(CancellationToken.None));
        
        Assert.Equal("https://dashboard.test.io/".ToString(), await (scheduler as IResourceWithDashboard).DashboardEndpointExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("TestSchedulerName", await scheduler.SchedulerNameExpression.GetValueAsync(CancellationToken.None));
    }

    [Fact]
    public async Task AddDurableTaskSchedulerAsEmulator()
    {
        using var builder = TestDistributedApplicationBuilder.Create();

        builder
            .AddDurableTaskScheduler("scheduler")
            .RunAsEmulator();

        using var app = builder.Build();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        var scheduler = model.Resources.OfType<DurableTaskSchedulerResource>().Single();

        Assert.Equal("None", scheduler.Authentication);
        Assert.Null(scheduler.ClientId);
        Assert.Null(scheduler.DashboardEndpoint);        
        Assert.True(scheduler.IsEmulator);
        Assert.Null(scheduler.SchedulerEndpoint);
        Assert.Null(scheduler.SchedulerName);

        Assert.Equal("Endpoint=https://scheduler.test.io/;Authentication=None", await scheduler.ConnectionStringExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("https://scheduler.test.io/", await scheduler.SchedulerEndpointExpression.GetValueAsync(CancellationToken.None));

        Assert.Null(scheduler.SubscriptionIdExpression);
        
        Assert.Equal("https://dashboard.test.io/".ToString(), await (scheduler as IResourceWithDashboard).DashboardEndpointExpression.GetValueAsync(CancellationToken.None));
        Assert.Equal("TestSchedulerName", await scheduler.SchedulerNameExpression.GetValueAsync(CancellationToken.None));
    }
}