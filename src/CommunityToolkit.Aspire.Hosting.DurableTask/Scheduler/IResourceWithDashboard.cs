using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

internal interface IResourceWithDashboard : IResource
{
    ReferenceExpression DashboardEndpoint { get; }
}
