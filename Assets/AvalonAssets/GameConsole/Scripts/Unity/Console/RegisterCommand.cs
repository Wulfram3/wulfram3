using System;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Console
{
    /// <summary>
    ///     For Unity editor use only,
    /// </summary>
    [Serializable]
    public class RegisterCommand
    {
        /// <summary>
        ///     Event to be triggered.
        /// </summary>
        public ConsoleEvent Command;

        /// <summary>
        ///     Name of the command.
        /// </summary>
        public string Name;
    }
}