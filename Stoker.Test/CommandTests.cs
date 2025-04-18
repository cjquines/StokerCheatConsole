using Stoker.Base.Impl;
using Stoker.Base.Interfaces;
using Xunit;

namespace Stoker.Test
{
    public class CommandTests
    {
        private Command CreateTestCommand()
        {
            var command = new Command
            {
                Name = "card",
                Description = "Manage cards"
            };

            // Add subcommands
            var addCommand = new Command
            {
                Name = "add",
                Description = "Add a card"
            };

            var removeCommand = new Command
            {
                Name = "remove",
                Description = "Remove a card"
            };

            command.AddSubCommand(addCommand);
            command.AddSubCommand(removeCommand);

            // Add arguments to add command
            var cardNameArg = new Argument<string>
            {
                Name = "name",
                Description = "Card name",
                SuggestionsProvider = () => ["fireball", "icebolt", "lightning"]
            };
            addCommand.AddArgument(cardNameArg);

            // Add options to add command
            var helpOption = new CommandOption<bool>
            {
                Name = "help",
                Description = "Show help",
                DefaultValue = "false"
            };
            addCommand.AddOption(helpOption);

            var verboseOption = new CommandOption<bool>
            {
                Name = "verbose",
                Description = "Show verbose output",
                DefaultValue = "false"
            };
            addCommand.AddOption(verboseOption);

            var pileOption = new CommandOption<int>
            {
                Name = "pile",
                Description = "Pile number",
                DefaultValue = "1",
                SuggestionsProvider = () => ["1", "2", "3"]
            };
            addCommand.AddOption(pileOption);

            return command;
        }

        [Fact]
        public void BaseCommand_ReturnsSubcommands()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions([]);

            Assert.Contains("add", completions);
            Assert.Contains("remove", completions);
            Assert.Equal(2, completions.Length);
        }

        [Fact]
        public void Subcommand_ReturnsArgumentSuggestions()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["add"]);

            Assert.Contains("fireball", completions);
            Assert.Contains("icebolt", completions);
            Assert.Contains("lightning", completions);
            Assert.Equal(3, completions.Length);
        }

        [Fact]
        public void AfterArgument_ReturnsOptions()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["add", "fireball"]);

            Assert.Contains("--help", completions);
            Assert.Contains("--verbose", completions);
            Assert.Contains("--pile", completions);
            Assert.Equal(3, completions.Length);
        }

        // [Fact]
        // public void PartialOption_ReturnsMatchingOptions()
        // {
        //     var command = CreateTestCommand();
        //     var completions = command.GetCompletions(["add", "--he"]);

        //     Assert.Contains("--help", completions);
        //     Assert.Single(completions);
        // }

        [Fact]
        public void CompleteOption_ReturnsOptionValues()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["add", "--pile"]);

            Assert.Contains("1", completions);
            Assert.Contains("2", completions);
            Assert.Contains("3", completions);
            Assert.Equal(3, completions.Length);
        }

        [Fact]
        public void UsedOption_NotSuggestedAgain()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["add", "--help", "fireball"]);

            Assert.DoesNotContain("--help", completions);
            Assert.Contains("--verbose", completions);
            Assert.Contains("--pile", completions);
            Assert.Equal(2, completions.Length);
        }

        [Fact]
        public void MixedOptionsAndArguments_HandlesCorrectly()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["add", "--help", "fireball", "--"]);

            Assert.Contains("--verbose", completions);
            Assert.Contains("--pile", completions);
            Assert.Equal(2, completions.Length);
        }

        [Fact]
        public void UnknownSubcommand_ReturnsEmpty()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["unknown"]);

            Assert.Empty(completions);
        }

        [Fact]
        public void PartialSubcommand_ReturnsMatching()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["ad"]);

            Assert.Contains("add", completions);
            Assert.Single(completions);
        }

        [Fact]
        public void OptionWithValue_HandlesNextArgument()
        {
            var command = CreateTestCommand();
            var completions = command.GetCompletions(["add", "--pile", "1", "fireball"]);

            Assert.Contains("--help", completions);
            Assert.Contains("--verbose", completions);
            Assert.Equal(2, completions.Length);
        }
    }
} 