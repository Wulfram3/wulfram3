using Umbrace.Unity.Contracts;

using UnityEngine;

namespace Umbrace.Unity.PurePool {

	/// <summary>
	/// Provides static access to the <see cref="GameObjectPoolManager"/> found in the scene.
	/// </summary>
	public static class StaticPoolManager {
		
		#region Instance property.
		/// <inheritdoc cref="GameObjectPoolManager.Instance" />
		public static GameObjectPoolManager Instance {
			get { return GameObjectPoolManager.Instance; }
		}
		#endregion

		#region Acquire(GameObject) methods.
		/// <summary>
		/// Acquires an instance of <paramref name="sourceObject"/> from a pool.
		/// </summary>
		/// <param name="sourceObject">The game object to acquire an instance of.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(this GameObject sourceObject) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticPoolManager.Instance.CanAcquire(sourceObject));

			return StaticPoolManager.Instance.Acquire(sourceObject);
		}

		/// <summary>
		/// Acquires an instance of <paramref name="sourceObject"/> from a pool, and sets its parent transform.
		/// </summary>
		/// <param name="sourceObject">The game object to acquire an instance of.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(this GameObject sourceObject, Transform parent) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticPoolManager.Instance.CanAcquire(sourceObject));

			return StaticPoolManager.Instance.Acquire(sourceObject, parent);
		}

		/// <summary>
		/// Acquires an instance of <paramref name="sourceObject"/> from a pool, and sets its parent transform.
		/// </summary>
		/// <param name="sourceObject">The game object to acquire an instance of.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <param name="spawnInWorldSpace"><see langword="true"/> if the original world position should be maintained when assigning the parent; otherwise, <see langword="false"/>.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(this GameObject sourceObject, Transform parent, bool spawnInWorldSpace) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticPoolManager.Instance.CanAcquire(sourceObject));

			return StaticPoolManager.Instance.Acquire(sourceObject, parent, spawnInWorldSpace);
		}

		/// <summary>
		/// Acquires an instance of <paramref name="sourceObject"/> from a pool, and sets its position and rotation.
		/// </summary>
		/// <param name="sourceObject">The game object to acquire an instance of.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(this GameObject sourceObject, Vector3 position, Quaternion rotation) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticPoolManager.Instance.CanAcquire(sourceObject));

			return StaticPoolManager.Instance.Acquire(sourceObject, position, rotation);
		}

		/// <summary>
		/// Acquires an instance of <paramref name="sourceObject"/> from a pool, and sets its parent transform, position and rotation.
		/// </summary>
		/// <param name="sourceObject">The game object to acquire an instance of.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(this GameObject sourceObject, Vector3 position, Quaternion rotation, Transform parent) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticPoolManager.Instance.CanAcquire(sourceObject));

			return StaticPoolManager.Instance.Acquire(sourceObject, position, rotation, parent);
		}
		#endregion

		#region Release(GameObject) and Release(GameObject, float) methods.
		/// <summary>
		/// Releases an instance of a game object that was previously acquired using <see cref="Acquire(GameObject)"/> back to its pool.
		/// </summary>
		/// <param name="instance">The instance to release back to the pool.</param>
		public static void Release(this GameObject instance) {
			Contract.RequiresNotNull(instance, "instance");

			StaticPoolManager.Instance.Release(instance);
		}

		/// <summary>
		/// Releases an instance of a game object that was previously acquired using <see cref="Acquire(GameObject)"/> back to its pool, after a specified time delay.
		/// </summary>
		/// <param name="instance">The instance to release back to the pool.</param>
		/// <param name="delay">The period of time to wait before releasing the instance to the pool.</param>
		/// <remarks>
		/// The delay is measured in scaled time, and is therefore affected by <see cref="Time.timeScale"/>.
		/// </remarks>
		public static void Release(this GameObject instance, float delay) {
			Contract.RequiresNotNull(instance, "instance");

			StaticPoolManager.Instance.Release(instance, delay);
		}
		#endregion

	}

}