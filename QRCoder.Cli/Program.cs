using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QRCoder;
using QRCoder.Cli.Commands;
using QRCoder.Cli.Helper;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

// var config = new ConfigurationBuilder()
//     .AddEnvironmentVariables()
//     .AddCommandLine(args)
//     .Build();

var services = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Error));

using var registrar = new DependencyInjectionRegistrar(services);
try
{
    var app = new CommandApp(registrar);
    app.Configure(appConfig =>
    {
        appConfig.SetApplicationName("QR Code Generator"); ;
        appConfig.AddBranch("generate", generate =>
        {
            generate.SetDescription("Print a QR code with the given data");
            generate.AddCommand<PrintCommand<PayloadCommandOptions, PayloadGenerator.PlainText>>("plaintext");
            generate.AddCommand<PrintCommand<GeolocationCommandOptions, PayloadGenerator.Geolocation>>("geolocation");
            generate.AddCommand<PrintCommand<UrlCommandOptions, PayloadGenerator.Url>>("url");
            generate.AddCommand<PrintCommand<MailCommandOptions, PayloadGenerator.Mail>>("mail");
            generate.AddCommand<PrintCommand<SMSCommandOptions, PayloadGenerator.SMS>>("sms");
            generate.AddCommand<PrintCommand<WhatsAppCommandOptions, PayloadGenerator.WhatsAppMessage>>("whatsapp");
            generate.AddCommand<PrintCommand<ContactCommandOptions, PayloadGenerator.ContactData>>("contact");
            generate.AddCommand<PrintCommand<WifiCommandOptions, PayloadGenerator.WiFi>>("wifi");
            generate.AddCommand<PrintCommand<BookmarkCommandOptions, PayloadGenerator.Bookmark>>("bookmark");
            generate.AddCommand<PrintCommand<CalendarCommandOptions, PayloadGenerator.CalendarEvent>>("calendar");
            generate.AddCommand<PrintCommand<PhoneCommandOptions, PayloadGenerator.PhoneNumber>>("phone");
        });

    });
    return await app.RunAsync(args);
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
    return 1;
}