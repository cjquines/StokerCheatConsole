using Stoker.Plugin.Interfaces;

namespace Stoker.Plugin.Impl
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
        private readonly CommandOption<T> _option;

        public OptionBuilder(string name)
        {
            _option = new CommandOption<T>
            {
                Name = name
            };
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

        /// <summary>
        /// Builds and returns the configured option.
        /// </summary>
        public ICommandOption Build()
        {
            return _option;
        }
    }
} 