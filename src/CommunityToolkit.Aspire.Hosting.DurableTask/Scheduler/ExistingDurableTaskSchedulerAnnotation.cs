using Aspire.Hosting.ApplicationModel;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

sealed class ExistingDurableTaskSchedulerAnnotation(ParameterOrValue connectionString) : IResourceAnnotation
{
    public ParameterOrValue ConnectionString => connectionString;
}
