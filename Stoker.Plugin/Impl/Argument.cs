using Stoker.Plugin.Interfaces;

namespace Stoker.Plugin.Impl
{
    public class Argument<T> : IArgument<T> 
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string DefaultValue { get; set; } = "";

        public Func<string, T?>? Parser { get; set; } = null;
        public Func<IEnumerable<string>>? SuggestionsProvider { get; set; } = null;

        public IEnumerable<string> Suggestions => SuggestionsProvider?.Invoke() ?? [];

        public T? Parse(string value)
        {
            if (Parser == null)
                return default;

            return Parser.Invoke(value);
        }

        object? IArgument.Parse(string value)
        {
            return Parse(value);
        }
    }
}
