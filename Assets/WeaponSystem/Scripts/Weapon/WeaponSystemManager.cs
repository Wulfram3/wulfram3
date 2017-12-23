using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class WeaponSystemManager : MonoBehaviour
	{
		void Awake ()
		{
			WeaponSystem.Pool = (ObjectPoolManager)GameObject.FindObjectOfType (typeof(ObjectPoolManager));	
			WeaponSystem.Finder = (FinderPool)GameObject.FindObjectOfType (typeof(FinderPool));	
		
		}

		void OnDestroy(){
			if (WeaponSystem.Pool) {
				WeaponSystem.Pool.ClearPool ();
			}
			if (WeaponSystem.Finder) {
				WeaponSystem.Finder.ClearTarget ();
			}
		}
	}
	
	public static class WeaponSystem
	{
		public static ObjectPoolManager Pool;
		public static FinderPool Finder;
	}
}