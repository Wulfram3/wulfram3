using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Console
{
    /// <summary>
    ///     Command line console system. Parse commandline and exeute registered command.
    /// </summary>
    public class CommandLineConsole
    {
        public const string CommandNotExist = "COMMAND_DOES_NOT_EXIST";
        private readonly Action<string> _output;
        private readonly IDictionary<string, Command> _registerCommands;

        /// <summary>
        ///     Create a new <see cref="CommandLineConsole" />.
        /// </summary>
        public CommandLineConsole() : this(message => { })
        {
        }

        /// <summary>
        ///     Create a new <see cref="CommandLineConsole" />.
        /// </summary>
        /// <param name="output">Function used to output message.</param>
        public CommandLineConsole(Action<string> output)
        {
            _registerCommands = new Dictionary<string, Command>();
            _output = output;
        }

        /// <summary>
        ///     Register a new command.
        /// </summary>
        /// <remarks>If a command with the same name has registered, the new one will be ignored.</remarks>
        /// <param name="name">Name of the command.</param>
        /// <param name="command">Command to be execute.</param>
        public void RegisterCommand(string name, Command command)
        {
            if (name == null || command == null) return;
            if (_registerCommands.ContainsKey(name)) return;
            _registerCommands.Add(name, command);
        }

        /// <summary>
        ///     Deregister a command.
        /// </summary>
        /// <param name="name">Name of the command.</param>
        public void DeregisterCommand(string name)
        {
            if (name != null)
                _registerCommands.Remove(name);
        }

        /// <summary>
        ///     Read a command and execute it if it has registered.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Command should looks like "command_name arg1 arg2" with quotes. There is no limit on argumenets.
        ///     </para>
        ///     <para>
        ///         Argments can use "" to pass a string with space. For exmaple, "a b "c d"" wtih exeute command named "a" with
        ///         two arguments "b" and "c d".
        ///     </para>
        ///     <para>There is no escape for ". Also, starting space and end space need to be quoted.</para>
        /// </remarks>
        /// <param name="command">Commnad input by user.</param>
        public void Read(string command)
        {
            if (command == null) return;
            var splitedArray = SplitCommand(command).ToArray();
            if (splitedArray.Length < 1) return;
            var commandName = splitedArray[0];
            // Replace command to CommandNotExist if command does not exist
            if (!_registerCommands.ContainsKey(commandName))
                if (_registerCommands.ContainsKey(CommandNotExist))
                    commandName = CommandNotExist;
                else
                    return;

            var commandArguments = splitedArray.Skip(1).ToArray();
            _registerCommands[commandName](_output, commandArguments);
        }

        /// <summary>
        ///     Split the command into array.
        /// </summary>
        /// <param name="command">Command to be splitd.</param>
        /// <returns>Splitd command.</returns>
        private static IEnumerable<string> SplitCommand(string command)
        {
            var inQuotes = false;
            return command.Split(character =>
            {
                if (character == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && character == ' ';
            })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }

        /// <summary>
        ///     Get all the registered commands.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCommands()
        {
            return _registerCommands.Keys.Except(new[] {CommandNotExist});
        }
    }
}