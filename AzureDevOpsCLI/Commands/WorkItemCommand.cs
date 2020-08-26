using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsCLI.Commands
{
    [Command("work-item", Description = "Retrieve information about a work item")]
    public class WorkItemCommand : BaseAzureDevOpsProjectCommand
    {
        [CommandOption("id",
            Description = "The id of the work item to retrieve.",
            IsRequired = true)]
        public int ID { get; set; }

        protected override async ValueTask InternalExecuteAsync(IConsole console, VssConnection connection)
        {
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await client.GetWorkItemAsync(Project, ID).ConfigureAwait(false);
            await WorkItemWriter.WriteWorkItem(console, workItem).ConfigureAwait(false);
        }
    }
}