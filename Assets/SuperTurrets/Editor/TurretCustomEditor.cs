using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using OptimizedGuy;

[CustomEditor(typeof(OptimizedGuy.SuperTurret))]
[CanEditMultipleObjects()]
public class TurretCustomEditor : Editor {

	public SerializedProperty bodyTargetingVelocityProp,cannonTargetingVelocityProp,cannonMinVAngleProp,cannonMaxVAngleProp,cannonMaxHAngleProp,cannonMinHAngleProp,
	bodyControllerProp,cannonsNumberProp,interpolationTypeProp,hasAnimationProp,hasTargetingAreaProp,tagsNumberProp,customTargetPriorityProp,targetPriorityProp,waitUntilCurrentTargetDiesProp,
	visibilityLevelProp,waitingTimeIfNotVisibleProp,skipNotVisibleTargetsProp,manualTargetProp,debugModeProp,mode, layerMaskProp, disableAnimatorWhenAttackingProp;

	GUIStyle titleStyle = null;

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
		hasAnimationProp			= serializedObject.FindProperty("hasAnimation");
		hasTargetingAreaProp		= serializedObject.FindProperty("hasTargetingArea");
		tagsNumberProp				= serializedObject.FindProperty("tagsNumber");
		customTargetPriorityProp	= serializedObject.FindProperty("customTargetPriority");
		targetPriorityProp			= serializedObject.FindProperty("targetPriority");
		waitUntilCurrentTargetDiesProp		= serializedObject.FindProperty("waitUntilCurrentTargetDies");
		visibilityLevelProp					= serializedObject.FindProperty("visibilityLevel");
		waitingTimeIfNotVisibleProp			= serializedObject.FindProperty("waitingTimeIfNotVisible");
		skipNotVisibleTargetsProp			= serializedObject.FindProperty("skipNotVisibleTargets");
		manualTargetProp			= serializedObject.FindProperty("manualTarget");
		debugModeProp				= serializedObject.FindProperty("debugMode");
		mode						= serializedObject.FindProperty("mode");
        layerMaskProp               = serializedObject.FindProperty("layerMask");
        disableAnimatorWhenAttackingProp = serializedObject.FindProperty("disableAnimatorWhenAttacking");

        titleStyle = new GUIStyle ();
		titleStyle.fontSize = 18;
		titleStyle.normal.textColor = Color.white;
		titleStyle.fontStyle = FontStyle.Bold;

		SuperTurret turret = target as SuperTurret;

		// Try to add automatically the animation controller.
		Animator animator = turret.GetComponent<Animator> ();

		if (animator == null)
			animator = turret.GetComponentInChildren<Animator> ();

