using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
/// <summary>
/// Simple abstract class for enemies. Turrets need to know if enemy is dead to stop targeting and attacking it.
/// Note: Before a enemy is disabled or deleted from scene, you can have an enemy diying. This Enemy continues existing 
/// in the scene, but is already dead. IsDead method is useful for that kind of cases.
/// </summary>
public abstract class AbstractEnemy : MonoBehaviour
{
	//Example enum, delete if needed
	public enum Hazard
	{
		HARD = 2,
		NORMAL = 1,
		EASY = 0
	}
	
	// This method is the only dependencie with turrets
	public abstract bool 	IsDead();
	
	/// <summary>
	/// Use by selection algorithm. Delete this method if you want. I wont affect turret basic behavior
	/// </summary>
	public abstract float  	GetLife();
	
	/// <summary>
	/// Use by selection algorithm. Delete this method if you want. I wont affect turret basic behavior
	/// </summary>
	public abstract int  	GetHazard();
}
}