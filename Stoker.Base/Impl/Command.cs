using Stoker.Base.Data;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Impl
{
    public class Command : ICommand
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<IArgument> Arguments { get; set; } = [];
        public List<ICommandOption> Options { get; set; } = [];
        public List<ICommand> SubCommands { get; set; } = [];
        public Func<HandlerArgs, Task>? Handler { get; set; } = null;

        IEnumerable<IArgument> ICommand.Arguments => Arguments;

        IEnumerable<ICommandOption> ICommand.Options => Options;

        IEnumerable<ICommand> ICommand.SubCommands => SubCommands;

        public void AddArgument(IArgument argument)
        {
            Arguments.Add(argument);
        }

        public void AddOption(ICommandOption option)
        {
            Options.Add(option);
        }

        public void AddSubCommand(ICommand subCommand)
        {
            SubCommands.Add(subCommand);
        }

        private HandlerArgs ParseArgs(string[] args)
        {
            var handlerArgs = new HandlerArgs();
            var argumentIndex = 0;
            var processedOptions = new HashSet<string>();

            //Add default values for all options with a default value
            foreach (var option in Options)
            {
                if (string.IsNullOrEmpty(option.DefaultValue))
                {
                    continue;
                }
                handlerArgs.Options.Add(option.Name, option.Parse(option.DefaultValue));
            }

            //Add default values for all arguments with a default value
            foreach (var argument in Arguments)
            {
                if (string.IsNullOrEmpty(argument.DefaultValue))
                {
                    continue;
                }
                handlerArgs.Arguments.Add(argument.Name, argument.Parse(argument.DefaultValue));
            }

            //Parse the arguments
            for (int i = 0; i < args.Length; i++)
            {
                string currentArg = args[i];

                // Check if it's an option (starts with -- or -)
                if (currentArg.StartsWith("--") || currentArg.StartsWith("-"))
                {
                    //Get the Option
                    string optionName = currentArg.TrimStart('-');
                    var option = Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Unknown option: {optionName}");

                    //Get the Parsed Value
                    object? parsedValue = null;
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        string nextArg = args[++i];
                        parsedValue = option.Parse(nextArg);
                    }
                    else
                    {
                        parsedValue = option.Parse(option.DefaultValue);
                    }

                    //Add the Option and Value to the HandlerArgs
                    if (handlerArgs.Options.ContainsKey(optionName))
                    {
                        handlerArgs.Options[optionName] = parsedValue;
                    }
                    else
                    {
                        handlerArgs.Options.Add(optionName, parsedValue);
                    }

                    processedOptions.Add(optionName);
                    continue;
                }

                // Check if it is a subcommand
                var subCommand = SubCommands.FirstOrDefault(sc => sc.Name.Equals(currentArg, StringComparison.OrdinalIgnoreCase));
                if (subCommand != null)
                {
                    // Collect all remaining arguments for the subcommand
                    handlerArgs.SubCommand = currentArg;
                    handlerArgs.UnparsedArgs = [.. args.Skip(i + 1)];
                    break;
                }

                // If we get here, treat it as a positional argument
                if (argumentIndex < Arguments.Count)
                {
                    var argument = Arguments[argumentIndex];
                    if (handlerArgs.Arguments.ContainsKey(argument.Name))
                    {
                        handlerArgs.Arguments[argument.Name] = argument.Parse(currentArg);
                    }
                    else
                    {
                        handlerArgs.Arguments.Add(argument.Name, argument.Parse(currentArg));
                    }
                    argumentIndex++;
                }
                else
                {
                    throw new Exception($"One too many arguments: {currentArg}");
                }
            }

            // Validate required options
            var missingRequiredOptions = Options
                .Where(o => o.IsRequired && !processedOptions.Contains(o.Name))
                .Select(o => o.Name)
                .ToList();
            if (missingRequiredOptions.Any())
            {
                throw new Exception($"Missing required options: {string.Join(", ", missingRequiredOptions)}");
            }

            return handlerArgs;
        }

        public Task ExecuteAsync(string[] args)
        {
            var handlerArgs = ParseArgs(args);
            return ExecuteAsyncInternal(handlerArgs);
        }

        public Task ExecuteAsync(HandlerArgs args)
        {
            var handlerArgs = ParseArgs(args.UnparsedArgs);
            return ExecuteAsyncInternal(handlerArgs);
        }

        private Task ExecuteAsyncInternal(HandlerArgs args)
        {

            // If we have subcommand arguments, find the appropriate subcommand and execute it
            if (args.SubCommand != null)
            {
                var subCommand = SubCommands.FirstOrDefault(sc => sc.Name.Equals(args.SubCommand, StringComparison.OrdinalIgnoreCase));
                if (subCommand != null)
                {
                    return subCommand.ExecuteAsync(args);
                }
            }

            if (Handler == null)
            {
                throw new Exception("No handler set for command");
            }

            return Handler(args);
        }

        public void SetHandler(Func<HandlerArgs, Task> handler)
        {
            Handler = handler;
        }

        public string[] GetCompletions(string[] args)
        {
            var completions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var lastArg = args.Length > 0 ? args[^1] : "";
            var isOption = lastArg.StartsWith("-");

            // Handle subcommand completion
            if (args.Length == 0 && SubCommands.Count > 0)
            {
                // Base command completion - suggest all subcommands
                completions.UnionWith(SubCommands.Select(sc => sc.Name));
                return [.. completions];
            }

            // If we have a subcommand, delegate to it
            var subCommand = SubCommands.FirstOrDefault(sc => sc.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));
            if (subCommand != null)
            {
                return subCommand.GetCompletions([.. args.Skip(1)]);
            }

            completions.UnionWith(SubCommands.Where(sc => sc.Name.StartsWith(args[0], StringComparison.OrdinalIgnoreCase)).Select(sc => sc.Name));

            var numOptions = 0;
            var usedOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("-"))
                {
                    var name = arg.TrimStart('-');
                    var option = Options.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    usedOptions.Add(name);
                    if (option == null)
                    {
                        continue; // unknown option
                    }

                    bool isBoolean = option.Type == typeof(bool);
                    bool expectsValue = !isBoolean;

                    if (!expectsValue)
                    {
                        numOptions++; // e.g. --verbose
                    }
                    else
                    {
                        numOptions++; // count the option itself

                        // Try to safely check if the next argument is not another option
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                        {
                            numOptions++; // count the value
                            i++; // consume the value
                        }
                    }
                }
            }

            var currentArgIndex = args.Length - numOptions;

            // Handle positional argument completion
            if (!isOption && currentArgIndex < Arguments.Count)
            {
                var currentArgument = Arguments[currentArgIndex];
                completions.UnionWith(currentArgument.Suggestions);
            }

            var pastArgIndex = currentArgIndex - 1;
            if (!isOption && pastArgIndex >= 0 && pastArgIndex < Arguments.Count)
            {
                var pastArgument = Arguments[pastArgIndex];
                completions
                    .UnionWith(pastArgument.Suggestions
                    .Where(s => s != lastArg && s.StartsWith(lastArg, StringComparison.OrdinalIgnoreCase)));
            }

            // Handle option completion
            if (isOption)
            {
                // If we're in the middle of typing an option
                if (lastArg.StartsWith("-"))
                {
                    var optionName = args[^1].TrimStart('-');
                    var option = Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
                    if (option != null)
                    {
                        completions.UnionWith(option.Suggestions);
                    }
                    else
                    {
                        // Suggest matching options
                        completions.UnionWith(Options
                            .Where(o => !usedOptions.Contains(o.Name))
                            .Select(o => $"--{o.Name}")
                            .Where(c => c.StartsWith(lastArg, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                // If we have a complete option name, suggest its values
                else if (args.Length >= 2 && args[^2].StartsWith("--"))
                {
                    var optionName = args[^2].TrimStart('-');
                    var option = Options.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
                    if (option != null)
                    {
                        completions.UnionWith(option.Suggestions);
                    }
                }
            }

            // If we've provided all arguments, suggest remaining options
            if (currentArgIndex >= Arguments.Count)
            {
                completions.UnionWith(Options
                    .Where(o => !usedOptions.Contains(o.Name))
                    .Select(o => $"--{o.Name}"));
            }

            // If we have no completions but have a partial word, try fuzzy matching
            if (completions.Count == 0 && !string.IsNullOrEmpty(lastArg))
            {
                // Try matching subcommands
                completions.UnionWith(SubCommands
                    .Where(sc => sc.Name.Contains(lastArg, StringComparison.OrdinalIgnoreCase))
                    .Select(sc => sc.Name));

                // Try matching options
                completions.UnionWith(Options
                    .Where(o => o.Name.Contains(lastArg, StringComparison.OrdinalIgnoreCase))
                    .Select(o => $"--{o.Name}"));
            }

            return [.. completions];
        }
    }
}
