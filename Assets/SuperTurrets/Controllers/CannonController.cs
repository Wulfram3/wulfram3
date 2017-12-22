using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// Controll cannon targeting movement.
    /// </summary>
    [AddComponentMenu("OptimizedGuy/SuperTurrets/Cannon Controller")]
    public class CannonController : MonoBehaviour 
    {
	    private ITurret 	turretActor;
	    private Quaternion 	originalRotation,lastFrameRotation;
	    private bool 		pointingEnemy;

	
	    [HideInInspector]
	    public 	Transform 	myTransform,parentTransform;

	    /// <summary>
	    /// You may need to change this constant based on your game scale. For small objects you can decrease the umbral.
	    /// </summary>
	    private static readonly float POINTING_ENEMY_ANGLE_UMBRAL = 5f;

	    void Awake()
	    {
		    myTransform 		= transform;

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
		    turretActor=actor;

		    if(turretActor.AutoDisableControllers())
			    enabled	= false;
        }
	
	    // Update is called once per frame
	    void LateUpdate ()
	    {	
		    if(turretActor.GetMode() == Mode.game2D)
		    {
			    pointingEnemy = true;
			    enabled = false;
			    return;
		    }

		    myTransform.localRotation = lastFrameRotation; // Small hack to overwrite animation transformations. We want to rotate cannons manually avoiding any animation rotation.

		    if(!turretActor.IsIdle())
		    {
			    Vector3 targetPosition = turretActor.GetTargetPosition();

			    Quaternion noClampedTargetRotation 	= Quaternion.LookRotation(parentTransform.InverseTransformPoint(targetPosition),Vector3.up);
			    // Store not clamped target local euler angles
			    Vector3 localEuler 					= noClampedTargetRotation.eulerAngles;

			    // Apply an offset to axis and put angle between 0f and 360f
			    float xAxis = localEuler.x - 180f;
			    if(xAxis < 0f)
				    xAxis += 360f;

			    // Clamping !
			    if(!turretActor.ShootIgnoringAllConstraints) // Skip clamp if we have to shoot ignoring all conditions
				    xAxis = Mathf.Clamp(xAxis,turretActor.GetMinVerticalAngle(),turretActor.GetMaxVerticalAngle());

			    xAxis += 180f; // Remove offset before appliying it

			    Quaternion targetLocalRotation 	= Quaternion.Euler(new Vector3(xAxis,originalRotation.eulerAngles.y,originalRotation.eulerAngles.z));

			    if(turretActor.GetInterpolationType() == InterpolationType.LERP)
			    {
				    myTransform.localRotation = Quaternion.Lerp(myTransform.localRotation,targetLocalRotation,Time.deltaTime*turretActor.GetCannonRotationSpeed());
			    }
			    else if(turretActor.GetInterpolationType() == InterpolationType.TOWARDS)
			    {
				    myTransform.localRotation = Quaternion.RotateTowards(myTransform.localRotation,targetLocalRotation,Time.deltaTime*turretActor.GetCannonRotationSpeed());
			    }
			    else if(turretActor.GetInterpolationType() == InterpolationType.SLERP)
			    {
				    myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation,targetLocalRotation,Time.deltaTime*turretActor.GetCannonRotationSpeed());
			    }

			    float angleDiff = Mathf.Abs(myTransform.localEulerAngles.x - localEuler.x);
                if (angleDiff < POINTING_ENEMY_ANGLE_UMBRAL)
				    pointingEnemy = true;
			    else
				    pointingEnemy = false;

		    }
		    else
		    {
			    myTransform.localRotation=Quaternion.Slerp(myTransform.localRotation,originalRotation,0.07f);
			    float angle = Quaternion.Angle(transform.localRotation,originalRotation);
			
			    if(angle < 1f)
			    {
				    transform.localRotation = originalRotation;

				    if(turretActor.AutoDisableControllers())
					    enabled	= false;
			    }
		    }

		    lastFrameRotation = myTransform.localRotation;
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
