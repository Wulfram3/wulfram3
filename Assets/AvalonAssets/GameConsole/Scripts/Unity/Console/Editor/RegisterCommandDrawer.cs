using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Console.Edit
{
    /// <summary>
    ///     Custom property drawer for <see cref="RegisterCommand" />.  For Unity editor use only.
    /// </summary>
    /// <remarks>You <b>do not</b> need it for scripting. Study it if you like.</remarks>
    [CustomPropertyDrawer(typeof (RegisterCommand))]
    public class RegisterCommandDrawer : PropertyDrawer
    {
        private const float LowPadding = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;

            var currentRect = new Rect(position) {height = EditorGUIUtility.singleLineHeight};
            var backgroundRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width,
                position.height - EditorGUIUtility.singleLineHeight);
            var titleRect = new Rect(currentRect);
            titleRect.y -= 2;
            titleRect.height += 4;

            var nameProperty = property.FindPropertyRelative("Name");
            var title = !string.IsNullOrEmpty(nameProperty.stringValue)
                ? nameProperty.stringValue
                : property.displayName;
            if(nameProperty.stringValue.Contains(" "))
                GUI.backgroundColor = Color.yellow;
            if (GUI.Button(titleRect, title, EditorStyles.objectFieldThumb))
                property.isExpanded = !property.isExpanded;
            GUI.backgroundColor = Color.white;
            if (property.isExpanded)
            {
                //Draw background
                EditorGUI.HelpBox(backgroundRect, "", MessageType.None);
                //Draw name
                currentRect.width -= 4;
                currentRect.y += 4 + EditorGUIUtility.singleLineHeight;
                if (nameProperty.stringValue.Contains(" "))
                    GUI.backgroundColor = Color.yellow;
                EditorGUI.PropertyField(currentRect, nameProperty);
                GUI.backgroundColor = Color.white;
                if (string.IsNullOrEmpty(nameProperty.stringValue))
                    nameProperty.stringValue = property.displayName;
                
                //Draw ConsoleEvent
                var eventRect = new Rect(currentRect);
                eventRect.x += 5;
                eventRect.width -= 5;
                eventRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var consoleEventProperty = property.FindPropertyRelative("Command");
                EditorGUI.PropertyField(eventRect, consoleEventProperty);


                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) + LowPadding;
        }
    }
}