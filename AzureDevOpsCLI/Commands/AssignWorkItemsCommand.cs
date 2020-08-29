using System;
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
    [Command("assign-work-items", Description = "Assigns one or more work items to the current user")]
    public class AssignWorkItemsCommand : BaseAzureDevOpsCommand
    {
        [CommandOption("ids",
            Description = "The ids of the work items to assign.",
            IsRequired = true)]
        public IEnumerable<int>? IDs { get; set; }

        protected override async ValueTask InternalExecuteAsync(IConsole console, VssConnection connection)
        {
            if (IDs == null || !IDs.Any())
                return;
            var client = connection.GetClient<WorkItemTrackingHttpClient>();
            var requests = IDs.Select(id =>
            {
                var patch = new JsonPatchDocument
                {
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.AssignedTo",
                        Value = connection.AuthenticatedIdentity.DisplayName,
                    }
                };
                return new WitBatchRequest
                {
                    Uri = $"/_apis/wit/workItems/{id}?api-version=4.1",
                    Method = "PATCH",
                    Headers = new Dictionary<string, string> {{"Content-Type", "application/json-patch+json"}},
                    Body = JsonConvert.SerializeObject(patch)
                };
            });
            await client.ExecuteBatchRequest(requests).ConfigureAwait(false);
        }
    }
}