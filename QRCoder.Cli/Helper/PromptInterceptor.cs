using Spectre.Console.Cli;

namespace QRCoder.Cli.Helper;

public class PromptInterceptor : ICommandInterceptor
{

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        foreach (var property in settings.GetType().GetProperties()) 
            PromptExtensions.ProcessProperty(property, settings);
    }

    
}