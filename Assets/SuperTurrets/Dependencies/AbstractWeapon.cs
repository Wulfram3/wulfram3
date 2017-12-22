using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    public abstract class AbstractWeapon : MonoBehaviour
    {
	    /// <summary>
	    /// The exact point where bullets will spawn.
	    /// </summary>
	    public Transform 		shootPoint;

	    /// <summary>
	    /// True when weapon is ready for shot. When Shot is called, weapon will shot.
	    /// WeaponReady is a flag that will change depending on weapon realoding time, ammo, fire rate, etc.
	    /// Each frame this var is get to check if turret can shot.
	    /// </summary>
	    public bool WeaponReady {get; protected set; }

	    /// <summary>
	    /// Called to avoid dependencies with default Unity execution order.
	    /// </summary>
	    public abstract void ManualInitialization();

        /// <summary>
        ///	Your weapon must create a new bullet and shot it to the target
        /// _localHitOffset represents an offset from the target center. Is useful to avoid all bullets going towards the target center in case you don't use bullets with physics
        /// and use them with any kind of translation in Update method. See docs for detailed info. 
        /// </summary>
        public abstract void Shoot(GameObject target, Vector3 _direction, Vector3 _localHitOffset = default(Vector3));

        /// <summary>
        ///	Shot method to shot to a point instead of to a target. Useful for more straight forward scenarios where you simply want to shoot to a point.
        /// _localHitOffset represents an offset from the target center. Is useful to avoid all bullets going towards the target center in case you don't use bullets with physics
        /// and use them with any kind of translation in Update method. See docs for detailed info. 
        /// </summary>
        public abstract void Shoot(Vector3 _position, Vector3 _direction, Vector3 _localHitOffset = default(Vector3));
    }
}
