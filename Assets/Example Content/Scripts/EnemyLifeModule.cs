using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class EnemyLifeModule : LifeModule
{	
	new protected void Awake()
	{
		base.Awake();
	}
	
	new protected void Start()
	{
		base.Start();	
	}
	
	protected override void setMaxLife(float maxLife, bool fillLife)
	{
		base.setMaxLife(maxLife,fillLife);
	}
	
	public override float hurt (GameObject bullet, RaycastHit hit, float damage)
	{
		if(life>=0)
		{
			//Hurt actions
			addLife(-damage);
			if(life<=0)
			{
				Vector3 velocityVector = bullet.GetComponent<Rigidbody>().velocity.normalized;
				float velocityPow2 = bullet.GetComponent<Rigidbody>().velocity.sqrMagnitude;
				float force = bullet.GetComponent<Rigidbody>().mass;
				
				KillingObject.deadlyObjectForce = (velocityVector*velocityPow2)*force;
				KillingObject.deadlyObjectIsBig = false;
				KillingObject.deadlyObjectCollisionPoint = hit.point;
			}
		}
		return life;
	}
}
}