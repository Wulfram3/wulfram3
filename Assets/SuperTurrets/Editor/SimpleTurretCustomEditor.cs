using UnityEngine;
using System.Collections;
using UnityEditor;
using OptimizedGuy;

[CustomEditor(typeof(OptimizedGuy.SimpleTurret))]
[CanEditMultipleObjects()]
public class SimpleTurretCustomEditor : Editor {

	public SerializedProperty bodyTargetingVelocityProp,cannonTargetingVelocityProp,cannonMinVAngleProp,cannonMaxVAngleProp,cannonMaxHAngleProp,cannonMinHAngleProp,
	bodyControllerProp,cannonsNumberProp,interpolationTypeProp;

	void OnEnable()
	{
		bodyTargetingVelocityProp 	= serializedObject.FindProperty("bodyTargetingVelocity");
		cannonTargetingVelocityProp = serializedObject.FindProperty("cannonTargetingVelocity");
		cannonMinVAngleProp 		= serializedObject.FindProperty("cannonMinVAngle");
		cannonMaxVAngleProp 		= serializedObject.FindProperty("cannonMaxVAngle");
		cannonMinHAngleProp 		= serializedObject.FindProperty("cannonMinHAngle");
		cannonMaxHAngleProp 		= serializedObject.FindProperty("cannonMaxHAngle");
		bodyControllerProp			= serializedObject.FindProperty("bodyController");
		cannonsNumberProp			= serializedObject.FindProperty("cannonsNumber");
		interpolationTypeProp		= serializedObject.FindProperty("interpolationType");
	}

	public override void OnInspectorGUI()
	{
		SimpleTurret turret = target as SimpleTurret;

		serializedObject.Update();
		
		if (turret.bodyController == null)
			EditorGUILayout.HelpBox("You must assign a BodyController",MessageType.Error);

		// Assign required components
		if(serializedObject.targetObjects.Length == 1)
			EditorGUILayout.PropertyField(bodyControllerProp,new GUIContent("Body controller","Turret base, this part will move horizontally only"),false);
		
		if (turret.cannonControllers != null && turret.cannonControllers.Length == 0)
			EditorGUILayout.HelpBox("You must assign a CannonController at least",MessageType.Error);
		
		if(turret.cannonControllers != null && turret.cannonControllers.Length >= 1 )
		{
			bool bad = true;
			foreach (CannonController controller in turret.cannonControllers)
			{
				if (controller != null)
				{
					bad = false;
					break;
				}
			}
			
			if(bad)
				EditorGUILayout.HelpBox("You must assign a CannonController at least",MessageType.Error);
		}
		
		InspectorCannons(turret);
		
		EditorGUILayout.Separator();
		 
		// Velocities
		EditorGUILayout.Slider(bodyTargetingVelocityProp,0f,300f,"Base point velocity");
		EditorGUILayout.Slider(cannonTargetingVelocityProp ,0f,300f,"Cannon point velocity");
		
		// Cannon max angle
		EditorGUILayout.PrefixLabel("Vertical");
		EditorGUI.indentLevel++;
		EditorGUILayout.Slider(cannonMinVAngleProp,0f,360f,"Cannon min angle");
		EditorGUILayout.Slider(cannonMaxVAngleProp,0f,360f,"Cannon max angle");

		if(cannonMinVAngleProp.floatValue > cannonMaxVAngleProp.floatValue)
			cannonMinVAngleProp.floatValue = cannonMaxVAngleProp.floatValue;

		if(cannonMaxVAngleProp.floatValue < cannonMinVAngleProp.floatValue)
			cannonMaxVAngleProp.floatValue = cannonMinVAngleProp.floatValue;
		EditorGUI.indentLevel--;

		EditorGUILayout.PrefixLabel("Horizontal");
		EditorGUI.indentLevel++;
		EditorGUILayout.Slider(cannonMinHAngleProp,0f,360f,"Cannon min angle");
		EditorGUILayout.Slider(cannonMaxHAngleProp,0f,360f,"Cannon max angle");
		
		if(cannonMinHAngleProp.floatValue > cannonMaxHAngleProp.floatValue)
			cannonMinHAngleProp.floatValue = cannonMaxHAngleProp.floatValue;
		
		if(cannonMaxHAngleProp.floatValue < cannonMinHAngleProp.floatValue)
			cannonMaxHAngleProp.floatValue = cannonMinHAngleProp.floatValue;
		EditorGUI.indentLevel--;

		EditorGUILayout.PropertyField(interpolationTypeProp,new GUIContent("Interpolation Type"));

		serializedObject.ApplyModifiedProperties();
		
		if (GUI.changed)
			EditorUtility.SetDirty(turret);
	}
	
