using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace QRCoder.Cli.Commands;

public class PayloadCommandOptions : PrintCommandOptions<PayloadGenerator.PlainText>
{
    [CommandOption("--value <Value>")]
    [Description("Plain Text Value to encode")]
    public string? Plaintext { get; set; }

    public override PayloadGenerator.PlainText Payload => new(Plaintext);
    
}

public class GeolocationCommandOptions : PrintCommandOptions<PayloadGenerator.Geolocation>
{
    [CommandOption("--latitude <Latitude>")]
    [Description("Plain Text Value to encode")]
    public double? Longitude { get; set; }
    
    [CommandOption("--longitude <Longitude>")]
    [Description("Plain Text Value to encode")]
    public double? Latitude { get; set; }
    
    [CommandOption("--encoding <Encoding>")]
    [Description("Location QR Code Type")]
    [DefaultValue(PayloadGenerator.Geolocation.GeolocationEncoding.GEO)]
    public PayloadGenerator.Geolocation.GeolocationEncoding? Encoding { get; set; }

    public override PayloadGenerator.Geolocation Payload => new(longitude: Longitude.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), latitude: Latitude.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), encoding: Encoding.GetValueOrDefault());
    
}

public class UrlCommandOptions : PrintCommandOptions<PayloadGenerator.Url>
{
    [CommandOption("--value <Value>")]
    [Description("URL value to encode")]
    [Url]
    public string? Url { get; set; }

    public override PayloadGenerator.Url Payload => new(Url);
}


public class BookmarkCommandOptions : PrintCommandOptions<PayloadGenerator.Bookmark>
{
    [CommandOption("--value <Value>")]
    [Description("URL value to encode")]
    [Url]
    public string? Url { get; set; }
    
    [CommandOption("--title <Title>")]
    [Description("Title value to encode")]
    public string? Title { get; set; }

    public override PayloadGenerator.Bookmark Payload => new(Url, Title);
}

public class ContactCommandOptions : PrintCommandOptions<PayloadGenerator.ContactData>
{
    [CommandOption("--first-name <FirstName>")]
    public string? FirstName { get; set; }
    
    [CommandOption("--last-name <LastName>")]
    public string? LastName { get; set; }
    
    [CommandOption("--nick-name <NickName>")]
    public string? NickName { get; set; }
    
    [CommandOption("--phone <Phone>")]
    [Description("Phone number to encode")]
    [Phone]
    public string? Number { get; set; }
    
    [CommandOption("--work-phone <WorkPhone>")]
    [Description("Work number to encode")]
    [Phone]
    public string? WorkNumber { get; set; }
    
    [CommandOption("--mobile-phone <MobilePhone>")]
    [Description("Mobile phone number to encode")]
    [Phone]
    public string? MobileNumber { get; set; }
    
    [CommandOption("--mail <Mail>")]
    [Description("Email Address")]
    [EmailAddress]
    public string? MailAddress { get; set; }
    
    [CommandOption("--website <Website>")]
    [Description("Website Url")]
    [Url]
    public string? Website { get; set; }
    
    [CommandOption("--street <Street>")]
    public string? Street { get; set; }
    
    [CommandOption("--house-number <HouseNumber>")]
    public string? HouseNumber { get; set; }
    
    [CommandOption("--city <City>")]
    public string? City { get; set; }
    
    [CommandOption("--zip-code <ZipCode>")]
    public string? Zipcode { get; set; }
    
    [CommandOption("--country <Country>")]
    public string? Country { get; set; }
    
    [CommandOption("--note <Note>")]
    public string? Note { get; set; }
    [CommandOption("--org <Organization>")]
    public string? Organization { get; set; }
    
    [CommandOption("--org-title <OrganizationTitle>")]
    public string? OrganizationTitle { get; set; }
    
    [CommandOption("--region <Region>")]
    public string? StateRegion { get; set; }
    
    [CommandOption("--birthday <Birthday>")]
    [Description("Birthday of the Contact")]
    public DateTime? Birthday { get; set; }
    
    
    [CommandOption("--contact-type <ContactType>")]
    [Description("Contact Type")]
    [DefaultValue(PayloadGenerator.ContactData.ContactOutputType.VCard3)]
    public PayloadGenerator.ContactData.ContactOutputType? ContactType { get; set; }
    
    [CommandOption("--address-order <AddressOrder>")]
    [Description("Address Order")]
    [DefaultValue(PayloadGenerator.ContactData.AddressOrder.Default)]
    public PayloadGenerator.ContactData.AddressOrder? AddressOrder { get; set; }
    

    public override PayloadGenerator.ContactData Payload => new(outputType: ContactType.GetValueOrDefault(PayloadGenerator.ContactData.ContactOutputType.VCard3), 
        firstname: FirstName, lastname: LastName, nickname: NickName, phone: Number, mobilePhone: MobileNumber, workPhone: WorkNumber, email: MailAddress, 
        birthday: Birthday, website: Website, street: Street, houseNumber: HouseNumber, city: City, zipCode: Zipcode, country: Country, note: Note, stateRegion: StateRegion, 
        addressOrder: AddressOrder.GetValueOrDefault(PayloadGenerator.ContactData.AddressOrder.Default), org: Organization, orgTitle: OrganizationTitle);
}

