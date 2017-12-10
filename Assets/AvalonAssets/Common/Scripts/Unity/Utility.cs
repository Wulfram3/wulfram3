using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity
{
    /// <summary>
    ///     This is an internal utilities class for Unity.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        ///     Returns the component with interface type if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <typeparam name="T">The type of Component to retrieve.</typeparam>
        /// <param name="obj"><see cref="GameObject" /> to be searched.</param>
        /// <returns>The component with interface type</returns>
        internal static T GetInterface<T>(this GameObject obj) where T : class
        {
            if (typeof (T).IsInterface) return obj.GetComponents<Component>().OfType<T>().FirstOrDefault();
            Debug.LogError(typeof (T) + ": is not an interface.");
            return null;
        }

        /// <summary>
        ///     Returns all components of interface type in the GameObject.
        /// </summary>
        /// <typeparam name="T">The type of Component to retrieve.</typeparam>
        /// <param name="obj"><see cref="GameObject" /> to be searched.</param>
        /// <returns>All components of interface type.</returns>
        internal static IEnumerable<T> GetInterfaces<T>(this GameObject obj) where T : class
        {
            if (typeof (T).IsInterface) return obj.GetComponents<Component>().OfType<T>();
            Debug.LogError(typeof (T) + ": is not an interface.");
            return Enumerable.Empty<T>();
        }

        /// <summary>
        ///     Convert color to a hex value.
        /// </summary>
        /// <param name="color">Color to be converted.</param>
        /// <returns>Hex value of the color.</returns>
        internal static string ToHex(this Color32 color)
        {
            var hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        ///     Convert color to a hex value.
        /// </summary>
        /// <param name="color">Color to be converted.</param>
        /// <returns>Hex value of the color.</returns>
        internal static string ToHex(this Color color)
        {
            Color32 color32 = color;
            return color32.ToHex();
        }

        /// <summary>
        ///     Convert hex value to a color.
        /// </summary>
        /// <param name="hex">Hex value to be converted.</param>
        /// <returns>Color of the hex value.</returns>
        internal static Color ToColor(this string hex)
        {
            return hex.ToColor32();
        }

        /// <summary>
        ///     Convert hex value to a color.
        /// </summary>
        /// <param name="hex">Hex value to be converted.</param>
        /// <returns>Color of the hex value.</returns>
        internal static Color32 ToColor32(this string hex)
        {
            hex = hex.Replace("0x", ""); //in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", ""); //in case the string is formatted #FFFFFF
            byte a = 255; //assume fully visible unless specified in hex
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        /// <summary>
        ///     Convert a string to a rich text string.
        /// </summary>
        /// <param name="input">Orginal string.</param>
        /// <param name="hex">Hex value of the color.</param>
        /// <returns>Rich text string</returns>
        internal static string AddColor(this string input, string hex)
        {
            return "<color=" + hex + ">" + input + "</color>";
        }

        /// <summary>
        ///     Convert a string to a rich text string.
        /// </summary>
        /// <param name="input">Orginal string.</param>
        /// <param name="color">Color to be added.</param>
        /// <returns>Rich text string</returns>
        internal static string AddColor(this string input, Color color)
        {
            return "<color=" + color.ToHex() + ">" + input + "</color>";
        }

        /// <summary>
        ///     Convert a string to a rich text string.
        /// </summary>
        /// <param name="input">Orginal string.</param>
        /// <param name="color">Color to be added.</param>
        /// <returns>Rich text string</returns>
        internal static string AddColor(this string input, Color32 color)
        {
            return "<color=" + color.ToHex() + ">" + input + "</color>";
        }
    }
}