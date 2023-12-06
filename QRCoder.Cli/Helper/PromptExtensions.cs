using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace QRCoder.Cli.Helper;

public static class PromptExtensions
{
    public static void PromptForMembers<T>(this T settings, bool showCommandLineFlags = false) where T : CommandSettings
    {
        foreach (var property in settings.GetType().GetProperties().Where(prop => prop.SetMethod is not null)) 
            ProcessProperty(property, settings, false, showCommandLineFlags);
    }
    
    public static TSettings PromptFor<TSettings, TProperty>(this TSettings settings, Expression<Func<TSettings, TProperty>> propertyGetter) where TSettings : CommandSettings
    {
        if (propertyGetter.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression '{propertyGetter}' refers to a method, not a property.");
        }

        if (member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException($"Expression '{propertyGetter}' refers to a field, not a property.");
        }
        ProcessProperty(propInfo, settings, false);
        return settings;
    }
    
    public static void ProcessProperty(PropertyInfo property,  object settingsInstance, bool attributeRequired = true, bool showCommandLineFlags = false)
    {
        var promptAttribute = property.GetCustomAttribute<PromptOptionAttribute>();
        if(promptAttribute is null && attributeRequired)
            return;
        var optionValue = property.GetValue(settingsInstance);
        //Prompt for anything other than default values 
        //preferable to check  
        if(!Equals(optionValue, promptAttribute?.PromptIfOptionEqualsThis ?? GetDefault(property.PropertyType)))
            return;
        var name = property.Name;
        var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description;
        var optionAttribute = property.GetCustomAttribute<CommandOptionAttribute>();
        var commandLineHint = showCommandLineFlags && optionAttribute is not null
            ? $"CLI Option: {PrintCliOptions(optionAttribute)}"
            : string.Empty;
        var promptText = string.IsNullOrEmpty(description) ? $"{name} ({commandLineHint})?" : $"{name}? ({description} | {commandLineHint})";
        var required = property.GetCustomAttribute<RequiredAttribute>() is not null;
        var value = PromptForValue(property.PropertyType, promptAttribute?.Options, promptText, required);
        var validationResults = new List<ValidationResult>();

        var valueWasProvided = value is not null && value is not string { Length: 0 };
        
        if (valueWasProvided || required)
        {
            while (!Validator.TryValidateValue(value, new ValidationContext(settingsInstance), validationResults,
                       property.GetCustomAttributes<ValidationAttribute>()))
            {
                AnsiConsole.MarkupLine("The provided value is invalid because:");
                foreach(var failedResult in validationResults)
                    AnsiConsole.MarkupLine($"[red]{failedResult.ErrorMessage}[/]");
                value = PromptForValue(property.PropertyType, promptAttribute?.Options, promptText, required);
                validationResults.Clear();
            }
        }
        
        property.SetValue(settingsInstance, value);

        string PrintCliOptions(CommandOptionAttribute cliOptionAttribute)
        {
            var options = cliOptionAttribute.LongNames.Select(longName => $"--{longName}").ToList();
            options.AddRange(cliOptionAttribute.ShortNames.Select(shortName => $"-{shortName}"));
            return string.Join(", ", options);
        }
    }

    private static object? PromptForValue(Type optionType, object[]? options, string promptText, bool required)
    {
        // We need to check whether the property is NULLABLE
        if (optionType.IsGenericType && optionType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
            optionType = optionType.GetGenericArguments()[0];
        }
        return optionType switch
        {
            _ when optionType == typeof(bool) || optionType == typeof(bool?) => AnsiConsole.Confirm(promptText),
            _ when optionType.IsEnum => optionType.GetCustomAttribute<FlagsAttribute>() is null 
                ? ShowSimpleEnumPrompt() : ShowFlagEnumPrompt(),
            _ when options is {Length: > 0} => ShowSelectionPrompt(),
            _ => ShowTextPrompt()
        };

        object ShowSelectionPrompt()
        {
            var promptType = typeof(SelectionPrompt<>).MakeGenericType(optionType);
            var promptInstance = Activator.CreateInstance(promptType);
            promptType.GetProperty(nameof(SelectionPrompt<string>.Title))!.SetValue(promptInstance, promptText);
            var setChoiceMethod = promptType.GetMethod(nameof(SelectionPrompt<string>.AddChoice))!;
            foreach (var option in options)
                setChoiceMethod.Invoke(promptInstance, new[] { option });

            var promptedValue = typeof(AnsiConsole)
                .GetMethod(nameof(AnsiConsole.Prompt))!
                .MakeGenericMethod(optionType)
                .Invoke(null, new[] { promptInstance });
            return promptedValue;
        }
        
        object ShowSimpleEnumPrompt()
        {
            var optionNames = Enum.GetNames(optionType);
            var prompt = new SelectionPrompt<string>()
                .Title(promptText)
                .AddChoices(optionNames);
            
            var choice = AnsiConsole.Prompt(prompt);
            var result = Enum.Parse(optionType, choice);
            return result;
        }
        
        object ShowFlagEnumPrompt()
        {
            var optionNames = Enum.GetNames(optionType);
            var choices = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                .NotRequired()
                .Title(promptText)
                .AddChoices(optionNames));
            var values = choices.Select(c => Enum.Parse(optionType, c));
            var numericType = Enum.GetUnderlyingType(optionType);
            object aggregateResult = numericType switch
            {
                _ when numericType == typeof(sbyte) => values.Cast<sbyte>().Aggregate(0, (flag, aggregate) => flag | (byte)aggregate),
                _ when numericType == typeof(short) => values.Cast<short>().Aggregate(0, (flag, aggregate) => flag | (ushort)aggregate),
                _ when numericType == typeof(int) => values.Cast<int>().Aggregate(0, (flag, aggregate) => flag | aggregate),
                _ when numericType == typeof(long) => values.Cast<long>().Aggregate(0L, (flag, aggregate) => flag | aggregate),
                _ when numericType == typeof(byte) => values.Cast<byte>().Aggregate(0u, (flag, aggregate) => flag | aggregate),
                _ when numericType == typeof(ushort) => values.Cast<ushort>().Aggregate(0u, (flag, aggregate) => flag | aggregate),
                _ when numericType == typeof(uint) => values.Cast<uint>().Aggregate(0u, (flag, aggregate) => flag | aggregate),
                _ when numericType == typeof(ulong) => values.Cast<ulong>().Aggregate(0ul, (flag, aggregate) => flag | aggregate),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Convert.ChangeType(aggregateResult, numericType);
        }
        
        object? ShowTextPrompt()
        {
            var promptType = typeof(TextPrompt<>).MakeGenericType(optionType);
            var promptInstance = Activator.CreateInstance(promptType, args: new object []{promptText, null});
            promptType.GetProperty(nameof(TextPrompt<string>.AllowEmpty))!.SetValue(promptInstance, !required);
            var promptedValue = typeof(AnsiConsole)
                .GetMethod(nameof(AnsiConsole.Prompt))!
                .MakeGenericMethod(optionType)
                .Invoke(null, new[] { promptInstance });
            return promptedValue;
            
        }
    }
    
    
    private static object? GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
}