public class CalendarCommandOptions : PrintCommandOptions<PayloadGenerator.CalendarEvent>
{
    [CommandOption("--subject <Subject>")]
    [Description("Subject of the Event")]
    public string? Subject { get; set; }
    
    [CommandOption("--description <Description>")]
    [Description("Description of the Event")]
    public string? Description { get; set; }
    
    
    [CommandOption("--location <Location>")]
    [Description("Location of the Event, either as address or lat:long")]
    public string? Location { get; set; }
    
    
    [CommandOption("--start <Start>")]
    [Description("Start Date of the Event")]
    public DateTime? Start { get; set; }
    
    [CommandOption("--end <End>")]
    [Description("End Date of the Event")]
    public DateTime? End { get; set; }
    
    [CommandOption("--all-day <AllDay>")]
    [Description("Whether it is an all day event")]
    public bool? AllDay { get; set; }
    
    [CommandOption("--encoding <Encoding>")]
    [Description("Calendar Event Encoding")]
    public PayloadGenerator.CalendarEvent.EventEncoding? Encoding { get; set; }
    

    public override PayloadGenerator.CalendarEvent Payload => new(Subject, Description, Location, Start.GetValueOrDefault(), End.GetValueOrDefault(), AllDay.GetValueOrDefault(), 
        Encoding.GetValueOrDefault(PayloadGenerator.CalendarEvent.EventEncoding.Universal));
}


public class PhoneCommandOptions : PrintCommandOptions<PayloadGenerator.PhoneNumber>
{
    [CommandOption("--value <Value>")]
    [Description("Phone number to encode")]
    [Phone]
    public string? Number { get; set; }

    public override PayloadGenerator.PhoneNumber Payload => new(Number);
}

public class MailCommandOptions : PrintCommandOptions<PayloadGenerator.Mail>
{
    [CommandOption("--recipient <Recipient>")]
    [Description("Mail address of the recipient")]
    [EmailAddress]
    public string? Receiver { get; set; }
    
    [CommandOption("--subject <Subject>")]
    [Description("Mail subject")]
    public string? Subject { get; set; }
    
    [CommandOption("--message <Message>")]
    [Description("Mail message")]
    public string? Message { get; set; }
    
    [CommandOption("--encoding <Encoding>")]
    [Description("Mail Encoding")]
    [DefaultValue(PayloadGenerator.Mail.MailEncoding.MAILTO)]
    public PayloadGenerator.Mail.MailEncoding? Encoding { get; set; }

    public override PayloadGenerator.Mail Payload => new(Receiver, Subject, Message, Encoding.GetValueOrDefault(PayloadGenerator.Mail.MailEncoding.MAILTO));
}

public class SMSCommandOptions : PrintCommandOptions<PayloadGenerator.SMS>
{
    [CommandOption("--recipient <Recipient>")]
    [Description("Phone Number of the recipient")]
    [Phone]
    public string? Receiver { get; set; }
    
    [CommandOption("--subject <Subject>")]
    [Description("SMS subject")]
    public string? Subject { get; set; }
    
    
    [CommandOption("--encoding <Encoding>")]
    [Description("SMS Encoding")]
    [DefaultValue(PayloadGenerator.SMS.SMSEncoding.SMS)]
    public PayloadGenerator.SMS.SMSEncoding? Encoding { get; set; }

    public override PayloadGenerator.SMS Payload => new(Receiver, Subject, Encoding.GetValueOrDefault(PayloadGenerator.SMS.SMSEncoding.SMS));
}

public class WhatsAppCommandOptions : PrintCommandOptions<PayloadGenerator.WhatsAppMessage>
{
    [CommandOption("--message <Message>")]
    [Description("WhatsApp Message")]
    public string? Message { get; set; }

    public override PayloadGenerator.WhatsAppMessage Payload => new(Message);
}

public class WifiCommandOptions : PrintCommandOptions<PayloadGenerator.WiFi>
{
    [CommandOption("--ssid <SSID>")]
    [Description("SSID of the network")]
    public string? SSID { get; set; }
    
    [CommandOption("--password <Password>")]
    [Description("Wifi Password")]
    [PasswordPropertyText]
    public string? Password { get; set; }
    
    [CommandOption("--escape-hex <EscapeHex>")]
    [DefaultValue(true)]
    public bool? EscapeHexStrings { get; set; }
    
    [CommandOption("--hidden-ssid <HiddenSSID>")]
    [DefaultValue(false)]
    public bool? IsHiddenSSID { get; set; }
    
    
    [CommandOption("--mode <AuthenticationMode>")]
    [Description("Wifi Authentication Mode")]
    [DefaultValue(PayloadGenerator.WiFi.Authentication.WPA)]
    public PayloadGenerator.WiFi.Authentication? Authentication { get; set; }

    public override PayloadGenerator.WiFi Payload => new(SSID, Password, Authentication.GetValueOrDefault(PayloadGenerator.WiFi.Authentication.WPA));
}