using Stoker.Plugin.Data;

namespace Stoker.Plugin.Interfaces
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
    }
}
