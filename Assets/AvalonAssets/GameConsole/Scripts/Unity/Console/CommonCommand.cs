using System;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Console
{
    /// <summary>
    ///     Common useful commands.
    /// </summary>
    /// <remarks>You should not use this classes directly.</remarks>
    public class CommonCommand : MonoBehaviour
    {
        /// <summary>
        ///     Game console that its registered to.
        /// </summary>
        [Tooltip("Game console that its registered to.")]
        public GameConsole GameConsole;

        /// <summary>
        ///     Print when a command not exist.
        /// </summary>
        public void CommandNotExist(Action<string> output, string[] args)
        {
            //output.Invoke("This command does not exists. Type \"Help\" to see what commands are available.");
        }

        /// <summary>
        ///     Print when type "Help" Command
        /// </summary>
        public void Help(Action<string> output, string[] args)
        {
            if (args.Length != 0)
                output.Invoke("Help does not support any argument.");
            else
                foreach (var command in GameConsole.GetCommands())
                    output.Invoke(command);
        }
    }
}