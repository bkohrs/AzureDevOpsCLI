using System.Threading.Tasks;
using CliFx;

namespace AzureDevOpsCLI
{
    public static class Program
    {
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .UseExecutableName("AzureDevOpsCLI.exe")
                .Build()
                .RunAsync();
    }
}