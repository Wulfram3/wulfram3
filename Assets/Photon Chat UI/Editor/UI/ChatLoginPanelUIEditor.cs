/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using PhotonChatUI;
using UnityEditor;

[CustomEditor(typeof(ChatLoginPanelUI))]
public class ChatLoginPanelUIEditor : ChatPanelUIEditor 
{
    protected override void OnUseAnimatorFadeGroup()
    {
        base.OnUseAnimatorFadeGroup();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoginResultParameterName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsLoginProcessingParameterName"));
    }
}
