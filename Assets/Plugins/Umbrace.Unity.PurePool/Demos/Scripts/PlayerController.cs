using System.Collections;

using Umbrace.Unity.PurePool;
using Umbrace.Unity.PurePool.UNet;

using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	[Tooltip("The networked object to be spawned when Spacebar is pressed.")]
	public GameObject SpawnObject;
	
	/// <inheritdoc />
	public override void OnStartClient() {
		base.OnStartClient();

		// Find the UNet integration component.
		var unet = Object.FindObjectOfType<UNetPooling>();

		// Register pooling support for the object to be spawned.
		unet.RegisterSpawnHandler(this.SpawnObject);
	}

	// Update is called once per frame
	private void Update () {
		if (!this.isLocalPlayer) return;

		float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150;
		float z = Input.GetAxis("Vertical") * Time.deltaTime * 3;

		this.transform.Rotate(0, x, 0);
		this.transform.Translate(0, 0, z);

		if (Input.GetKeyDown(KeyCode.Space)) {
			// Choose a position within the screen space.
			Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), Random.Range(10, 200)));

			this.Cmd_Spawn(position);
		}
	}

	[Command]
	private void Cmd_Spawn(Vector3 position) {
		if (NetworkServer.active) {
			// Instead of instantiating, we can take an object from the pool instead.
			//var plant = Object.Instantiate(this.SpawnObject, position, Random.rotation);
			var plant = GameObjectPoolManager.Instance.Acquire(this.SpawnObject, position, Random.rotation);
			NetworkServer.Spawn(plant);

			this.StartCoroutine(this.DestroyLater(plant, 2));
		}
	}

	public IEnumerator DestroyLater(GameObject go, float timer) {
		yield return new WaitForSeconds(timer);

		// Instead of destroying, we can release the object back to the pool.
		//Object.Destroy(go);
		GameObjectPoolManager.Instance.Release(go);
		NetworkServer.UnSpawn(go);
	}

}
