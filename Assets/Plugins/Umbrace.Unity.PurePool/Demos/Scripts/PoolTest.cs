using Umbrace.Unity.PurePool;

using UnityEngine;

public class PoolTest : MonoBehaviour {

	public GameObjectPoolManager PoolManager;
	public GameObject ObjectToAcquire;
	public int AcquirePerSecond = 10;

	private void Awake() {
		// Find the manager if one hasn't been specified.
		if (this.PoolManager == null) {
			this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
		}
	}

	private void Update() {
		// Acquire and release the object multiple times per second. This simulates and exaggerates many different types of objects being used each frame.
		for (int i = 0; i < this.AcquirePerSecond; i++) {
			var instance = this.PoolManager.Acquire(this.ObjectToAcquire);
			this.PoolManager.Release(instance);
		}
	}

}