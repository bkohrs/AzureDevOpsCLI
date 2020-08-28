using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AzureDevOpsCLI.Commands
{
    [Command("update-remaining-work", Description = "Update the remaining work for a task")]
    public class UpdateRemainingWorkCommand : BaseAzureDevOpsProjectCommand
    {
        [CommandOption("id",
            Description = "The id of the work item to update.",
            IsRequired = true)]
        public int ID { get; set; }
        [CommandOption("completed-work",
            Description = "The amount of hours completed.",
            IsRequired = true)]
        public int CompletedWork { get; set; }

        protected override async ValueTask InternalExecuteAsync(IConsole console, VssConnection connection)
        {
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await client.GetWorkItemAsync(Project, ID).ConfigureAwait(false);
            var remainingWork = Convert.ToInt32(workItem.Fields["Microsoft.VSTS.Scheduling.RemainingWork"]);
            await client.UpdateWorkItemAsync(new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork",
                    Value = remainingWork - CompletedWork,
                }
            }, ID).ConfigureAwait(false);
        }
    }
}