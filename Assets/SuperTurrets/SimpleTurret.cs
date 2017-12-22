using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// Simple turret without targeting AI or shooting logic. It will aim to a point if the required rotation is inside max and min angles.
    /// This turrets are designed for cases where you want to implement your own targeting logic or if you don't have any kind of targeting and you simply want to aim to a point in the space.
    /// </summary>
    public class SimpleTurret : MonoBehaviour, ITurret {

	    /// <summary>
	    /// Turret base moving velocity to target an enemy.
	    /// </summary>
	    public float 					bodyTargetingVelocity = 1f;
	
	    /// <summary>
	    /// Turret cannons moving velocity to target an enemy.
	    /// </summary>
	    public float 					cannonTargetingVelocity = 1f;

	    /// <summary>
	    /// Max vertical angle that cannon can handle to target an enemy. From 0 to 180.
	    /// </summary>
	    public float 					cannonMaxVAngle	= 180f;
	
	    /// <summary>
	    /// Min vertical angle that cannon can handle to target an enemy. From 0 to 180.
	    /// </summary>
	    public float 					cannonMinVAngle	= 90f;
	
	    /// <summary>
	    /// Max horizontal angle that cannon can handle to target an enemy. From 0 to 180.
	    /// </summary>
	    public float 					cannonMaxHAngle	= 270f;
	
	    /// <summary>
	    /// Min horizontal angle that cannon can handle to target an enemy. From 0 to 180.
	    /// </summary>
	    public float 					cannonMinHAngle	= 90f;

	    /// <summary>
	    /// The body controller.
	    /// </summary>
	    public BodyController			bodyController;	

	    /// <summary>
	    /// Array with all cannon info.
	    /// </summary>
	    public CannonController[] 	cannonControllers = new CannonController[0];

	    /// <summary>
	    /// Cannon and base rotation interpolation type, Lerp,Towards or Slerp.
	    /// </summary>
	    public InterpolationType		interpolationType;

	    /// <summary>
	    /// Position to aim
	    /// </summary>
	    public Vector3					TargetPosition {get;set;}

	    /// <summary>
	    /// 3D or 2D Mode ?
	    /// </summary>
	    public Mode 				mode;

	    // Public vars needed for custom editor
	    public int  cannonsNumber = 0;
	    public bool cannonsExpanded;

	    /// <summary>
	    /// The point to shot when shotIgnoringAllConstraints = true;
	    /// </summary>
	    public Vector3 PointToShootIgnoringAllConstraints {get; set;}
	
	    /// <summary>
	    /// If true, the turret will shot to this point ignoring all constraints.
	    /// </summary>
	    public bool ShootIgnoringAllConstraints {get; set;}

	
	    // Use this for initialization
	    void Awake () {
		    bodyController.SetTurretActor(this);

		    foreach(var cannon in cannonControllers)
			    cannon.SetTurretActor(this);
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }

	    /// <summary>
	    /// True if component is disabled
	    /// </summary>
	    public bool IsIdle ()
	    {
		    return enabled == false && !ShootIgnoringAllConstraints;
	    }

	    /// <summary>
	    /// Point to look at.
	    /// </summary>
	    public Vector3 GetTargetPosition ()
	    {
		    if(ShootIgnoringAllConstraints)
			    return PointToShootIgnoringAllConstraints;
		    else
			    return TargetPosition;
	    }

	    /// <summary>
	    /// Type of rotation interpolation.
	    /// </summary>
	    public InterpolationType GetInterpolationType ()
	    {
		    return interpolationType;
	    }

	    public float GetMinVerticalAngle ()
	    {
		    return cannonMinVAngle;
	    }

	    public float GetMaxVerticalAngle ()
	    {
		    return cannonMaxVAngle;
	    }

	    public float GetMinHorizontalAngle ()
	    {
		    return cannonMinHAngle;
	    }

	    public float GetMaxHorizontalAngle ()
	    {
		    return cannonMaxHAngle;
	    }

	    public float GetBaseRotationSpeed ()
	    {
		    return bodyTargetingVelocity;
	    }

	    public float GetCannonRotationSpeed ()
	    {
		    return cannonTargetingVelocity;
	    }

	    public bool AutoDisableControllers()
	    {
		    return false;
	    }

	    /// <summary>
	    /// Gets the center position of all cannons.
	    /// </summary>
	    public Vector3 GetCannonsPosition()
	    {
		    float x=0,y=0,z=0;
		    foreach (CannonController cannon in cannonControllers)
		    {
			    if(cannon == null)
				    continue;

			    Transform cannonTransform = cannon.transform;
			    x += cannonTransform.position.x;
			    y += cannonTransform.position.y;
			    z += cannonTransform.position.z;
		    }
		
		    float total = cannonControllers.Length;
		    return new Vector3(x/total,y/total,z/total);
	    }

	    public Mode GetMode()
	    {
		    return mode;
	    }

	    public void SetShootingPointIgnoringAllConstraints (Vector3 _position, bool _toggle)
	    {
		    PointToShootIgnoringAllConstraints 	= _position;
		    ShootIgnoringAllConstraints 		= _toggle;
	    }
    }
}
