using UnityEngine;
using UnityEditor;
using OptimizedGuy;
using UnityEditor.Animations;
using System.Collections.Generic;

[CustomEditor(typeof(OptimizedGuy.MecanimAnimationController),true)]
[CanEditMultipleObjects()]
public class AnimationControllerEditor : Editor
{
    public SerializedProperty animatorProp,deploySpeedProp, foldSpeedProp, selectedLayerIndexProp, 
        deployParameterIndexProp, foldParameterIndexProp, idleStateIndexProp, deployedStateIndexProp, 
        deployTriggerIndexProp, foldTriggerIndexProp;

    void OnEnable()
    {
        animatorProp    = serializedObject.FindProperty("animator");
        deploySpeedProp = serializedObject.FindProperty("deploySpeed");
        foldSpeedProp   = serializedObject.FindProperty("foldSpeed");
        selectedLayerIndexProp = serializedObject.FindProperty("selectedLayerIndex");
        deployParameterIndexProp = serializedObject.FindProperty("deployParameterIndex");
        foldParameterIndexProp = serializedObject.FindProperty("foldParameterIndex");
        idleStateIndexProp = serializedObject.FindProperty("idleStateIndex");
        deployedStateIndexProp = serializedObject.FindProperty("deployedStateIndex");
        deployTriggerIndexProp = serializedObject.FindProperty("deployTriggerIndex");
        foldTriggerIndexProp = serializedObject.FindProperty("foldTriggerIndex");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        MecanimAnimationController mecanimAnimationController = target as MecanimAnimationController;

        EditorGUILayout.PropertyField(animatorProp, new GUIContent("Animator"));

        Animator animator = mecanimAnimationController.animator;

        if (animator == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        AnimatorController runtimeAnimator = animator.runtimeAnimatorController as AnimatorController;

        if(runtimeAnimator == null) // Take care of override animators
        {
            AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (overrideController != null)
                runtimeAnimator = overrideController.runtimeAnimatorController as AnimatorController;
            else
            {
                EditorGUILayout.HelpBox("You must assign an AnimationController to the Animator component", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }

        // Layers
        List<string> layerNames = new List<string>();
        foreach(var layer in runtimeAnimator.layers)
        {
            layerNames.Add(layer.name);
        }

        if (layerNames.Count > 0)
        {
            DrawPopUp(layerNames, selectedLayerIndexProp, "Layer");
            mecanimAnimationController.baseLayer = layerNames[selectedLayerIndexProp.intValue];
        }

        // Fold/Deploy speed parameters.
        List<string> parameterNames = new List<string>();
        foreach(var parameter in runtimeAnimator.parameters)
        {
            if(parameter.type == AnimatorControllerParameterType.Float)
                parameterNames.Add(parameter.name);
        }

        if (parameterNames.Count > 0)
        {
            DrawPopUp(parameterNames, deployParameterIndexProp, "Deploy Speed Paramaters");
            mecanimAnimationController.deploySpeedParameterName = parameterNames[deployParameterIndexProp.intValue];
            EditorGUILayout.PropertyField(deploySpeedProp, new GUIContent("Deploy Speed"));

            DrawPopUp(parameterNames, foldParameterIndexProp, "Fold Speed Paramater");
            mecanimAnimationController.foldSpeedParameterName = parameterNames[foldParameterIndexProp.intValue];
            EditorGUILayout.PropertyField(foldSpeedProp, new GUIContent("Fold Speed"));
        }

        // Fold/Deploy Trigger parameters
        List<string> triggerNames = new List<string>();
        foreach (var parameter in runtimeAnimator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
                triggerNames.Add(parameter.name);
        }

        if (triggerNames.Count > 0)
        {
            DrawPopUp(triggerNames, deployTriggerIndexProp, "Deploy Trigger");
            mecanimAnimationController.deployTriggerName = triggerNames[deployTriggerIndexProp.intValue];

            DrawPopUp(triggerNames, foldTriggerIndexProp, "Fold Trigger");
            mecanimAnimationController.foldTriggerName = triggerNames[foldTriggerIndexProp.intValue];
        }


        // Idle/Deployed states
        List<string> stateNames = new List<string>();
        foreach (var state in runtimeAnimator.layers[mecanimAnimationController.selectedLayerIndex].stateMachine.states)
        {
            stateNames.Add(state.state.name);
        }

        if (stateNames.Count > 0)
        {
            DrawPopUp(stateNames, idleStateIndexProp, "Idle State");
            mecanimAnimationController.idleStateName = stateNames[idleStateIndexProp.intValue];
            DrawPopUp(stateNames, deployedStateIndexProp, "Deployed State");
            mecanimAnimationController.deployedStateName = stateNames[deployedStateIndexProp.intValue];
        }


        serializedObject.ApplyModifiedProperties();
    }

    void DrawPopUp(List<string> _names,SerializedProperty _property,string _title)
    {
        if (_property.hasMultipleDifferentValues)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        _property.intValue = EditorGUILayout.Popup(_title, _property.intValue, _names.ToArray());

        GUI.enabled = true;
    }
}
