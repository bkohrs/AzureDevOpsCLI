using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsCLI.Commands
{
    [Command("identity", Description = "Retrieve identity information from Azure DevOps.")]
    public class IdentityCommand : ICommand
    {
        [CommandOption("base-url", 'b',
            Description = "The base url of your Azure DevOps organization.",
            IsRequired = true,
            EnvironmentVariableName = "azdo-cli-base-url")]
        public string? BaseUrl { get; set; }

        [CommandOption("personal-access-token", 'p',
            Description = "The personal access token to authenticate to Azure DevOps.",
            IsRequired = true,
            EnvironmentVariableName = "azdo-cli-personal-access-token")]
        public string? PersonalAccessToken { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var baseUrl = new Uri(BaseUrl ?? "");
            var credentials = new VssBasicCredential("", PersonalAccessToken);
            var connection = new VssConnection(baseUrl, credentials);
            var identity = connection.AuthenticatedIdentity;
            await console.Output.WriteLineAsync(
                $"ID: {identity.Id}").ConfigureAwait(false);
            await console.Output.WriteLineAsync(
                $"Name: {identity.DisplayName}").ConfigureAwait(false);
        }
    }
}