using UnityEngine;

/// <summary>
/// A component that causes the GameObject it's attached to to orbit a specified object.
/// </summary>
public class OrbitAround : MonoBehaviour {

	[Tooltip("The target object to rotate around.")]
	public Transform ObjectToOrbit;

	[Tooltip("The number of degrees per second that the object will rotate around the target.")]
	public float OrbitSpeed = 1;

	private void Update() {
		this.transform.RotateAround(this.ObjectToOrbit.position, Vector3.up, this.OrbitSpeed * Time.deltaTime);
	}

}