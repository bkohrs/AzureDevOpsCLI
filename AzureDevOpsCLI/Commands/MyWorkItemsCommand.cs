using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsCLI.Commands
{
    [Command("my-work-items", Description = "Retrieve work items assigned to your user account")]
    public class MyWorkItemsCommand : BaseAzureDevOpsProjectCommand
    {
        protected override async ValueTask InternalExecuteAsync(IConsole console, VssConnection connection)
        {
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
            var wiql = new Wiql
            {
                Query = $@"
SELECT [System.Id]
FROM WorkItems
WHERE [System.TeamProject] = '{Project}'
AND [System.AssignedTo] = @me
AND [System.State] NOT IN ('Closed', 'Completed', 'Done', 'Removed')
"
            };
            var result = await client.QueryByWiqlAsync(wiql).ConfigureAwait(false);
            if (!result.WorkItems.Any())
                return;
            var workItemIDs = result.WorkItems.Select(workItem => workItem.Id);
            var workItems = await client.GetWorkItemsAsync(Project, workItemIDs).ConfigureAwait(false);
            foreach (var workItem in workItems)
            {
                await WorkItemWriter.WriteWorkItem(console, workItem).ConfigureAwait(false);
                await console.Output.WriteLineAsync().ConfigureAwait(false);
            }
        }
    }
}