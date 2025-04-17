using Stoker.Plugin.Data;
using Stoker.Plugin.Interfaces;

namespace Stoker.Plugin.Impl
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

                // Check if it's a subcommand
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
                    var parsedValue = argument.Parse(currentArg);
                    handlerArgs.Arguments.Add(parsedValue);
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

            if (argumentIndex < Arguments.Count)
            {
                throw new Exception($"Missing arguments: ({string.Join(", ", Arguments.Skip(argumentIndex).Select(a => a.Name))})");
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

        private Task ExecuteAsyncInternal(HandlerArgs args){
            
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
    }
}
