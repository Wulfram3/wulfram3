using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class MoverBullet : WeaponBase
	{
		public int Lifetime;
		public float Speed = 80;
		public float SpeedMax = 80;
		public float SpeedMult = 1;
		private float speedTemp;
		private Rigidbody rigidBody;
	
		private void Awake ()
		{
			speedTemp = Speed;
			objectPool = this.GetComponent<ObjectPool> ();
			rigidBody = this.GetComponent<Rigidbody> ();
		}
	
		private void Start ()
		{
			if (objectPool && WeaponSystem.Pool != null) {
				objectPool.SetDestroy (Lifetime);
			} else {
				Destroy (gameObject, Lifetime);
			}
		}
	
		public void OnEnable ()
		{
			Speed = speedTemp;
			if (objectPool && WeaponSystem.Pool != null) {
				objectPool.SetDestroy (Lifetime);
			}
		}

		private void FixedUpdate ()
		{
			if (WeaponSystem.Pool != null && objectPool != null && !objectPool.Active) 
				return;
		
			if (!rigidBody)
				return;
		
			if (!RigidbodyProjectile) {
				rigidBody.velocity = transform.forward * Speed;
			} else {
				if (rigidBody.velocity.normalized != Vector3.zero)
					this.transform.forward = rigidBody.velocity.normalized;	
			}
			if (Speed < SpeedMax) {
				Speed += SpeedMult * Time.fixedDeltaTime;
			}
		}
	}
}
