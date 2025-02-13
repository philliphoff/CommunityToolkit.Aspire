using Microsoft.DurableTask;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler.Worker.Tasks.Echo;

[DurableTask("Echo")]
public class EchoOrchestrator : TaskOrchestrator<EchoInput, string>
{
    public override async Task<string> RunAsync(TaskOrchestrationContext context, EchoInput input)
    {
        string output = await context.CallEchoActivityAsync(input);

        return output;
    }
}
