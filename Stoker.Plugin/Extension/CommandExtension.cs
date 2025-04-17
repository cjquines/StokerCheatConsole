using System.Text;
using Stoker.Plugin.Interfaces;
using Stoker.Plugin.Impl;

namespace Stoker.Plugin.Extension
{
    public static class CommandExtension
    {
        /// <summary>
        /// Adds standard help options (-h, --help) to a command.
        /// When these options are used, the command's help text will be displayed and the command will exit.
        /// 
        /// Example usage:
        /// command.AddHelpOptions();
        /// 
        /// This will add options that can be used like:
        /// command -h
        /// command --help
        /// </summary>
        public static void AddHelpOptions(this ICommand command)
        {
            var helpOption = new CommandOption<bool>
            {
                Name = "help",
                Description = "Show help information",
                Aliases = ["h"],
                DefaultValue = "",
                Parser = (value) => value != null
            };

            command.AddOption(helpOption);

            // Set up the handler to check for help options
            var handler = command.Handler;
            if (handler == null)
            {
                command.SetHandler((args) =>
                {
                    if (args.Options.ContainsKey("help") && args.Options["help"] is bool showHelp && showHelp)
                    {
                        System.Console.WriteLine(command.GetCommandHelpString());
                    }
                    return Task.CompletedTask;
                });
            }
            else
            {
                command.SetHandler(async (args) =>
                {
                    if (args.Options.ContainsKey("help") && args.Options["help"] is bool showHelp && showHelp)
                    {
                        System.Console.WriteLine(command.GetCommandHelpString());
                        return;
                    }

                    // If help wasn't requested, execute the original command
                    await command.ExecuteAsync(args);
                });
            }
        }

        /// <summary>
        /// Generates a usage string for a command.
        /// Example outputs:
        /// - Simple command: "command"
        /// - Command with options: "command [--option1 --option2=value]"
        /// - Command with many options: "command [options]"
        /// - Command with arguments: "command <arg1> <arg2>"
        /// - Command with no args/options: "command ..."
        /// </summary>
        public static string GetCommandUsage(this ICommand command)
        {
            var usage = new StringBuilder();

            usage.Append(command.Name);
            if (command.Options.Count() < 3)
            {
                usage.Append(" [");
                bool first = true;
                foreach (var option in command.Options)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        usage.Append(" ");
                    }

                    if (string.IsNullOrEmpty(option.DefaultValue))
                    {
                        usage.Append($"--{option.Name}");
                    }
                    else
                    {
                        usage.Append($"--{option.Name}=<{option.DefaultValue}>");
                    }
                }
                usage.Append("]");
            }
            else if (command.Options.Count() != 0)
            {
                usage.Append(" [options]");
            }

            if (command.Arguments.Count() == 0 && command.Options.Count() == 0)
            {
                usage.Append(" ...");
            }

            foreach (var argument in command.Arguments)
            {
                usage.Append($" <{argument.Name}>");
            }

            return usage.ToString();
        }

        /// <summary>
        /// Generates a complete help text for a command in docopt format.
        /// Example output:
        /// 
        /// Command Name.
        /// 
        /// Description of the command.
        /// 
        /// Usage:
        ///   command subcommand1 [--option1 --option2=value] <arg1>
        ///   command subcommand2 [options] <arg1> <arg2>
        ///   command [--option1 --option2=value] <arg1>
        /// 
        /// Options:
        ///   -o, --option1           Description of option1  [default: default1]
        ///   -p, --option2=<value>   Description of option2  [default: default2]
        /// </summary>
        public static string GetCommandHelpString(this ICommand command)
        {
            var stringBuilder = new StringBuilder();

            // Add command name and description
            stringBuilder.AppendLine($"{command.Name}.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(command.Description);
            stringBuilder.AppendLine();

            // Add usage section
            stringBuilder.AppendLine("Usage:");

            if (command.SubCommands.Any())
            {
                foreach (var subCommand in command.SubCommands)
                {
                    stringBuilder.AppendLine($"  {command.Name} {subCommand.GetCommandUsage()}");
                }
            }
            stringBuilder.AppendLine($"  {command.GetCommandUsage()}");

            // Add options section if there are any options
            if (command.Options.Any())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Options:");
                foreach (var option in command.Options)
                {
                    var aliases = option.Aliases.Select(a => $"-{a}").ToList();
                    aliases.Add($"--{option.Name}");
                    var optionString = string.Join(", ", aliases);
                    stringBuilder.AppendLine($"  {optionString,-30} {option.Description}  [default: {option.DefaultValue}]");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
