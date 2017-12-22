using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    public enum InterpolationType
    {
	    LERP, 		// Slow in fast out
	    SLERP, 		// Slow in and slow out (spherical interpolation)
	    TOWARDS,	// Linear interpolation, the speed is constant
    }

    /// <summary>
    /// This enum specifies the level of precission for visibility tests.
    /// None: 		No visibility test is done, if enemy is inside turret area, turret will attack it
    /// Simple: 	One ray is used to determine if turret can see enemy,
    /// Detailed: 	Each cannon launchs a ray to do a visibility test with the enemy.
    /// </summary>
    public enum VisibilityPrecissionLevel
    {
	    None,
	    Simple,
	    Detailed
    }

    // Depengind of the Mode we will use 3d physics or 2d
    public enum Mode
    {
	    game3D,
	    game2D
    }

    /// <summary>
    /// Basic functions needed for turrets components to comunicate with the turret.
    /// </summary>
    public interface ITurret
    {
	    /// <summary>
	    /// Return true if the turret is idle;
	    /// </summary>
	    bool IsIdle();

	    /// <summary>
	    /// Position to try to look at.
	    /// </summary>
	    Vector3 GetTargetPosition();

	    /// <summary>
	    /// Get interpolation type for turret's base and cannons.
	    /// </summary>
	    InterpolationType GetInterpolationType();

	    /// <summary>
	    /// Min angle from 0 to 360 where cannons can move vertically.
	    /// </summary>
	    float GetMinVerticalAngle();

	    /// <summary>
	    /// Max angle from 0 to 360 where cannons can move vertically.
	    /// </summary>
	    float GetMaxVerticalAngle();

	    /// <summary>
	    /// Min angle from 0 to 360 where base can move horizontally.
	    /// </summary>
	    float GetMinHorizontalAngle();

	    /// <summary>
	    /// Max angle from 0 to 360 where cannons can move horizontally.
	    /// </summary>
	    float GetMaxHorizontalAngle();

	    /// <summary>
	    /// Gets the base rotation speed.
	    /// </summary>
	    float GetBaseRotationSpeed();

	    /// <summary>
	    /// Gets the cannon rotation speed.
	    /// </summary>
	    float GetCannonRotationSpeed();

	    /// <summary>
	    /// Optimization wich disables base and cannon controllers if turret is idle. Must be false if you don't have any logic controling it.
	    /// </summary>
	    bool AutoDisableControllers();

	    /// <summary>
	    /// 3D or 2D ?
	    /// </summary>
	    Mode GetMode();

	    /// <summary>
	    /// Sets a shoting point ignoring all constraints. Useful for some circunstances where you want to skip all the logic and simple force the turret to shot to a given point.
	    /// A common scenario for this method can be cinematics.
	    /// </summary>
	    void SetShootingPointIgnoringAllConstraints(Vector3 _position, bool _toggle);

	    /// <summary>
	    /// The point to shot when shotIgnoringAllConstraints = true;
	    /// </summary>
	    Vector3 PointToShootIgnoringAllConstraints {get; set;}
	
	    /// <summary>
	    /// If true, the turret will shot to this point ignoring all constraints.
	    /// </summary>
	    bool ShootIgnoringAllConstraints { get; set; }
    }
}
