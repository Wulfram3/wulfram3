/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using PhotonChatUI;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(ChatPanelUI))]
public class ChatPanelUIEditor : Editor
{
    private AnimBool _isResizableFadeGroupAnim;

    private AnimBool _isResizableOrIsDraggableFadeGroupAnim;

    private AnimBool _useAnimatorFadeGroupAnim;

    private void OnEnable()
    {
        _isResizableFadeGroupAnim = new AnimBool(true);
        _isResizableFadeGroupAnim.valueChanged.AddListener(Repaint);

        _isResizableOrIsDraggableFadeGroupAnim = new AnimBool(true);
        _isResizableOrIsDraggableFadeGroupAnim.valueChanged.AddListener(Repaint);

        _useAnimatorFadeGroupAnim = new AnimBool(true);
        _useAnimatorFadeGroupAnim.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        var isDraggableProperty = serializedObject.FindProperty("IsDraggable");
        EditorGUILayout.PropertyField(isDraggableProperty);

        var isResizableProperty = serializedObject.FindProperty("IsResizable");
        EditorGUILayout.PropertyField(isResizableProperty);

        _isResizableFadeGroupAnim.target = isResizableProperty.boolValue;

        if (EditorGUILayout.BeginFadeGroup(_isResizableFadeGroupAnim.faded))
        {
            EditorGUI.indentLevel++;
            OnIsResizableFadeGroup();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        _isResizableOrIsDraggableFadeGroupAnim.target = isResizableProperty.boolValue || isDraggableProperty.boolValue;

        if (EditorGUILayout.BeginFadeGroup(_isResizableOrIsDraggableFadeGroupAnim.faded))
        {
            EditorGUI.indentLevel++;
            OnIsResizableOrIsDraggableFadeGroup();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        var useAnimatorProperty = serializedObject.FindProperty("UseAnimator");
        EditorGUILayout.PropertyField(useAnimatorProperty);

        _useAnimatorFadeGroupAnim.target = useAnimatorProperty.boolValue;

        if (EditorGUILayout.BeginFadeGroup(_useAnimatorFadeGroupAnim.faded))
        {
            EditorGUI.indentLevel++;
            OnUseAnimatorFadeGroup();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnIsResizableFadeGroup()
    {
        var resizeHandleAnchors = serializedObject.FindProperty("ResizeHandleAnchors");
        resizeHandleAnchors.intValue =
            (int)(ChatPanelUI.ResizeAnchor)EditorGUILayout.EnumMaskField("Resize Handle Anchors",
                (ChatPanelUI.ResizeAnchor)resizeHandleAnchors.intValue);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ResizeHandleSize"));
    }

    protected virtual void OnIsResizableOrIsDraggableFadeGroup()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RectConstrain"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RectMinSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RectMaxSize"));    }

    protected virtual void OnUseAnimatorFadeGroup()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OverrideAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsOpenedParameterName"));
    }
}
