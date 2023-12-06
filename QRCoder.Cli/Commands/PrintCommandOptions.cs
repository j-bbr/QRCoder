using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Spectre.Console;
using Spectre.Console.Cli;
using Color = SixLabors.ImageSharp.Color;

namespace QRCoder.Cli.Commands;

public abstract class PrintCommandOptions<TPayload> : CommandSettings where TPayload : PayloadGenerator.Payload
{
    
    [CommandArgument(0, "[OutputFile]")]
    [Description("Output path for the generated QR Code, leave empty for output to StdOut")]
    public string? OutputPath {get; set;}
    
    
    
    [CommandOption("--prompt-mode <PromptMode>")]
    [Description("Whether to ask for all input ")]
    [DefaultValue(false)]
    public bool PromptMode { get; set; }
    
    [CommandOption("--visualize <Visualize>")]
    [Description("Whether to visualize the output on the console ")]
    [DefaultValue(false)]
    public bool Visualize { get; set; }
    
    
    [CommandOption("--transparent <Transparent>")]
    [Description("Whether to have a transparent background for the QR Code ")]
    [DefaultValue(false)]
    public bool Transparent { get; set; }
    

    [CommandOption("-q|--quality-mode <QualityMode>")]
    [Description("QR Code Quality Settings")]
    public QRCodeGenerator.ECCLevel? QualityLevel { get; set; }
    
    [CommandOption("-t|--type <OutputType>")]
    [Description("QR Code Output Type, defaults to PNG")]
    [DefaultValue(QRCodeTypes.UniversalImage)]
    public QRCodeTypes? Type { get; set; }
    
    [CommandOption("--logo-path <LogoPath>")]
    [Description("Logo to interlace with QR Code")]
    public string? LogoPath { get; set; }
    
    [CommandOption("--logo-percentage <LogoPath>")]
    [Description("Logo Percentage")]
    [Range(1, 100)]
    [DefaultValue(15)]
    public int? LogoPercentage { get; set; }
    
    [CommandOption("--light-color <LightColor>")]
    [Description("Color of the bright parts in the QR Code")]
    [DefaultValue("#FFFFFF")]
    public string? LightColor { get; set; }
    
    [CommandOption("--dark-color <DarkColor>")]
    [Description("Color of the dark parts in the QR Code")]
    [DefaultValue("#000000FF")]
    public string? DarkColor { get; set; }
    
    
    public abstract TPayload Payload { get; }
    
    public bool UsesStandardOut => string.IsNullOrEmpty(OutputPath);
}