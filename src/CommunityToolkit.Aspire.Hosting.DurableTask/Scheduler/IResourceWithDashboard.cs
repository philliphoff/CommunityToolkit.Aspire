using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

interface IResourceWithDashboard : IResource
{
    ReferenceExpression DashboardEndpointExpression { get; }

    bool IsTaskHub => false;
}
