namespace Stoker.Base.Interfaces
{
    public interface IArgument
    {
        string Name { get; }
        string Description { get; }
        string DefaultValue { get; }
        IEnumerable<string> Suggestions { get; }
        object? Parse(string value);
        Type Type { get; }
    }

    public interface IArgument<T> : IArgument
    {
        new T? Parse(string value);
    }
}
