namespace Stoker.Plugin.Interfaces
{
    public interface IArgument
    {
        string Name { get; }
        string Description { get; }
        string DefaultValue { get; }
        IEnumerable<string> Suggestions { get; }
        object? Parse(string value);
    }

    public interface IArgument<T> : IArgument
    {
        new T? Parse(string value);
    }
}
