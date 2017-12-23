using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class CameraFollower : MonoBehaviour
	{

		public GameObject Target;
		public Vector3 Offset;
	
		void Start ()
		{
	
		}

		void Update ()
		{
		
			if (Target) {
				this.transform.position = Vector3.Lerp (this.transform.position, Target.transform.position + Offset, Time.deltaTime * 10);	
				this.transform.position += (CameraEffects.Shaker.ShakeMagnitude * 0.2f);
			}
	
		}
	}
}