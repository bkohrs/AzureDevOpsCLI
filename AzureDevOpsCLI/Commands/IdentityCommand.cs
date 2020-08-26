using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsCLI.Commands
{
    [Command("identity", Description = "Retrieve identity information from Azure DevOps.")]
    public class IdentityCommand : BaseAzureDevOpsCommand
    {
        protected override async ValueTask InternalExecuteAsync(IConsole console, VssConnection connection)
        {
            var identity = connection.AuthenticatedIdentity;
            await console.Output.WriteLineAsync(
                $"ID: {identity.Id}").ConfigureAwait(false);
            await console.Output.WriteLineAsync(
                $"Name: {identity.DisplayName}").ConfigureAwait(false);
        }
    }
}