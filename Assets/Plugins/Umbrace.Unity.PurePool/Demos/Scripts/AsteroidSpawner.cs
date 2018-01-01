using System.Collections;

using Umbrace.Unity.PurePool;

using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {

	public Camera Camera;
	public GameObject Asteroid;
	public GameObjectPoolManager PoolManager;
	public float MinimumTime = 0.05f;
	public float MaximumTime = 1;

	private void Awake() {
		// Find the camera if one hasn't been specified.
		if (this.Camera == null) {
			this.Camera = Camera.main;
		}

		// Find the manager if one hasn't been specified.
		if (this.PoolManager == null) {
			this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
		}

		// Create the pool for the asteroids.
		this.PoolManager.CreatePool(new GameObjectPoolSettings {
			Source = this.Asteroid,
			InitialSize = 85,
			MaximumSize = 100,
			InitialiseOnStart = true,
			LogMessages = LogLevel.Information,
			DontDestroyOnLoad = false
		});
	}
	
	private void OnEnable() {
		// Start a coroutine that will spawn asteroids at varying intervals.
		this.StartCoroutine(this.SpawnOverTime());
	}

	private IEnumerator SpawnOverTime() {
		// Continuously spawn asteroids until the script is disabled.
		while (this.enabled) {
			// Wait for a random number of seconds between the minimum and maximum time.
			yield return new WaitForSeconds(Random.Range(this.MinimumTime, this.MaximumTime));

			// Spawn an asteroid.
			this.Spawn();
		}
	}

	private void Spawn() {
		// Choose a position within the screen space.
		Vector3 position = this.Camera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), Random.Range(10, 200)));

		// Acquire (pool-based replacement for Instantiate) an asteroid.
		this.PoolManager.Acquire(this.Asteroid, position, Random.rotation, this.transform);

		// The line above would originally have looked like this:
		//Object.Instantiate(this.Asteroid, position, Random.rotation, this.transform);
	}
	
}