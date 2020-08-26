using CliFx.Attributes;

namespace AzureDevOpsCLI.Commands
{
    public abstract class BaseAzureDevOpsProjectCommand : BaseAzureDevOpsCommand
    {
        [CommandOption("project", 'p',
            Description = "The name of the Azure DevOps project",
            IsRequired = true)]
        public string? Project { get; set; }
    }
}