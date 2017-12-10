using System;
using UnityEngine.Events;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Console
{
    /// <summary>
    ///     A serializable <see cref="UnityEvent{T,U}" />.
    /// </summary>
    [Serializable]
    public class ConsoleEvent : UnityEvent<Action<string>,string[]>
    {
    }
}