using QRCoder.Cli.Helper;
using QRCoder.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using Spectre.Console;
using Spectre.Console.Cli;
using Color = SixLabors.ImageSharp.Color;

namespace QRCoder.Cli.Commands;

public class PrintCommand<TPayloadCommandSettings, TPayload> : AsyncCommand<TPayloadCommandSettings> where TPayloadCommandSettings : PrintCommandOptions<TPayload> where TPayload : PayloadGenerator.Payload 
{
    public override async Task<int> ExecuteAsync(CommandContext context, TPayloadCommandSettings settings)
    {
        if(settings.PromptMode)
            settings.PromptForMembers(showCommandLineFlags: true);
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(settings.Payload, settings.QualityLevel.GetValueOrDefault());

        await using var outputStream = settings.UsesStandardOut 
            ? Console.OpenStandardOutput() 
            : File.OpenWrite(settings.OutputPath!);

        var extension = Path.GetExtension(settings.OutputPath);
        IImageFormat imageFormat = extension switch
        {
            ".png" => PngFormat.Instance,
            ".jpg" or "jpeg" => JpegFormat.Instance,
            ".gif" => GifFormat.Instance,
            ".bmp" => BmpFormat.Instance,
            _ => PngFormat.Instance
        };
        
        switch (settings.Type)
        {
            case QRCodeTypes.Ascii:
            {
                using var asciiCode = new AsciiQRCode(qrCodeData);
                await using var textWriter = new StreamWriter(outputStream);
                foreach (var line in asciiCode.GetLineByLineGraphic(repeatPerModule: 1))
                    await textWriter.WriteLineAsync(line);
                break;
            }
            case QRCodeTypes.PNG:
            { 
                using var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeAsPngByteArr = settings.Transparent
                    ? qrCode.GetGraphic(20, new byte[] { 0, 0, 0, byte.MaxValue }, new byte[] { 0, 0, 0, 0 })
                    : qrCode.GetGraphic(20, new byte[] { 0, 0, 0, byte.MaxValue }, new byte[] { byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue });
                await outputStream.WriteAsync(qrCodeAsPngByteArr);
                if (settings.Visualize)
                {
                    var image = new CanvasImage(qrCodeAsPngByteArr);
                    AnsiConsole.Write(image);
                }
                break;
            }
            case QRCodeTypes.SVG:
            { 
                using var qrCode = new SvgQRCode(qrCodeData);
                await using var textWriter = new StreamWriter(outputStream);
                await textWriter.WriteAsync(qrCode.GetGraphic(50));
                break;
            }
            case QRCodeTypes.UniversalImage:
            { 
                using var qrCode = new QRCode(qrCodeData);
                var icon = string.IsNullOrEmpty(settings.LogoPath) ? null : await Image.LoadAsync(settings.LogoPath);
                var darkColor = Color.ParseHex(settings.DarkColor);
                var lightColor = Color.ParseHex(settings.LightColor);
                if (settings.Transparent)
                    lightColor = lightColor.WithAlpha(0);
                using var image = qrCode.GetGraphic(20, darkColor: darkColor, lightColor: lightColor, icon: icon, iconSizePercent: settings.LogoPercentage.GetValueOrDefault(15));
                await image.SaveAsync(outputStream, format: imageFormat);
                break;
            }
            
            default:
                throw new ArgumentOutOfRangeException(nameof(settings.Type));
        }
        
        return 0;
    }
}