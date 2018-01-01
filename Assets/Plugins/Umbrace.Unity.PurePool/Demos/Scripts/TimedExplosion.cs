using System.Collections;

using Umbrace.Unity.PurePool;

using UnityEngine;

public class TimedExplosion : MonoBehaviour, IPoolable {

	public GameObject Explosion;
	public GameObjectPoolManager PoolManager;
	public float MinimumTime = 5;
	public float MaximumTime = 15;
	public bool Pooled = true;
	
	private void Awake() {
		// Find the manager if one hasn't been specified.
		if (this.PoolManager == null) {
			this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
		}
	}

	private void Start() {
		// Most code that usually goes in the Start method should be placed in the IPoolable.Acquire method.
		// Start is only called once during the lifetime of the object, but we need to re-run this code every time the object is taken from the pool.

		// If this object is not being taken from a pool (i.e. it was placed in the scene in the Editor), start the coroutine to explode it here.
		if (!this.Pooled) {
			this.StartCoroutine(this.ExplodeLater());
		}
	}

	private IEnumerator ExplodeLater() {
		// Wait for a random number of seconds between the minimum and maximum time.
		yield return new WaitForSeconds(Random.Range(this.MinimumTime, this.MaximumTime));

		// Acquire (pool-based replacement for Instantiate) an explosion at the same position as the asteroid.
		this.PoolManager.Acquire(this.Explosion, this.transform.position, this.transform.rotation);

		// Release (pool-based replacement for Destroy) the asteroid.
		this.PoolManager.Release(this.gameObject);

		// The two lines above would originally have looked like this:
		//Object.Instantiate(this.Explosion, this.transform.position, this.transform.rotation);
		//Object.Destroy(this.gameObject);
	}

	void IPoolable.Acquire() {
		// Start a coroutine that will cause an explosion at a later point in time.
		this.StartCoroutine(this.ExplodeLater());
	}

	void IPoolable.Release() {
		// Do nothing.
	}

}