using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class TankMover : MonoBehaviour
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
			if (Input.GetButton ("Fire2")) {
				if (weapon)
					weapon.SwitchWeapon ();
			}
			this.transform.Rotate (new Vector3 (0, Input.GetAxis ("Horizontal") * TurnSpeed * Time.deltaTime, 0));
			this.transform.position += this.transform.forward * Input.GetAxis ("Vertical") * Speed * Time.deltaTime;
		}
	}
}