using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Edit
{
    /// <summary>
    ///     This is an internal utilities class for editor.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        ///     Default <see cref="Rect" /> height.
        /// </summary>
        internal const float DefaultRectHeight = 18f;

        /// <summary>
        ///     Default <see cref="Rect" /> Width.
        /// </summary>
        internal const float DefaultRectWidth = 18f;

        /// <summary>
        ///     Generate a default size progress bar for editor.
        /// </summary>
        /// <remarks><paramref name="value" /> should be between 0 to 1.</remarks>
        /// <param name="value">Perecentage of the progress bar.</param>
        /// <param name="text">Text show on the bar.</param>
        internal static void ProgressBar(float value, string text)
        {
            // Set Size
            var rect = GUILayoutUtility.GetRect(DefaultRectWidth, DefaultRectHeight);
            EditorGUI.ProgressBar(rect, value, text);
            EditorGUILayout.Space();
        }

        /// <summary>
        ///     Get the script path.
        /// </summary>
        /// <param name="behaviour">Class instance of <see cref="MonoBehaviour" />.</param>
        /// <returns>Script path.</returns>
        internal static string GetScriptPath(MonoBehaviour behaviour)
        {
            var script = MonoScript.FromMonoBehaviour(behaviour);
            return AssetDatabase.GetAssetPath(script);
        }

        /// <summary>
        ///     Get the script path.
        /// </summary>
        /// <param name="scriptableObject">Class instance of <see cref="ScriptableObject" />.</param>
        /// <returns>Script path.</returns>
        internal static string GetScriptPath(ScriptableObject scriptableObject)
        {
            var script = MonoScript.FromScriptableObject(scriptableObject);
            return AssetDatabase.GetAssetPath(script);
        }

        /// <summary>
        ///     Draw a default script field.
        /// </summary>
        /// <param name="behaviour">Class instance of the script.</param>
        internal static void ScriptField(MonoBehaviour behaviour)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(behaviour), typeof (MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        ///     Draw a default script field.
        /// </summary>
        /// <param name="scriptableObject">Class instance of the script.</param>
        internal static void ScriptField(ScriptableObject scriptableObject)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(scriptableObject), typeof (MonoScript),
                false);
            EditorGUI.EndDisabledGroup();
        }
    }
}