	public void OnSceneGUI()
	{
		SimpleTurret turret = target as SimpleTurret;

		
		// Cannon vertical angle
		if(turret.bodyController != null)
		{
			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,0.2f);

			// Vertical angle
			float radius 			= 5f;
			Vector3 center 			= turret.transform.position;
			Vector3 normal 			= turret.bodyController.transform.right*1;
			Vector3 from			= Quaternion.AngleAxis(turret.cannonMaxVAngle,normal) * turret.bodyController.transform.forward * -1;

			Handles.DrawSolidArc(center,normal,from,turret.cannonMinVAngle - turret.cannonMaxVAngle,radius);

			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,1f);
			Vector3 maxHandlePosition 	= center + (from*radius);
			float handleSize 			= HandleUtility.GetHandleSize(maxHandlePosition);
			turret.cannonMaxVAngle 		= Handles.ScaleValueHandle(turret.cannonMaxVAngle,maxHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,0f);
			turret.cannonMaxVAngle 		= Mathf.Clamp(turret.cannonMaxVAngle,turret.cannonMinVAngle,360f);

			Vector3 minHandlePosition 	= center + (Quaternion.AngleAxis(turret.cannonMinVAngle - turret.cannonMaxVAngle,normal) * from) * radius;
			handleSize 					= HandleUtility.GetHandleSize(minHandlePosition);
			turret.cannonMinVAngle 		= Handles.ScaleValueHandle(turret.cannonMinVAngle,minHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,1f);
			turret.cannonMinVAngle 		= Mathf.Clamp(turret.cannonMinVAngle,0f,turret.cannonMaxVAngle);

			// Horizontal angle
			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,0.2f);

			center 					= turret.transform.position;
			normal 					= turret.transform.up;
			from					= Quaternion.AngleAxis(turret.cannonMaxHAngle,normal) * turret.transform.forward * -1;

			Handles.DrawSolidArc(center,normal,from,turret.cannonMinHAngle - turret.cannonMaxHAngle,radius);

			Handles.color 				= new Color(Color.white.r,Color.white.b,Color.white.b,1f);
			maxHandlePosition 			= center + (from*radius);
			handleSize 					= HandleUtility.GetHandleSize(maxHandlePosition);
			turret.cannonMaxHAngle 		= Handles.ScaleValueHandle(turret.cannonMaxHAngle,maxHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,0f);
			turret.cannonMaxHAngle 		= Mathf.Clamp(turret.cannonMaxHAngle,turret.cannonMinHAngle,360f);

			minHandlePosition 			= center + (Quaternion.AngleAxis(turret.cannonMinHAngle - turret.cannonMaxHAngle,normal) * from) * radius;
			handleSize 					= HandleUtility.GetHandleSize(minHandlePosition);
			turret.cannonMinHAngle 		= Handles.ScaleValueHandle(turret.cannonMinHAngle,minHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,1f);
			turret.cannonMinHAngle 		= Mathf.Clamp(turret.cannonMinHAngle,0f,turret.cannonMaxHAngle);
		}
		
		// Manual Target
		/*Handles.BeginGUI();
			GUIContent content = new GUIContent("Manual target","If you set a manual target, turret will ignore all targetings criteria and will point to the manually asigned target");
			GUILayout.BeginArea(new Rect(Screen.width-320f,Screen.height-70f,300f,50f),content);
       		turret.manualTarget= EditorGUILayout.ObjectField("Manual target",turret.manualTarget,typeof( GameObject),true) as GameObject;
			GUILayout.EndArea();
        Handles.EndGUI();*/

		if (GUI.changed)
			EditorUtility.SetDirty(turret);
	}

	void InspectorCannons(SimpleTurret turret)
	{
		turret.cannonsExpanded = EditorGUILayout.Foldout( turret.cannonsExpanded,"Cannons");

		if(turret.cannonsExpanded)
		{
			EditorGUILayout.PropertyField(cannonsNumberProp,new GUIContent("Number of cannons"));
			//turret.cannonsNumber = EditorGUILayout.IntField("Number of cannons",turret.cannonsNumber);
			
			if(turret.cannonsNumber == 0)
				// 1 canon minimum
				turret.cannonsNumber = 1;
			
			if(turret.cannonControllers.Length != turret.cannonsNumber)
			{
				CannonController[] cannons  = new CannonController[turret.cannonsNumber];
				
				for (int x = 0; x< turret.cannonsNumber; x++)
				{
					if (turret.cannonControllers.Length > x)
						cannons[x] = turret.cannonControllers[x];
				}
				
				turret.cannonControllers = cannons;
			}
			
			if(serializedObject.targetObjects.Length != 1)
				return;

			for (int x =0; x < turret.cannonControllers.Length; x++)
			{
				turret.cannonControllers[x] = EditorGUILayout.ObjectField("Cannon "+x, turret.cannonControllers[x], typeof(CannonController),true) as CannonController;
			}
		}	
	}
}
