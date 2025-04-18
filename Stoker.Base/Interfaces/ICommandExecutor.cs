namespace Stoker.Base.Interfaces
{
    public interface ICommandExecutor
    {
        Task ExecuteAsync(string fullCommand);
        Task ExecuteAsync(string commandName, string[] args);
        bool ContainsCommand(string commandName);
        bool TryAddCommand(ICommand command);
        string[] GetCompletions(string fullCommand);
        string[] GetCompletions(string commandName, string[] args);
    }
}
