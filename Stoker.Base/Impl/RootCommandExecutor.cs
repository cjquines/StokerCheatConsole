using System.Text;
using Stoker.Base.Data;
using Stoker.Base.Interfaces;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;

namespace Stoker.Base.Impl
{
    public class RootCommandExecutor : ICommandExecutor
    {
        public List<ICommand> Commands { get; set; } = [];

        private Lazy<ConsoleLogger> LoggerLazy { get; set; } = new(() => Railend.GetContainer().GetInstance<ConsoleLogger>());

        public bool TryAddCommand(ICommand command)
        {
            if (ContainsCommand(command.Name))
                return false;
            Commands.Add(command);
            return true;
        }

        public bool ContainsCommand(string commandName)
        {
            return Commands.Any(c => c.Name == commandName);
        }

        public Task ExecuteAsync(string fullCommand)
        {
            //split the command into command name and arguments, should split on spaces, except inside quotes (both " and ')
            var args = SplitArgs(fullCommand).ToArray();
            if (args.Length < 1)
                return Task.CompletedTask;
            var commandName = args[0];
            return this.ExecuteAsync(commandName, [.. args.Skip(1)]);
        }

        public Task ExecuteAsync(string commandName, string[] args)
        {
            var command = Commands.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
            {
                LoggerLazy.Value.Error($"Command '{commandName}' not found");
                return Task.CompletedTask;  
            }
            // Plugin.Logger.LogInfo($"Executing command: {commandName}");
            // Plugin.Logger.LogInfo($"Arguments: {string.Join(" ", args)}");
            LoggerLazy.Value.Log($"> {commandName} {string.Join(" ", args)}");
            try
            {
                return command.ExecuteAsync(args);
            }
            catch (Exception e)
            {
                LoggerLazy.Value.Error(e.Message);
                return Task.CompletedTask;
            }
        }

        private static IEnumerable<string> SplitArgs(string commandLine)
        {
            //@CREDIT: https://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp/298990#298990

            var result = new StringBuilder();
            var quoted = false; //whether we are inside quotes
            var escaped = false; //whether we are inside an escaped character
            var started = false; //whether we have started a new argument
            var allowcaret = false; //whether we are allowed to use ^ as a special character
            for (int i = 0; i < commandLine.Length; i++)
            {
                var chr = commandLine[i];

                if (chr == '^' && !quoted)
                {
                    if (allowcaret)
                    {
                        result.Append(chr);
                        started = true;
                        escaped = false;
                        allowcaret = false;
                    }
                    else if (i + 1 < commandLine.Length && commandLine[i + 1] == '^')
                    {
                        allowcaret = true;
                    }
                    else if (i + 1 == commandLine.Length)
                    {
                        result.Append(chr);
                        started = true;
                        escaped = false;
                    }
                }
                else if (escaped)
                {
                    result.Append(chr);
                    started = true;
                    escaped = false;
                }
                else if (chr == '"')
                {
                    quoted = !quoted;
                    started = true;
                }
                else if (chr == '\\' && i + 1 < commandLine.Length && commandLine[i + 1] == '"')
                {
                    escaped = true;
                }
                else if (chr == ' ' && !quoted)
                {
                    if (started) yield return result.ToString();
                    result.Clear();
                    started = false;
                }
                else
                {
                    result.Append(chr);
                    started = true;
                }
            }

            if (started) yield return result.ToString();
        }

        public string[] GetCompletions(string commandName, string[] args)
        {
            var command = Commands.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
                return [];

            return command.GetCompletions(args);
        }

        public string[] GetCompletions(string fullCommand)
        {
            var args = SplitArgs(fullCommand).ToArray();
            if (args.Length < 1)
                return [];
            var commandName = args[0];
            return this.GetCompletions(commandName, [.. args.Skip(1)]);
        }
    }
}
