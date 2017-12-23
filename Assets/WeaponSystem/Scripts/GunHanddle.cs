using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class GunHanddle : MonoBehaviour
	{

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
			// You can access all weapon parameters by call GetCurrentWeapon
			// e.g. weapon.GetCurrentWeapon().Ammo.toString()
		}
	}
}