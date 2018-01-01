using Umbrace.Unity.Contracts;

using UnityEngine;

namespace Umbrace.Unity.PurePool {

	/// <summary>
	/// Provides static access to the <see cref="NamedGameObjectPoolManager"/> found in the scene.
	/// </summary>
	public static class StaticNamedPoolManager {
		
		#region Instance property.
		/// <inheritdoc cref="NamedGameObjectPoolManager.Instance" />
		public static NamedGameObjectPoolManager Instance {
			get { return NamedGameObjectPoolManager.Instance; }
		}
		#endregion

		#region Acquire(string) methods.
		/// <summary>
		/// Acquires an instance of the source object with the specified name, from a pool.
		/// </summary>
		/// <param name="sourceObject">The name of the game object to acquire an instance of.</param>
		/// <returns>The instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(string sourceObject) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticNamedPoolManager.Instance.CanAcquire(sourceObject));

			return StaticNamedPoolManager.Instance.Acquire(sourceObject);
		}

		/// <summary>
		/// Acquires an instance of the source object with the specified name from a pool, and sets its parent transform.
		/// </summary>
		/// <param name="sourceObject">The name of the game object to acquire an instance of.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(string sourceObject, Transform parent) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticNamedPoolManager.Instance.CanAcquire(sourceObject));

			return StaticNamedPoolManager.Instance.Acquire(sourceObject, parent);
		}

		/// <summary>
		/// Acquires an instance of the source object with the specified name from a pool, and sets its parent transform.
		/// </summary>
		/// <param name="sourceObject">The name of the game object to acquire an instance of.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <param name="spawnInWorldSpace"><see langword="true"/> if the original world position should be maintained when assigning the parent; otherwise, <see langword="false"/>.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(string sourceObject, Transform parent, bool spawnInWorldSpace) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticNamedPoolManager.Instance.CanAcquire(sourceObject));

			return StaticNamedPoolManager.Instance.Acquire(sourceObject, parent, spawnInWorldSpace);
		}

		/// <summary>
		/// Acquires an instance of the source object with the specified name from a pool, and sets its position and rotation.
		/// </summary>
		/// <param name="sourceObject">The name of the game object to acquire an instance of.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(string sourceObject, Vector3 position, Quaternion rotation) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticNamedPoolManager.Instance.CanAcquire(sourceObject));

			return StaticNamedPoolManager.Instance.Acquire(sourceObject, position, rotation);
		}

		/// <summary>
		/// Acquires an instance of the source object with the specified name from a pool, and sets its parent transform, position and rotation.
		/// </summary>
		/// <param name="sourceObject">The name of the game object to acquire an instance of.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <returns>An instance of <paramref name="sourceObject"/> acquired from the pool.</returns>
		public static GameObject Acquire(string sourceObject, Vector3 position, Quaternion rotation, Transform parent) {
			Contract.RequiresNotNull(sourceObject, "sourceObject");
			Contract.Requires(StaticNamedPoolManager.Instance.CanAcquire(sourceObject));

			return StaticNamedPoolManager.Instance.Acquire(sourceObject, position, rotation, parent);
		}
		#endregion

		#region Release(GameObject) extension method.
		// Uncomment the Release method if you are not using the StaticPoolManager script.

		///// <summary>
		///// Releases an instance of a game object that was previously acquired using <see cref="Acquire(string)"/> back to its pool.
		///// </summary>
		///// <param name="instance">The instance to release back to the pool.</param>
		//public static void Release(this GameObject instance) {
		//	Contract.RequiresNotNull(instance, "instance");

		//	StaticNamedPoolManager.Instance.Release(instance);
		//}
		#endregion

	}

}