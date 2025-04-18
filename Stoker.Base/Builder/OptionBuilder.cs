using Stoker.Base.Impl;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Builder
{
    /// <summary>
    /// A fluent builder for creating command options.
    /// 
    /// Example usage:
    /// var option = new OptionBuilder<int>("count")
    ///     .WithDescription("Number of things")
    ///     .WithDefaultValue("1")
    ///     .WithAliases("c", "n")
    ///     .WithParser(value => int.Parse(value))
    ///     .IsRequired()
    ///     .Build();
    /// </summary>
    public class OptionBuilder<T>
    {
        private readonly CommandBuilder parentBuilder;

        private readonly CommandOption<T> _option;

        public OptionBuilder(CommandBuilder parentBuilder, string name)
        {
            _option = new CommandOption<T>
            {
                Name = name
            };
            this.parentBuilder = parentBuilder;
        }

        public OptionBuilder(CommandBuilder parentBuilder, CommandOption<T> option)
        {
            _option = option;
            this.parentBuilder = parentBuilder;
        }

        /// <summary>
        /// Sets the option's description.
        /// </summary>
        public OptionBuilder<T> WithDescription(string description)
        {
            _option.Description = description;
            return this;
        }

        /// <summary>
        /// Sets the option's default value.
        /// </summary>
        public OptionBuilder<T> WithDefaultValue(string defaultValue)
        {
            _option.DefaultValue = defaultValue;
            return this;
        }

        /// <summary>
        /// Sets the option's aliases.
        /// </summary>
        public OptionBuilder<T> WithAliases(params string[] aliases)
        {
            _option.Aliases = aliases;
            return this;
        }

        /// <summary>
        /// Sets the option's parser function.
        /// </summary>
        public OptionBuilder<T> WithParser(Func<string, T> parser)
        {
            _option.Parser = parser;
            return this;
        }

        /// <summary>
        /// Marks the option as required.
        /// </summary>
        public OptionBuilder<T> IsRequired()
        {
            _option.IsRequired = true;
            return this;
        }

        public CommandBuilder Parent()
        {
            return parentBuilder;
        }

        /// <summary>
        /// Builds and returns the configured option.
        /// </summary>
        public ICommandOption Build()
        {
            return _option;
        }
    }
} 