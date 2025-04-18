using Stoker.Base.Impl;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Builder
{
    /// <summary>
    /// A fluent builder for creating command arguments.
    /// 
    /// Example usage:
    /// var argument = new ArgumentBuilder<string>("name")
    ///     .WithDescription("The name of the thing")
    ///     .WithDefaultValue("default")
    ///     .WithParser(value => value.ToUpper())
    ///     .WithSuggestions(() => ["suggestion1", "suggestion2"])
    ///     .Build();
    /// </summary>
    public class ArgumentBuilder<T>
    {
        private readonly CommandBuilder parentBuilder;

        private readonly Argument<T> _argument;

        public ArgumentBuilder(CommandBuilder parentBuilder, string name)
        {
            _argument = new Argument<T>
            {
                Name = name
            };
            this.parentBuilder = parentBuilder;
        }

        public ArgumentBuilder(CommandBuilder parentBuilder, Argument<T> argument)
        {
            _argument = argument;
            this.parentBuilder = parentBuilder;
        }

        /// <summary>
        /// Sets the argument's description.
        /// </summary>
        public ArgumentBuilder<T> WithDescription(string description)
        {
            _argument.Description = description;
            return this;
        }

        /// <summary>
        /// Sets the argument's default value.
        /// </summary>
        public ArgumentBuilder<T> WithDefaultValue(string defaultValue)
        {
            _argument.DefaultValue = defaultValue;
            return this;
        }

        /// <summary>
        /// Sets the argument's parser function.
        /// </summary>
        public ArgumentBuilder<T> WithParser(Func<string, T> parser)
        {
            _argument.Parser = parser;
            return this;
        }

        /// <summary>
        /// Sets the argument's suggestions provider.
        /// </summary>
        public ArgumentBuilder<T> WithSuggestions(Func<string[]> suggestionsProvider)
        {
            _argument.SuggestionsProvider = suggestionsProvider;
            return this;
        }

        public CommandBuilder Parent()
        {
            return parentBuilder;
        }

        /// <summary>
        /// Builds and returns the configured argument.
        /// </summary>
        public IArgument Build()
        {
            return _argument;
        }
    }
} 