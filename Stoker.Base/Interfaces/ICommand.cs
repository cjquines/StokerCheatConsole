using Stoker.Base.Data;

namespace Stoker.Base.Interfaces
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<IArgument> Arguments { get; }
        IEnumerable<ICommandOption> Options { get; }
        IEnumerable<ICommand> SubCommands { get; }
        Func<HandlerArgs, Task>? Handler { get; }
        void AddArgument(IArgument argument);
        void AddOption(ICommandOption option);
        void AddSubCommand(ICommand subCommand);
        void SetHandler(Func<HandlerArgs, Task> handler);
        Task ExecuteAsync(string[] args);
        Task ExecuteAsync(HandlerArgs args);
        /// <summary>
        /// Get completions for the command.
        /// Completions are suggestions for the user to help them complete the command via tab completion.
        /// They use suggestions from the command's arguments and options.
        /// </summary>
        /// <param name="args">The arguments to get completions for.</param>
        /// <returns>An array of completions.</returns>
        string[] GetCompletions(string[] args);
    }
}
