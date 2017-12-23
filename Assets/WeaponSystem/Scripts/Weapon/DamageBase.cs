using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class DamageBase : MonoBehaviour
	{
		public GameObject Effect;
		[HideInInspector]
		public GameObject Owner;
		public int Damage = 20;
		[HideInInspector]
		public ObjectPool objectPool;
		public string[] TargetTag = new string[1]{ "Enemy" };
		public string[] IgnoreTag;

		public bool DoDamageCheck (GameObject gob)
		{
			for (int i = 0; i < IgnoreTag.Length; i++) {
				if (IgnoreTag [i] == gob.tag)
					return false;
			}
			return true;
		}


		public void IgnoreSelf (GameObject owner)
		{
			if (GetComponent<Collider> () && owner) {
				if (owner.GetComponent<Collider> ())
					Physics.IgnoreCollision (GetComponent<Collider> (), owner.GetComponent<Collider> ());

				if (Owner.transform.root) {
					foreach (Collider col in Owner.transform.root.GetComponentsInChildren<Collider>()) {
						Physics.IgnoreCollision (GetComponent<Collider> (), col);
					}
				}
			}
		}
	}

	public struct DamagePack
	{
		public int Damage;
		public GameObject Owner;
	}
}