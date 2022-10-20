// See https://aka.ms/new-console-template for more information
using CliFx;
//3 commands 

public class DemoCli
{
    public DemoCli()
    {
        CliApplicationBuilder = new CliApplicationBuilder().AddCommandsFromThisAssembly();
    }

    public CliApplicationBuilder CliApplicationBuilder { get; }

    public ValueTask<int> RunAsync(params string[] args)
    {
        return CliApplicationBuilder
            .Build()
            .RunAsync(args);
    }
}