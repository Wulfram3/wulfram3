using UnityEngine;

/// <summary>
/// A component that automatically rotates the GameObject it's attached to.
/// </summary>
public class AutoRotate : MonoBehaviour {

	public float HorizontalSpeed = 2.0f;
	public float VerticalSpeed = 2.0f;

	private void Update () {
		this.transform.Rotate(-Vector3.right * Time.deltaTime * this.HorizontalSpeed);
		this.transform.Rotate(Vector3.up * Time.deltaTime * this.VerticalSpeed);
	}

}