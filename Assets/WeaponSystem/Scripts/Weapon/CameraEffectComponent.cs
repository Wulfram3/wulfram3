using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HWRWeaponSystem
{
	public class CameraEffectComponent : MonoBehaviour
	{
		private Vector3 positionTmp;
		public float MaxDistance = 100;
		public float ShakeMult = 0.2f;

		void Start ()
		{
			if (this.transform.parent) {
				positionTmp = this.transform.localPosition;
			} else {
				positionTmp = this.transform.position;
			}
		}

		void Update ()
		{
			float distance = Vector3.Distance (this.transform.position, CameraEffects.Shaker.PositionShaker);
			float damping = (1.0f / MaxDistance) * Mathf.Clamp (MaxDistance - distance, 0, MaxDistance);
			if (this.transform.parent) {
				this.transform.localPosition = positionTmp + (CameraEffects.Shaker.ShakeMagnitude * damping * ShakeMult);
			} else {
				this.transform.position = positionTmp + (CameraEffects.Shaker.ShakeMagnitude * damping * ShakeMult);
			}
		}
	}
}