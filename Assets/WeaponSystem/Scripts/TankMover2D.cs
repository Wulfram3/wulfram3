using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class TankMover2D : MonoBehaviour
	{

		public float Speed = 20;
		public float TurnSpeed = 100;
		public WeaponController weapon;
	
		void Start ()
		{
			weapon = this.transform.GetComponentInChildren (typeof(WeaponController)).GetComponent<WeaponController> ();
		}

		void Update ()
		{
			if (Input.GetButton ("Fire1")) {
				if (weapon)
					weapon.LaunchWeapon ();
			}

			this.transform.position += Vector3.right * Input.GetAxis ("Horizontal") * Speed * Time.deltaTime;
		}
	}
}