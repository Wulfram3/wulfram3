using UnityEngine;

/// <summary>
/// A component that allows an attached camera to be rotated, by dragging the mouse while holding the left mouse button down.
/// </summary>
public class CameraDrag : MonoBehaviour {

	public Camera Camera;
	
	public float HorizontalSpeed = 2;
	public float VerticalSpeed = 2;
	
	private void Awake() {
		if (this.Camera == null) {
			this.Camera = Camera.main;
		}
	}
	
	private void LateUpdate() {
		if (Input.GetMouseButton(0)) {
			this.Camera.transform.Rotate(-Vector3.right * Time.deltaTime * Input.GetAxis("Mouse Y") * this.HorizontalSpeed);
			this.Camera.transform.Rotate(Vector3.up * Time.deltaTime * Input.GetAxis("Mouse X") * this.VerticalSpeed);
		}
	}

}