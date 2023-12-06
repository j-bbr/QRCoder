namespace QRCoder.Cli.Helper;

public class PromptOptionAttribute : Attribute
{
    public object? PromptIfOptionEqualsThis { get; set; }
    
    public object[] Options { get; set; }

    public PromptOptionAttribute(params object[] options)
    {
        Options = options;
    }
}