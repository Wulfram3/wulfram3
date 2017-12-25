/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using System.Collections.Generic;
using System.Linq;
using PhotonChatUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ChatChannelMessageUI))]
public class ChatChannelMessageUIEditor : Editor
{
    private ReorderableList _emoticonsReorderableList;

    private readonly GUIContent _emptyGUIContent = new GUIContent("");

    private readonly GUIContent _tagGUIContent = new GUIContent("Tag");

    private readonly GUIContent _charWidthGUIContent = new GUIContent("Width");

    public virtual void OnEnable()
    {
        _emoticonsReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("Emoticons"),
            true, false, true, true)
        {
            drawElementCallback = DrawEmoticonsReorderableListElement,
        };
    }

    private void DrawEmoticonsReorderableListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = serializedObject.FindProperty("Emoticons").GetArrayElementAtIndex(index);

        rect.width = (rect.width - 105.0f) / 3.0f;

        float diff = rect.height - EditorGUIUtility.singleLineHeight;

        rect.yMax -= diff / 2.0f;
        rect.yMin += diff / 2.0f;

        EditorGUI.PropertyField(rect, element.FindPropertyRelative("Sprite"), _emptyGUIContent);

        var lRect = rect;

        lRect.x += rect.width;
        lRect.width = 35.0f;

        EditorGUI.LabelField(lRect, _tagGUIContent);

        rect.x = lRect.x + lRect.width;

        EditorGUI.PropertyField(rect, element.FindPropertyRelative("Tag"), _emptyGUIContent);

        lRect = rect;

        lRect.x += rect.width;
        lRect.width = 40.0f;

        EditorGUI.LabelField(lRect, _charWidthGUIContent);

        rect.x = lRect.x + lRect.width;

        EditorGUI.PropertyField(rect, element.FindPropertyRelative("CharWidth"), _emptyGUIContent);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Text"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MessageFormat"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SenderColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MessageColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OpenPrivateChatWithSenderAfterClick"));

        if (GUILayout.Button("Load emoticons from spritesheets"))
        {
            EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", GUIUtility.GetControlID(FocusType.Passive));
        }

        string commandName = Event.current.commandName;
        if (commandName == "ObjectSelectorClosed")
        {
            var spritesheet = EditorGUIUtility.GetObjectPickerObject();

            if (spritesheet != null && spritesheet is Texture2D)
            {
                string spritesheetPath = AssetDatabase.GetAssetPath(spritesheet);
                var textureImporter = AssetImporter.GetAtPath(spritesheetPath) as TextureImporter;
                if (textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
                {
                    var eList = new List<ChatChannelMessageUI.Emoticon>();

                    var sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(spritesheetPath);

                    for (int i = 0; i < textureImporter.spritesheet.Length; i++)
                    {
                        var sprite = sprites.First(x => x is Sprite && x.name == textureImporter.spritesheet[i].name) as Sprite;

                        if (sprite != null)
                        {
                            eList.Add(new ChatChannelMessageUI.Emoticon()
                            {
                                CharWidth = 5,
                                Tag = ":" + textureImporter.spritesheet[i].name + ":",
                                Sprite = sprite
                            });
                        }
                    }

                    (target as ChatChannelMessageUI).Emoticons = eList.ToArray();
                    EditorUtility.SetDirty(target);
                    serializedObject.UpdateIfDirtyOrScript();
                }
                
            }
        }
        else
        {
            var emoticons = serializedObject.FindProperty("Emoticons");
            if (emoticons.arraySize < 100)
                _emoticonsReorderableList.DoList(EditorGUILayout.GetControlRect(GUILayout.Height(_emoticonsReorderableList.GetHeight()), GUILayout.ExpandWidth(true)));
            else
            {
                EditorGUILayout.HelpBox("More than 100 emoticons - using standard array drawer", MessageType.Warning);
                EditorGUILayout.PropertyField(emoticons, true);
            }

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }        
    }
}
