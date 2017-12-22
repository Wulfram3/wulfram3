using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{
	public Vector3 axisRotation;

	public void Update()
	{
		transform.Rotate (axisRotation * Time.deltaTime,Space.Self);
	}
}
