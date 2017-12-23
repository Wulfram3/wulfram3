using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class Explosion : MonoBehaviour
	{
		public int Force;
		public int Radius;
		public AudioClip[] Sounds;
		public Vector3 ShakeForce = new Vector3(0,5,0);

		private void Start ()
		{
      		
		}
	
		private void OnEnable ()
		{
			CameraEffects.Shake (ShakeForce, this.transform.position);
			Vector3 explosionPos = transform.position;
			Collider[] colliders = Physics.OverlapSphere (explosionPos, Radius);
			if (Sounds.Length > 0) {
				AudioSource.PlayClipAtPoint (Sounds [Random.Range (0, Sounds.Length)], transform.position);
			}
			foreach (Collider hit in colliders) {
				if (hit.GetComponent<Rigidbody>()) {
					hit.GetComponent<Rigidbody>().AddExplosionForce (Force, explosionPos, Radius, 3.0f);
				}
			}
		}
	}
}