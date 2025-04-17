using Stoker.Plugin.Data;
using Stoker.Plugin.Extension;
using Stoker.Plugin.Impl;
using Stoker.Plugin.Interfaces;

namespace Stoker.Plugin.Builder
{
    /// <summary>
    /// A fluent builder for creating and configuring commands.
    /// 
    /// Example usage:
    /// var command = new CommandBuilder("mycommand")
    ///     .WithDescription("A command that does something")
    ///     .WithArgument("arg1", "First argument")
    ///     .WithOption("option1", "An optional parameter")
    ///     .WithSubCommand(subCommand)
    ///     .WithHandler(async (args) => { /* command logic */ })
    ///     .WithHelp()
    ///     .Build();
    /// </summary>
    public class CommandBuilder
    {
        protected readonly Command _command;

        public CommandBuilder(string name)
        {
            _command = new Command
            {
                Name = name
            };
        }

        public CommandBuilder(Command command)
        {
            _command = command;
        }

        /// <summary>
        /// Sets the command's description.
        /// </summary>
        public CommandBuilder WithDescription(string description)
        {
            _command.Description = description;
            return this;
        }

        public ArgumentBuilder<string> WithArgument(string name){
            var argument = new Argument<string>
            {
                Name = name
            };
            _command.AddArgument(argument);
            return new ArgumentBuilder<string>(this, argument);
        }

        /// <summary>
        /// Adds an option to the command.
        /// </summary>
        public OptionBuilder<T> WithOption<T>(string name){
            var option = new CommandOption<T>
            {   
                Name = name,
            };
            _command.AddOption(option);
            return new OptionBuilder<T>(this, option);
        }

        /// <summary>
        /// Adds a subcommand to the command.
        /// </summary>
        public SubCommandBuilder WithSubCommand(string name)
        {
            var subCommand = new Command
            {
                Name = name
            };
            _command.AddSubCommand(subCommand);
            return new SubCommandBuilder(this, subCommand);
        }

        /// <summary>
        /// Sets the command's handler function.
        /// </summary>
        public CommandBuilder SetHandler(Func<HandlerArgs, Task> handler)
        {
            _command.SetHandler(handler);
            return this;
        }

        public CommandBuilder UseHandlerMiddleware(Func<Func<HandlerArgs, Task>?, Func<HandlerArgs, Task>> middleware)
        {
            var oldHandler = _command.Handler;
            _command.SetHandler(middleware(oldHandler));
            return this;
        }

        public string GetCommandHelpString()
        {
            return _command.GetCommandHelpString();
        }

        public string GetCommandUsageString()
        {
            return _command.GetCommandUsage();
        }

        /// <summary>
        /// Builds and returns the configured command.
        /// </summary>
        public ICommand Build()
        {
            return _command;
        }

        public virtual CommandBuilder Parent()
        {
            return this;
        }
    }
} 