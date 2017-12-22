using UnityEngine;
using System.Collections;
using System;

namespace OptimizedGuy
{
public abstract class LifeModule : MonoBehaviour 
{
	/// <summary>
	/// Store data when life reachs 0
	/// </summary>
	public class DeadInfo
	{
		public bool 	deadlyObjectIsBig;
		public Vector3 	deadlyObjectForce;
		public Vector3 deadlyObjectCollisionPoint;
	}
	
	public float life = 0;
	public float maxLife = 0;
	public float[] extraLifeByDifficult;
	
	//Stores the element that killed you!
	private DeadInfo killingObject;
	public DeadInfo KillingObject {
		get {
			return killingObject;
		}
		set {
			killingObject = value;
		}
	}
	
	protected void Awake()
	{
		KillingObject = new LifeModule.DeadInfo();
	}
	
	protected void Start()
	{
		setMaxLife(maxLife,true);		
	}
	
	public void addLife(float lifeQuantity)
	{
		life += lifeQuantity;
	}
		
	protected virtual void setMaxLife(float maxLife, bool fillLife)
	{
		if(extraLifeByDifficult.Length==0)
			throw new InvalidOperationException("All enemies must have extra life by each difficult level: "+gameObject.name);
		
		this.maxLife=maxLife;
		if(fillLife)
			life = maxLife;
	}
	
	public float getLife()
	{
		return life;	
	}
		
	public abstract float hurt(GameObject bullet, RaycastHit hit, float damage);
}
}
