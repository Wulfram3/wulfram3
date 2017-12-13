using System;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Console
{
    /// <summary>
    ///     Command line command.
    /// </summary>
    /// <param name="args">Arguments of the command.</param>
    /// <param name="output">Output message on screen.</param>
    public delegate void Command(Action<string> output, string[] args);
}