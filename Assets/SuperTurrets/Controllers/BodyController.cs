using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
/// <summary>
/// Controll turret body. Turret body has a limited rotation.
/// </summary>
[AddComponentMenu("OptimizedGuy/SuperTurrets/Body Controller")]
public class BodyController : MonoBehaviour 
{
	// Actor that manage this controller
	private ITurret 	turretActor;
	// Save original rotation so turret can return when idle
	private Quaternion 	originalRotation,lastFrameRotation;
	// Save transform for optimization
	private Transform	myTransform,parentTransform;
	private bool 		pointingEnemy;

	/// <summary>
	/// You may need to change this constant based on your game scale. For small objects you can decrease the umbral.
	/// </summary>
	private static readonly float POINTING_ENEMY_ANGLE_UMBRAL = 5f;
	
	// Use this for initialization
	void Awake ()
	{
		myTransform = transform;

		parentTransform = myTransform.parent;
		if(parentTransform == null)
			parentTransform = myTransform;

		originalRotation = myTransform.localRotation;
	}
	
	/// <summary>
	/// Main actor will call this method.
	/// </summary>
	/// <param name="actor">
	/// Turret actor where controller gets info,
	/// </param>
	public void SetTurretActor(ITurret actor)
	{
		turretActor = actor;

		if(turretActor.AutoDisableControllers())
			enabled	= false;
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		myTransform.localRotation = lastFrameRotation; // Small hack to overwrite animation transformations. We want to rotate cannons manually avoiding any animation rotation.

		if(!turretActor.IsIdle())
		{
			Quaternion targetLocalRotation 	= Quaternion.identity;

			if(turretActor.GetMode() == Mode.game3D)
				targetLocalRotation = Calculate3DRotation();
			else
				targetLocalRotation = Calculate2DRotation();
		
			if(turretActor.GetInterpolationType() == InterpolationType.LERP)
			{
				myTransform.localRotation = Quaternion.Lerp(myTransform.localRotation,targetLocalRotation,Time.deltaTime*turretActor.GetBaseRotationSpeed());
			}
			else if(turretActor.GetInterpolationType() == InterpolationType.TOWARDS)
			{
				myTransform.localRotation = Quaternion.RotateTowards(myTransform.localRotation,targetLocalRotation,Time.deltaTime*turretActor.GetBaseRotationSpeed());
			}
			else if(turretActor.GetInterpolationType() == InterpolationType.SLERP)
			{
				myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation,targetLocalRotation,Time.deltaTime*turretActor.GetBaseRotationSpeed());
			}

			float axisDiff = float.MaxValue;
			if(turretActor.GetMode() == Mode.game3D)
			{
				axisDiff = myTransform.localEulerAngles.y - targetLocalRotation.eulerAngles.y;
			}
			else
			{
				axisDiff = myTransform.localEulerAngles.z - targetLocalRotation.eulerAngles.z;
			}

			if(Mathf.Abs(axisDiff) < POINTING_ENEMY_ANGLE_UMBRAL)
				pointingEnemy = true;
			else
				pointingEnemy = false;

		}
		else
		{ // Return to original position
			myTransform.localRotation=Quaternion.Slerp(myTransform.localRotation,originalRotation,0.07f);	
			float angle = Quaternion.Angle(myTransform.localRotation,originalRotation);

			if(angle<1)
			{
				myTransform.localRotation = originalRotation;

				if(turretActor.AutoDisableControllers())
					enabled	= false;
			}
		}

		lastFrameRotation = myTransform.localRotation;
	}

	/// <summary>
	/// Calculates the 3D rotation. Turret model needs to look towards Z
	/// </summary>
	/// <returns>The D rotation.</returns>
	public Quaternion Calculate3DRotation()
	{
		Vector3 targetPosition = turretActor.GetTargetPosition();

		Vector3 localEuler = Quaternion.LookRotation(parentTransform.InverseTransformPoint(targetPosition),Vector3.up).eulerAngles;

		float yAxis = localEuler.y - 180f;
		if(yAxis < 0f)
			yAxis += 360f;
		
		// Clamping !
		if(!turretActor.ShootIgnoringAllConstraints) // Skip clamp if we have to shoot ignoring all conditions
			yAxis = Mathf.Clamp(yAxis,turretActor.GetMinHorizontalAngle(),turretActor.GetMaxHorizontalAngle());

		yAxis += 180f; // Remove offset before appliying it

		return Quaternion.Euler(new Vector3(originalRotation.x,yAxis,originalRotation.z));
	}

	/// <summary>
	/// Calculates the 2D rotation. Y axis is aligned with target
	/// </summary>
	/// <returns>The D rotation.</returns>
	public Quaternion Calculate2DRotation()
	{
		Vector3 targetPosition = turretActor.GetTargetPosition();
		Vector3 dir = targetPosition - myTransform.position;

		Quaternion auxRotation = myTransform.localRotation; // save rotation
		myTransform.up = dir; // Modify rotation changing up vector
		Vector3 localEuler = myTransform.localEulerAngles;
		myTransform.localRotation = auxRotation; // Restore rotation

		float zAxis = localEuler.z - 180f;
		if(zAxis < 0f)
			zAxis += 360f;
		
		// Clamping !
		if(!turretActor.ShootIgnoringAllConstraints) // Skip clamp if we have to shoot ignoring all conditions
			zAxis = Mathf.Clamp(zAxis,turretActor.GetMinHorizontalAngle(),turretActor.GetMaxHorizontalAngle());

		zAxis += 180f; // Remove offset before appliying it

		return Quaternion.Euler(new Vector3(originalRotation.x,originalRotation.y,zAxis));
	}

	/// <summary>
	/// Is this cannon approximately pointing to the target ?
	/// </summary>
	public bool IsPointingTarget()
	{
		return pointingEnemy;
	}
}
}
