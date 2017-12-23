using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class MouseLook2D : MonoBehaviour
	{
		public Camera CurrentCamera;
		public float MaxAimRange = 10000;
		public Vector3 AimPoint;
		public bool LockX, LockY, LockZ;
		public GameObject AimObject;

		void Start ()
		{
	
		}
	
		void Update ()
		{
			if (CurrentCamera == null) {
			
				CurrentCamera = Camera.main;
			
				if (CurrentCamera == null)
					CurrentCamera = Camera.current;
			}
		
			RaycastHit hit;
			var ray = CurrentCamera.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, MaxAimRange)) {
				AimPoint = hit.point;
				AimPoint = AxisLock (AimPoint);
				AimObject = hit.collider.gameObject;
			} else {
				AimPoint = ray.origin + (ray.direction * MaxAimRange);
				AimPoint = AxisLock (AimPoint);
				AimObject = null;
			}
			if (AimObject) {
				this.gameObject.transform.LookAt (AimObject.transform.position + AimPoint);
			} else {
				this.gameObject.transform.LookAt (AimPoint);
			}
		
		}
	
		public Vector3 AxisLock (Vector3 axis)
		{
			if (LockX)
				axis.x = this.transform.position.x;
		
			if (LockY)
				axis.y = this.transform.position.y;
		
			if (LockZ)
				axis.z = this.transform.position.z;
						
			return axis;
		}
	}
}
