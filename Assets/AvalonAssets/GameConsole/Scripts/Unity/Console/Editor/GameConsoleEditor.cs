using AvalonAssets.Unity.Edit;
using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Console.Edit
{
    /// <summary>
    ///     Custom editor for <see cref="GameConsole" />.  For Unity editor use only.
    /// </summary>
    /// <remarks>You <b>do not</b> need it for scripting. Study it if you like.</remarks>
    [CustomEditor(typeof (GameConsole))]
    public class GameConsoleEditor : Editor
    {
        private SerializedProperty _allowConsoleProperty;
        private SerializedProperty _commandColorProperty;
        private SerializedProperty _commandPrefixProperty;
        private SerializedProperty _commandsProperty;
        private GameConsole _gameConsole;
        private SerializedProperty _inputFieldProperty;
        private bool _isCommandExpanded = true;
        private bool _isConfigurationExpanded = true;
        private bool _isGuiComponentExpanded = true;
        private SerializedProperty _nonExistCommandProperty;
        private SerializedProperty _openConsoleKeyProperty;
        private SerializedProperty _outputTextProperty;
        private SerializedProperty _printCommandProperty;
        private SerializedProperty _scrollbarProperty;
        private SerializedProperty _scrollRectProperty;
        private SerializedProperty _allowAutoCompleteProperty;
        private SerializedProperty _autoCompleteProperty;

        private void OnEnable()
        {
            _commandsProperty = serializedObject.FindProperty("Commands");
            _inputFieldProperty = serializedObject.FindProperty("InputField");
            _outputTextProperty = serializedObject.FindProperty("OutputText");
            _scrollbarProperty = serializedObject.FindProperty("Scrollbar");
            _scrollRectProperty = serializedObject.FindProperty("ScrollRect");
            _allowConsoleProperty = serializedObject.FindProperty("AllowConsole");
            _openConsoleKeyProperty = serializedObject.FindProperty("OpenConsoleKey");
            _nonExistCommandProperty = serializedObject.FindProperty("NonExistCommand");
            _commandPrefixProperty = serializedObject.FindProperty("CommandPrefix");
            _commandColorProperty = serializedObject.FindProperty("CommandColor");
            _printCommandProperty = serializedObject.FindProperty("PrintCommand");
            _autoCompleteProperty = serializedObject.FindProperty("AutoComplete");
            _allowAutoCompleteProperty = serializedObject.FindProperty("AllowAutoComplete");
            _gameConsole = (GameConsole) serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            Utility.ScriptField(_gameConsole);
            serializedObject.Update();
            if (_inputFieldProperty.objectReferenceValue == null ||
                _outputTextProperty.objectReferenceValue == null ||
                _scrollbarProperty.objectReferenceValue == null ||
                _scrollRectProperty.objectReferenceValue == null ||
                (_autoCompleteProperty.objectReferenceValue == null && _allowAutoCompleteProperty.boolValue))
                GUI.backgroundColor = Color.red;
            if (GUILayout.Button("GUI Component", EditorStyles.objectFieldThumb))
                _isGuiComponentExpanded = !_isGuiComponentExpanded;
            GUI.backgroundColor = Color.white;
            if (_isGuiComponentExpanded)
            {
                var rect = EditorGUILayout.BeginVertical();
                rect.y -= 4;
                rect.height += 7;
                rect.x -= 2;
                rect.width += 4;

                //Draw background
                EditorGUI.HelpBox(rect, "", MessageType.None);

                if (_inputFieldProperty.objectReferenceValue == null)
                    GUI.backgroundColor = Color.red;
                EditorGUILayout.PropertyField(_inputFieldProperty);
                GUI.backgroundColor = Color.white;
                if (_outputTextProperty.objectReferenceValue == null)
                    GUI.backgroundColor = Color.red;
                EditorGUILayout.PropertyField(_outputTextProperty);
                GUI.backgroundColor = Color.white;
                if (_scrollbarProperty.objectReferenceValue == null)
                    GUI.backgroundColor = Color.red;
                EditorGUILayout.PropertyField(_scrollbarProperty);
                GUI.backgroundColor = Color.white;
                if (_scrollRectProperty.objectReferenceValue == null)
                    GUI.backgroundColor = Color.red;
                EditorGUILayout.PropertyField(_scrollRectProperty);
                GUI.backgroundColor = Color.white;
                if (_allowAutoCompleteProperty.boolValue)
                {
                    if (_autoCompleteProperty.objectReferenceValue == null)
                        GUI.backgroundColor = Color.red;
                    EditorGUILayout.PropertyField(_autoCompleteProperty);
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Configuration", EditorStyles.objectFieldThumb))
                _isConfigurationExpanded = !_isConfigurationExpanded;
            if (_isConfigurationExpanded)
            {
                var rect = EditorGUILayout.BeginVertical();
                rect.y -= 4;
                rect.height += 7;
                rect.x -= 2;
                rect.width += 4;

                //Draw background
                EditorGUI.HelpBox(rect, "", MessageType.None);

                EditorGUILayout.PropertyField(_allowConsoleProperty);
                EditorGUILayout.PropertyField(_allowAutoCompleteProperty);
                EditorGUILayout.PropertyField(_openConsoleKeyProperty);
                EditorGUILayout.PropertyField(_printCommandProperty);
                if (_printCommandProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(_commandPrefixProperty);
                    EditorGUILayout.PropertyField(_commandColorProperty);
                }
                EditorGUILayout.PropertyField(_nonExistCommandProperty);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Command", EditorStyles.objectFieldThumb))
                _isCommandExpanded = !_isCommandExpanded;
            if (_isCommandExpanded)
            {
                var rect = EditorGUILayout.BeginVertical();
                rect.y -= 4;
                rect.height += 7;
                rect.x -= 2;
                rect.width += 4;

                //Draw background
                EditorGUI.HelpBox(rect, "", MessageType.None);

                // Create New Button
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("New Command"))
                    _commandsProperty.InsertArrayElementAtIndex(_commandsProperty.arraySize);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.Space();

                // List all pools
                for (var i = 0; i < _commandsProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(_commandsProperty.GetArrayElementAtIndex(i), false);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical(GUILayout.Width(20));
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("×", GUILayout.ExpandWidth(false)))
                        _commandsProperty.DeleteArrayElementAtIndex(i);
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}