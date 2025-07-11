using Stoker.Base.Impl;
using Stoker.Base.Interfaces;

namespace Stoker.Base.Builder
{
    /// <summary>
    /// A fluent builder for creating subcommands.
    /// 
    /// Example usage:
    /// var subCommand = new SubCommandBuilder("add")
    ///     .WithDescription("Add a new item")
    ///     .WithArgument<string>("name", "Name of the item")
    ///     .WithOption<int>("count", "Number of items", defaultValue: "1")
    ///     .WithHandler(async (args) => { /* subcommand logic */ })
    ///     .WithHelp()
    ///     .Build();
    /// </summary>
    public class SubCommandBuilder : CommandBuilder
    {
        private readonly CommandBuilder parentBuilder;

        public SubCommandBuilder(CommandBuilder parentBuilder, Command command) : base(command)
        {
            this.parentBuilder = parentBuilder;
        }

        public SubCommandBuilder(CommandBuilder parentBuilder, string name) : base(name)
        {
            this.parentBuilder = parentBuilder;
        }

        public override CommandBuilder Parent()
        {
            return parentBuilder;
        }
    }
} 