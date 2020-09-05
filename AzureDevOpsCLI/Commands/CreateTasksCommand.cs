using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;

namespace AzureDevOpsCLI.Commands
{
    [Command("create-tasks", Description = "Creates one or more tasks for a work item")]
    public class CreateTasksCommand : BaseAzureDevOpsProjectCommand
    {
        [CommandOption("parent",
            Description = "The id of the work item that is the parent of the new tasks",
            IsRequired = true)]
        public int ParentWorkItemID { get; set; }

        [CommandOption("task-names",
            Description = "The names of the tasks to create",
            IsRequired = true)]
        public IEnumerable<string>? TaskNames { get; set; }

        protected override async ValueTask InternalExecuteAsync(IConsole console, VssConnection connection)
        {
            if (TaskNames == null || !TaskNames.Any())
                return;
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
            var requests = TaskNames.Select((taskName, index) =>
            {
                var patch = new JsonPatchDocument
                {
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/id",
                        Value = -index - 1,
                    },
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Title",
                        Value = taskName,
                    },
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/relations/-",
                        Value = new WorkItemRelation
                        {
                            Rel = "System.LinkTypes.Hierarchy-Reverse",
                            Url = $"{BaseUrl}/_apis/wit/workItems/{ParentWorkItemID}"
                        }
                    }
                };
                return new WitBatchRequest
                {
                    Uri = $"/{Project}/_apis/wit/workItems/$Task?api-version=4.1",
                    Method = "PATCH",
                    Headers = new Dictionary<string, string> {{"Content-Type", "application/json-patch+json"}},
                    Body = JsonConvert.SerializeObject(patch)
                };
            });
            var responses = await client.ExecuteBatchRequest(requests).ConfigureAwait(false);
            foreach (var response in responses)
            {
                var workItem = JsonConvert.DeserializeObject<WorkItem>(response.Body);
                await console.Output.WriteLineAsync($"{workItem.Id}: {workItem.Fields["System.Title"]}").ConfigureAwait(false);
            }
        }
    }
}