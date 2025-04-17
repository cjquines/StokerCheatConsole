namespace Stoker.Plugin.Interfaces
{
    public interface ICommandOption
    {
        string Name { get; }
        string Description { get; }
        bool IsRequired { get; }
        string DefaultValue { get; }
        IEnumerable<string> Suggestions { get; }
        IEnumerable<string> Aliases { get; }
        object? Parse(string value);
    }

    public interface ICommandOption<T> : ICommandOption
    {
        new T? Parse(string value);
    }
}