		if(animator != null && turret.GetComponent<AnimationControllerAbstract>() == null)
		{
			MecanimAnimationController animationController = turret.gameObject.AddComponent<MecanimAnimationController>();
			animationController.animator = animator;
		}
	}

	public override void OnInspectorGUI()
	{
		SuperTurret turret = target as SuperTurret;

		serializedObject.Update();

		EditorGUILayout.LabelField ("Controllers",titleStyle);

		EditorGUILayout.PropertyField(mode,new GUIContent("Game type"),false);


        // Assign required components
        if (serializedObject.targetObjects.Length == 1)
		{
		    EditorGUILayout.PropertyField(bodyControllerProp,new GUIContent("Body controller","Turret base, this part will move horizontally only"),false);

			if (bodyControllerProp.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("You must assign a BodyController",MessageType.Error);
                ApplyAndExit();
                return;
            }
			
			if(turret.bodyController != null && turret.bodyController.transform == turret.transform)
			{
				EditorGUILayout.HelpBox("The BodyController and the turret can't be in the same Transform. BodyController must be in a children of SuperTurret",MessageType.Error);
                ApplyAndExit();
                return;
            }
		}
		if(turret.cannons != null && turret.cannons.Length >= 1 )
		{
			bool bad = true;
			foreach (var cannon in turret.cannons)
			{
				if (cannon.cannonController != null)
				{
					bad = false;
					break;
				}
			}
			
			if(bad)
				EditorGUILayout.HelpBox("You must assign a CannonController at least",MessageType.Error);
		}

		InspectorCannons(turret);
		
		if (turret.cannons != null && turret.cannons.Length == 0)
		{
			EditorGUILayout.HelpBox("You must assign a CannonController at least",MessageType.Error);
            ApplyAndExit();
            return;
        }
		
		EditorGUILayout.Separator();
		
		// Has animation ? 
		//turret.hasAnimation = EditorGUILayout.Toggle("Deploy animation ?",turret.hasAnimation);
		EditorGUILayout.PropertyField(hasAnimationProp,new GUIContent("Deploy animation ?"," The turret has an animation before start shoting?"));

		if(serializedObject.targetObjects.Length == 1){
			if(turret.hasAnimation)
			{
				if (turret.animationController == null)
					EditorGUILayout.HelpBox("You must assign an AnimationController",MessageType.Error);
				turret.animationController = EditorGUILayout.ObjectField("Animation controller: ",turret.animationController,typeof(AnimationControllerAbstract),true) as AnimationControllerAbstract;

                EditorGUILayout.PropertyField(disableAnimatorWhenAttackingProp, new GUIContent("Disable animator when deployed"));
			}
			else
				turret.animationController = null;
		}
			
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField ("Constraints",titleStyle);
		EditorGUILayout.Separator();
		// Targeting area
		
		EditorGUILayout.HelpBox("If you want your turret start targeting enemies before it can shot them, enable targeting area.",MessageType.Info);
		EditorGUILayout.PropertyField(hasTargetingAreaProp,new GUIContent("Targeting area ?"));
		
		InspectorTags(turret);
		 
		// Velocities
		EditorGUILayout.Slider(bodyTargetingVelocityProp,0f,300f,"Base point velocity");
		EditorGUILayout.Slider(cannonTargetingVelocityProp ,0f,300f,"Cannon point velocity");
		
		// CANNON ANGLES

		if(turret.GetMode() == Mode.game3D)
		{
			EditorGUILayout.PrefixLabel("Vertical Angles");
			EditorGUI.indentLevel++;
			EditorGUILayout.Slider(cannonMinVAngleProp,0f,360f,"Cannon min angle");
			EditorGUILayout.Slider(cannonMaxVAngleProp,0f,360f,"Cannon max angle");

			if(cannonMinVAngleProp.floatValue > cannonMaxVAngleProp.floatValue)
				cannonMinVAngleProp.floatValue = cannonMaxVAngleProp.floatValue;

			if(cannonMaxVAngleProp.floatValue < cannonMinVAngleProp.floatValue)
				cannonMaxVAngleProp.floatValue = cannonMinVAngleProp.floatValue;

            if (cannonMinVAngleProp.floatValue == cannonMaxVAngleProp.floatValue)
            {
                EditorGUILayout.HelpBox("You can't aim anything if the MIN and MAX angles are equal", MessageType.Error);
            }

			EditorGUI.indentLevel--;
		}

		EditorGUILayout.PrefixLabel("Horizontal Angle");
		EditorGUI.indentLevel++;
		EditorGUILayout.Slider(cannonMinHAngleProp,0f,360f,"Cannon min angle");
		EditorGUILayout.Slider(cannonMaxHAngleProp,0f,360f,"Cannon max angle");
		
		if(cannonMinHAngleProp.floatValue > cannonMaxHAngleProp.floatValue)
			cannonMinHAngleProp.floatValue = cannonMaxHAngleProp.floatValue;
		
		if(cannonMaxHAngleProp.floatValue < cannonMinHAngleProp.floatValue)
			cannonMaxHAngleProp.floatValue = cannonMinHAngleProp.floatValue;

        if (cannonMinHAngleProp.floatValue == cannonMaxHAngleProp.floatValue)
        {
            EditorGUILayout.HelpBox("You can't aim anything if the MIN and MAX angles are equal",MessageType.Error);
        }
        EditorGUI.indentLevel--;

		EditorGUILayout.PropertyField(interpolationTypeProp,new GUIContent("Interpolation Type"));

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField ("AI",titleStyle);
		EditorGUILayout.Separator();

        // Wait time if not visible
        EditorGUILayout.HelpBox("If current target can't be aimed, how much time turret will be targeting it before change to another target", MessageType.Info);
        EditorGUILayout.PropertyField(waitingTimeIfNotVisibleProp, new GUIContent("Wait time if can't aim"));

        // Targeting priority
        EditorGUILayout.LabelField("Custom target selection priority ?");
		//turret.customTargetPriority = EditorGUILayout.Toggle("Custom selection strategy",turret.customTargetPriority);
		EditorGUILayout.PropertyField(customTargetPriorityProp,new GUIContent("Custom Selection Strategy"));

		if (turret.customTargetPriority)
		{
			EditorGUILayout.Separator();

			if(!targetPriorityProp.hasMultipleDifferentValues)
			{
				EditorGUILayout.HelpBox("You can implement different selection strategies. By default, turret attack the first targeted object.",MessageType.Info);
				EditorGUILayout.PropertyField(targetPriorityProp,new GUIContent("Target selection strategy"));
			}
			
			// Change when a new target is availible
			EditorGUILayout.HelpBox("If not checked, turret will change current target when another target is availible if this new target has a higher priority according to target selection priority (when set).If checked, turret only will change target when the current one dies or when the wait timer runs out.",MessageType.Info);
			EditorGUILayout.PropertyField(waitUntilCurrentTargetDiesProp,new GUIContent("Wait ?"));
		}

		// Manual target
		EditorGUILayout.PropertyField(manualTargetProp,new GUIContent("Manual target"));

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Visibility", titleStyle);
        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(visibilityLevelProp,new GUIContent("Visibility accuaracy: "));
		
		if(turret.visibilityLevel != VisibilityPrecissionLevel.None)
		{	
			// Skip not visible targets
			EditorGUILayout.PropertyField(skipNotVisibleTargetsProp,new GUIContent("Skip not visible targets"));

            // Layermask for raycasting test
            EditorGUILayout.PropertyField(layerMaskProp, new GUIContent("Raycasting Layermask"));
		}
		
		// Debug info
		EditorGUILayout.PropertyField(debugModeProp,new GUIContent("Debug Info"));

		if(turret.debugMode)
		{
			GUILayout.BeginVertical("box");

			if(Application.isPlaying && !string.IsNullOrEmpty(turret.targetStatus))
				GUILayout.Label(turret.targetStatus+" "+turret.timeTargetNotReady);

			foreach(var tag in turret.targetTags)
			{
				GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

				GUIStyle redStyle = new GUIStyle();
				redStyle.normal.textColor = Color.red;

				GUIStyle selecteStyle = enemies.Length == 0 ? redStyle : new GUIStyle("label");
				GUILayout.Label("Enemies with tag: "+tag+"= "+enemies.Length,selecteStyle);
			}

			GUILayout.EndVertical();
		}

		serializedObject.ApplyModifiedProperties();
		
		if (GUI.changed)
			EditorUtility.SetDirty(turret);
	}
	
	public void OnSceneGUI()
	{
		SuperTurret turret = target as SuperTurret;
	
		// Set/unset targeting area to the turret
		TargetingArea(turret);
		AttackingArea(turret);
		// Minimum attack distance
		MinimumAttackDistance(turret);
		
		// Cannon vertical angle
		if(turret.bodyController != null)
		{
			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,0.2f);

			// Vertical angle
			float radius 			= turret.attackAreaRadius;
			Vector3 center 			= turret.transform.position;
			Vector3 normal 			= turret.bodyController.transform.right*1;
			Vector3 from			= Quaternion.AngleAxis(turret.cannonMaxVAngle,normal) * turret.bodyController.transform.forward * -1;

			if(turret.mode == Mode.game3D)
				Handles.DrawSolidArc(center,normal,from,turret.cannonMinVAngle - turret.cannonMaxVAngle,radius);

			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,1f);
			Vector3 maxHandlePosition 	= center + (from*radius);
			float handleSize 			= HandleUtility.GetHandleSize(maxHandlePosition);

			if(turret.mode == Mode.game3D)
				turret.cannonMaxVAngle 		= Handles.ScaleValueHandle(turret.cannonMaxVAngle,maxHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,0.15f);
			turret.cannonMaxVAngle 		= Mathf.Clamp(turret.cannonMaxVAngle,turret.cannonMinVAngle,360f);

			Vector3 minHandlePosition 	= center + (Quaternion.AngleAxis(turret.cannonMinVAngle - turret.cannonMaxVAngle,normal) * from) * radius;
			handleSize 					= HandleUtility.GetHandleSize(minHandlePosition);
			if(turret.mode == Mode.game3D)
				turret.cannonMinVAngle 		= Handles.ScaleValueHandle(turret.cannonMinVAngle,minHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,0.15f);
			turret.cannonMinVAngle 		= Mathf.Clamp(turret.cannonMinVAngle,0f,turret.cannonMaxVAngle);

			// Horizontal angle
			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,0.2f);

			center 						= turret.transform.position;

			if(turret.mode == Mode.game3D)
			{
				normal 					= turret.transform.up;
				from					= Quaternion.AngleAxis(turret.cannonMaxHAngle,normal) * turret.transform.forward * -1;
			}
			else
			{
				normal 					= turret.transform.forward;
				from					= Quaternion.AngleAxis(turret.cannonMaxHAngle,normal) * turret.transform.up * -1;
			}

			Handles.DrawSolidArc(center,normal,from,turret.cannonMinHAngle - turret.cannonMaxHAngle,radius);

			Handles.color 				= new Color(Color.white.r,Color.white.b,Color.white.b,1f);
			maxHandlePosition 			= center + (from*radius);
			handleSize 					= HandleUtility.GetHandleSize(maxHandlePosition);
			turret.cannonMaxHAngle 		= Handles.ScaleValueHandle(turret.cannonMaxHAngle,maxHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,0.15f);
			turret.cannonMaxHAngle 		= Mathf.Clamp(turret.cannonMaxHAngle,turret.cannonMinHAngle,360f);

			minHandlePosition 			= center + (Quaternion.AngleAxis(turret.cannonMinHAngle - turret.cannonMaxHAngle,normal) * from) * radius;
			handleSize 					= HandleUtility.GetHandleSize(minHandlePosition);
			turret.cannonMinHAngle 		= Handles.ScaleValueHandle(turret.cannonMinHAngle,minHandlePosition,Quaternion.identity,handleSize,Handles.SphereCap,0.15f);
			turret.cannonMinHAngle 		= Mathf.Clamp(turret.cannonMinHAngle,0f,turret.cannonMaxHAngle);
		}

        foreach(var cannon in turret.cannons)
        {
            Vector3 rotationForMode = turret.mode == Mode.game3D ? Vector3.zero : new Vector3(-90f, 0f, 0f);

            Handles.color = Color.green;
            if(cannon.recoil && cannon.recoilAnchor != null)
            {
                Handles.ArrowCap(0, cannon.recoilAnchor.position, cannon.recoilAnchor.rotation * Quaternion.Euler(rotationForMode) * Quaternion.Euler(0f,180f,0f), cannon.maxDistance);
            }

            Handles.color = Color.red;
            if (cannon.shootPoint != null)
            {
                Handles.ArrowCap(0, cannon.shootPoint.position, cannon.shootPoint.rotation * Quaternion.Euler(rotationForMode), 0.5f);
            }
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
	
	
	void InspectorCannons(SuperTurret turret)
	{
		//GUI.backgroundColor = new Color(168f,204f,182f,1f);
		turret.cannonsExpanded = EditorGUILayout.Foldout( turret.cannonsExpanded,"Cannons");

		if(turret.cannonsExpanded)
		{
			EditorGUILayout.PropertyField(cannonsNumberProp,new GUIContent("Number of cannons"));
			//turret.cannonsNumber = EditorGUILayout.IntField("Number of cannons",turret.cannonsNumber);
			
			if(turret.cannonsNumber == 0)
				// 1 canon minimum
				turret.cannonsNumber = 1;
			
			if(turret.cannons.Length != turret.cannonsNumber)
			{
				SuperTurret.CannonInfo[] cannons  = new SuperTurret.CannonInfo[turret.cannonsNumber];
				
				for (int x = 0; x< turret.cannonsNumber; x++)
				{
					if (turret.cannons.Length > x)
						cannons[x] = turret.cannons[x];
				}
				
				turret.cannons = cannons;
			}

            if (serializedObject.targetObjects.Length != 1)
            {
                ApplyAndExit();
                return;
            }

			for (int x =0; x < turret.cannons.Length; x++)
			{
				EditorGUI.indentLevel++;

				if(turret.cannons[x] == null)
				{
					turret.cannons[x] = new SuperTurret.CannonInfo();
				}

				turret.cannons[x].cannonController 	= EditorGUILayout.ObjectField(x+" Cannon", turret.cannons[x].cannonController, typeof(CannonController),true) as CannonController;
				turret.cannons[x].weaponPrefab 		= EditorGUILayout.ObjectField(x+" Weapon Prefab (Optional)", turret.cannons[x].weaponPrefab, typeof(AbstractWeapon),false) as AbstractWeapon;

				GUI.enabled = turret.cannons[x].weaponPrefab != null;
				turret.cannons[x].shootPoint 		= EditorGUILayout.ObjectField(x+" Shoot Point ", turret.cannons[x].shootPoint, typeof(Transform),true) as Transform;
                if (turret.cannons[x].shootPoint == null)
                {
                    EditorGUILayout.HelpBox("You must assign a transform to the Shoot Point to know where to instantiate the shots", MessageType.Error);
                }

                GUI.enabled = true;
                turret.cannons[x].recoil            = EditorGUILayout.Toggle(x+" Has recoil ?", turret.cannons[x].recoil);

                EditorGUI.indentLevel++;
                GUI.enabled = turret.cannons[x].recoil;

                turret.cannons[x].recoilAnchor      = EditorGUILayout.ObjectField("Recoil Transform " + x, turret.cannons[x].recoilAnchor, typeof(Transform), true) as Transform;
                turret.cannons[x].forwardSpeed      = EditorGUILayout.FloatField("Forward Speed", turret.cannons[x].forwardSpeed);
                turret.cannons[x].backwardsSpeed    = EditorGUILayout.FloatField("Backward Speed", turret.cannons[x].backwardsSpeed);
                turret.cannons[x].maxDistance       = EditorGUILayout.FloatField("Max distance", turret.cannons[x].maxDistance);
                EditorGUI.indentLevel--;

                GUI.enabled = true;

				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}
		}
		GUI.backgroundColor = Color.white;
	}
	
	void InspectorTags(SuperTurret turret)
	{
		GUI.backgroundColor = new Color(0f,0f,1f,0.2f);
		turret.tagsExpanded = EditorGUILayout.Foldout( turret.tagsExpanded,"Tags");

		if(turret.tagsExpanded)
		{
			EditorGUILayout.HelpBox("The turret will attack all enemies with a tag contained in your tag list. Wich target to attack first depends on your target selection strategy.",MessageType.Info);

			EditorGUILayout.PropertyField(tagsNumberProp,new GUIContent("Number of tags"));

            if (tagsNumberProp.hasMultipleDifferentValues)
            {
                ApplyAndExit();
                return;
            }

			foreach(var target in serializedObject.targetObjects) // Array in case of multiselection
			{
				SuperTurret myTarget = target as SuperTurret;

				if(myTarget.targetTags.Length != myTarget.tagsNumber)
				{
					string[] tags 		= new string[myTarget.tagsNumber];
					
					for (int x = 0; x< myTarget.tagsNumber; x++)
					{
						if (myTarget.targetTags.Length > x)
							tags[x] = myTarget.targetTags[x];
						else
							tags[x]	= "Untagged";
					}
					
					myTarget.targetTags = tags;
				}
			}

			for (int x =0; x < turret.targetTags.Length; x++)
			{
				turret.targetTags[x] = EditorGUILayout.TagField("Enemy Tag "+x,turret.targetTags[x]);

				// For some reason, the following lines throw an exception
//				SerializedProperty arrayProp = serializedObject.FindProperty("targetTags.Array.data["+x+"]");
//				
//				if(arrayProp != null && arrayProp.hasMultipleDifferentValues)
//					break;
//				
//				if(arrayProp == null)
//					continue;
//				
//				if(string.IsNullOrEmpty(arrayProp.stringValue))
//					arrayProp.stringValue = "Untagged";
//				
//				//arrayProp.stringValue = EditorGUILayout.TagField("Tag"+x,arrayProp.stringValue);
//				arrayProp.stringValue = EditorGUILayout.TextField("Tag"+x,arrayProp.stringValue);
			}
		}

        if (tagsNumberProp.intValue == 0)
        {
            EditorGUILayout.HelpBox("If no tags are added, this turret will shoot to everything", MessageType.Warning);
        }


        GUI.backgroundColor = Color.white;
	}
	
	/// <summary>
	/// Control the minimum distance of turret to attack.
	/// </summary>
	/// <param name='turret'>
	/// Turret.
	/// </param>
	void MinimumAttackDistance(SuperTurret turret)
	{
		Handles.color = new Color(Color.blue.r,Color.blue.g,Color.blue.b,1f);
		
		if (turret.minimumDistance == 0)
		{
			if (turret.bodyController != null)
			{
				float width = turret.attackAreaRadius / 4f;
				
				turret.minimumDistance = width;
			}
		}

		Vector3 handlePosition = turret.transform.position+(Vector3.right*turret.minimumDistance);
		 turret.minimumDistance =   Handles.ScaleValueHandle(turret.minimumDistance,
					handlePosition,
                    turret.transform.rotation,
		            HandleUtility.GetHandleSize(handlePosition),
                    Handles.SphereCap,
                    2);

		handlePosition = turret.transform.position+(Vector3.left*turret.minimumDistance);
		 turret.minimumDistance =   Handles.ScaleValueHandle(turret.minimumDistance,
                    handlePosition,
                    turret.transform.rotation,
					HandleUtility.GetHandleSize(handlePosition),
                    Handles.SphereCap,
                    2);
		
		turret.minimumDistance = Mathf.Clamp(turret.minimumDistance,0f,turret.attackAreaRadius);

		Vector3 upVector = turret.mode == Mode.game3D ? turret.transform.up : turret.transform.forward;
		
		//turret.minimumDistance = Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,turret.minimumDistance,false);
		Handles.color = new Color(Color.blue.r,Color.blue.g,Color.blue.b,0.05f);
		Handles.DrawSolidDisc(turret.transform.position,upVector,turret.minimumDistance);
		Handles.color = new Color(Color.blue.r,Color.blue.g,Color.blue.b,1f);
		
		Handles.Label(turret.transform.position+(Vector3.left*turret.minimumDistance),"Minimum distance");
		Handles.Label(turret.transform.position+(Vector3.right*turret.minimumDistance),"Minimum distance");
	}

	/// <summary>
	/// Create an attacking area the first time and manage attacking area radius.
	/// </summary>
	/// <param name='turret'>
	/// Turret.
	/// </param>
	void AttackingArea(SuperTurret turret)	
	{
		Handles.color = Color.red;

		//turret.attackAreaRadius = Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,turret.attackAreaRadius,true);
		Vector3 handlePosition = turret.transform.position + turret.transform.right * turret.attackAreaRadius;
		turret.attackAreaRadius = Handles.ScaleValueHandle(turret.attackAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);

		handlePosition = turret.transform.position + turret.transform.right * turret.attackAreaRadius*-1;
		turret.attackAreaRadius = Handles.ScaleValueHandle(turret.attackAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);

		if(turret.mode == Mode.game3D)
		{
			handlePosition = turret.transform.position + turret.transform.forward * turret.attackAreaRadius;
			turret.attackAreaRadius = Handles.ScaleValueHandle(turret.attackAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);

			handlePosition = turret.transform.position + turret.transform.forward * turret.attackAreaRadius*-1;
			turret.attackAreaRadius = Handles.ScaleValueHandle(turret.attackAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);
		}
		
		// Attack area must be always major than minimum distance and minnor than tagreting area
		if (turret.hasTargetingArea)
			turret.attackAreaRadius = Mathf.Clamp(turret.attackAreaRadius,turret.minimumDistance,turret.targetingAreaRadius);
		else
			turret.attackAreaRadius = Mathf.Clamp(turret.attackAreaRadius,turret.minimumDistance,float.MaxValue);
			

		Handles.color = new Color(Color.red.r,Color.red.g,Color.red.b,0.06f);
		Vector3 upVector = turret.mode == Mode.game3D ? turret.transform.up : turret.transform.forward;
		Handles.DrawSolidDisc(turret.transform.position,upVector,turret.attackAreaRadius);
	
		//Handles.Label(turret.transform.position+(Vector3.up*turret.attackAreaRadius),"Attacking Area");
		Handles.Label(turret.transform.position+(Vector3.left*turret.attackAreaRadius),"Attacking Area");
		Handles.Label(turret.transform.position+(Vector3.right*turret.attackAreaRadius),"Attacking Area");
		//Handles.Label(turret.transform.position+(Vector3.down*turret.attackAreaRadius),"Attacking Area");
	}
	
	/// <summary>
	/// Add or remove targeting area to the turret.
	/// </summary>
	/// <param name='turret'>
	/// Turret.
	/// </param>
	void TargetingArea (SuperTurret turret)
	{
		if(turret.hasTargetingArea)
		{
			Handles.color = Color.yellow;
			
			//turret.targetingAreaRadius = Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,turret.targetingAreaRadius,true);

			Vector3 handlePosition = turret.transform.position + turret.transform.right * turret.targetingAreaRadius;
			turret.targetingAreaRadius = Handles.ScaleValueHandle(turret.targetingAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);
			
			handlePosition = turret.transform.position + turret.transform.right * turret.targetingAreaRadius*-1;
			turret.targetingAreaRadius = Handles.ScaleValueHandle(turret.targetingAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);
			
			handlePosition = turret.transform.position + turret.transform.forward * turret.targetingAreaRadius;
			turret.targetingAreaRadius = Handles.ScaleValueHandle(turret.targetingAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);
			
			handlePosition = turret.transform.position + turret.transform.forward * turret.targetingAreaRadius*-1;
			turret.targetingAreaRadius = Handles.ScaleValueHandle(turret.targetingAreaRadius,handlePosition,turret.transform.rotation,HandleUtility.GetHandleSize(handlePosition)*0.4f,Handles.DotCap,0f);

			// Targeting area must be always major than attack area
			turret.targetingAreaRadius = Mathf.Clamp(turret.targetingAreaRadius,turret.attackAreaRadius,float.MaxValue);
			
			Handles.color = new Color(Color.yellow.r,Color.yellow.g,Color.yellow.b,0.05f);
			Vector3 upVector = turret.mode == Mode.game3D ? turret.transform.up : turret.transform.forward;
			Handles.DrawSolidDisc(turret.transform.position,upVector,turret.targetingAreaRadius);
			
			//Handles.Label(turret.transform.position+(Vector3.up*turret.targetingAreaRadius),"Targeting Area");
			Handles.Label(turret.transform.position+(Vector3.left*turret.targetingAreaRadius),"Targeting Area");
			Handles.Label(turret.transform.position+(Vector3.right*turret.targetingAreaRadius),"Targeting Area");
			//Handles.Label(turret.transform.position+(Vector3.down*turret.targetingAreaRadius),"Targeting Area");
		}
	}

	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset(System.Type _type)
	{
		ScriptableObject asset = ScriptableObject.CreateInstance(_type);
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + _type.ToString() + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("Tools/OptimizedGuy/Create Custom Target Priority Scriptable Object",true)]
	public static bool CreateTargetPriorityValidation ()
	{
		MonoScript targetPriorityScript = UnityEditor.Selection.activeObject as MonoScript;

		return targetPriorityScript.GetClass().BaseType == typeof(AbstractTargetPriority);
	}

	[MenuItem("Tools/OptimizedGuy/Create Custom Target Priority Scriptable Object")]
	public static void CreateTargetPriority ()
	{
		MonoScript targetPriorityScript = UnityEditor.Selection.activeObject as MonoScript;

		if (targetPriorityScript.GetClass().BaseType == typeof(AbstractTargetPriority))
		{
			CreateAsset(targetPriorityScript.GetClass());
		}
		else
		{
			Debug.LogError("You must select the script which inherits from AbstractTargetPriority");
		}
	}

    void ApplyAndExit()
    {
        serializedObject.ApplyModifiedProperties();
    }
}
