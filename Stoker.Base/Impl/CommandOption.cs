
using Stoker.Base.Interfaces;

namespace Stoker.Base.Impl
{
    public class CommandOption<T> : ICommandOption<T>
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsRequired { get; set; } = false;
        public string DefaultValue { get; set; } = "";
        public IEnumerable<string> Aliases { get; set; } = [];
        public IEnumerable<string> Suggestions => SuggestionsProvider?.Invoke() ?? [];

        public Func<string, T?>? Parser { get; set; } = null;
        public Func<IEnumerable<string>>? SuggestionsProvider { get; set; } = null;

        public T? Parse(string value)
        {
            if (Parser == null)
                return default;

            return Parser.Invoke(value);
        }

        public Type Type => typeof(T);

        object? ICommandOption.Parse(string value)
        {
            return Parse(value);
        }
    }
}
