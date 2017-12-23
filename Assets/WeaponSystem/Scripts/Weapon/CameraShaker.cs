using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HWRWeaponSystem
{
	public class CameraShaker : MonoBehaviour
	{
		public float ShakeMult = 0.1f;
		public float MaxDistance = 100;
		public Vector3 PositionShaker;
		public Vector3 ShakeMagnitude;
		private Vector3 positionTmp;

		void Start ()
		{
			CameraEffects.Shaker = this;
		}

		Vector3 forcePower;

		public void Shake (Vector3 power, Vector3 position)
		{
			PositionShaker = position;
			forcePower = -power;
		}

		void Update ()
		{
			forcePower = Vector3.Lerp (forcePower, Vector3.zero, Time.deltaTime * 5);	
			ShakeMagnitude = new Vector3 (Mathf.Cos (Time.time * 80) * forcePower.x, Mathf.Cos (Time.time * 80) * forcePower.y, Mathf.Cos (Time.time * 80) * forcePower.z);
		}
	}

	public static class CameraEffects
	{
		public static CameraShaker Shaker;

		public static void Shake (Vector3 power, Vector3 position)
		{
			if (Shaker != null)
				Shaker.Shake (power, position);
		}
	}

}