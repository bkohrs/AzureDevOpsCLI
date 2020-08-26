using System.Threading.Tasks;
using CliFx;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsCLI
{
    public static class WorkItemWriter
    {
        private static string? FieldOutput(object workItemField)
        {
            return workItemField switch
            {
                IdentityRef r => r.DisplayName,
                _ => workItemField?.ToString()
            };
        }

        public static async ValueTask WriteWorkItem(IConsole console, WorkItem workItem)
        {
            await console.Output.WriteLineAsync($"ID: {workItem.Id}").ConfigureAwait(false);
            foreach (var field in workItem.Fields.Keys)
            {
                var fieldOutput = FieldOutput(workItem.Fields[field]);
                await console.Output.WriteLineAsync($"{field}: {fieldOutput}").ConfigureAwait(false);
            }
        }
    }
}