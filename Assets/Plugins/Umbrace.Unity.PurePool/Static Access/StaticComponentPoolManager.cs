using System;

using Umbrace.Unity.Contracts;

using UnityEngine;

namespace Umbrace.Unity.PurePool {

	/// <summary>
	/// Provides static access to the <see cref="ComponentPoolManager"/> found in the scene.
	/// </summary>
	public static class StaticComponentPoolManager {
		
		#region Instance property.
		/// <inheritdoc cref="ComponentPoolManager.Instance" />
		public static ComponentPoolManager Instance {
			get { return ComponentPoolManager.Instance; }
		}
		#endregion

		#region Acquire(Type) methods.
		/// <summary>
		/// Acquires an instance of <paramref name="componentType"/> from a pool.
		/// </summary>
		/// <param name="componentType">The component to acquire an instance of.</param>
		/// <returns>An instance of <paramref name="componentType"/> acquired from the pool.</returns>
		public static Component Acquire(this Type componentType) {
			Contract.RequiresNotNull(componentType, "componentType");
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(componentType));

			return StaticComponentPoolManager.Instance.Acquire(componentType);
		}

		/// <summary>
		/// Acquires an instance of <paramref name="componentType"/> from a pool, and sets its parent transform.
		/// </summary>
		/// <param name="componentType">The component to acquire an instance of.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <returns>An instance of <paramref name="componentType"/> acquired from the pool.</returns>
		public static Component Acquire(this Type componentType, Transform parent) {
			Contract.RequiresNotNull(componentType, "componentType");
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(componentType));

			return StaticComponentPoolManager.Instance.Acquire(componentType, parent);
		}
		
		/// <summary>
		/// Acquires an instance of <paramref name="componentType"/> from a pool, and sets its position and rotation.
		/// </summary>
		/// <param name="componentType">The component to acquire an instance of.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <returns>An instance of <paramref name="componentType"/> acquired from the pool.</returns>
		public static Component Acquire(this Type componentType, Vector3 position, Quaternion rotation) {
			Contract.RequiresNotNull(componentType, "componentType");
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(componentType));

			return StaticComponentPoolManager.Instance.Acquire(componentType, position, rotation);
		}

		/// <summary>
		/// Acquires an instance of <paramref name="componentType"/> from a pool, and sets its parent transform, position and rotation.
		/// </summary>
		/// <param name="componentType">The component to acquire an instance of.</param>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <returns>An instance of <paramref name="componentType"/> acquired from the pool.</returns>
		public static Component Acquire(this Type componentType, Transform parent, Vector3 position, Quaternion rotation) {
			Contract.RequiresNotNull(componentType, "componentType");
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(componentType));

			return StaticComponentPoolManager.Instance.Acquire(componentType, parent, position, rotation);
		}
		#endregion

		#region Acquire<T>() methods.
		/// <summary>
		/// Acquires an instance of the component <typeparamref name="T"/> from a pool.
		/// </summary>
		/// <typeparam name="T">The type of component to acquire an instance of.</typeparam>
		/// <returns>An instance of the component <typeparamref name="T"/> acquired from the pool.</returns>
		public static Component Acquire<T>() where T : Component {
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(typeof(T)));

			return StaticComponentPoolManager.Instance.Acquire<T>();
		}

		/// <summary>
		/// Acquires an instance of the component <typeparamref name="T"/> from a pool, and sets its parent transform.
		/// </summary>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <typeparam name="T">The type of component to acquire an instance of.</typeparam>
		/// <returns>An instance of the component <typeparamref name="T"/> acquired from the pool.</returns>
		public static Component Acquire<T>(Transform parent) where T : Component {
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(typeof(T)));

			return StaticComponentPoolManager.Instance.Acquire<T>(parent);
		}

		/// <summary>
		/// Acquires an instance of the component <typeparamref name="T"/> from a pool, and sets its position and rotation.
		/// </summary>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <typeparam name="T">The type of component to acquire an instance of.</typeparam>
		/// <returns>An instance of the component <typeparamref name="T"/> acquired from the pool.</returns>
		public static Component Acquire<T>(Vector3 position, Quaternion rotation) where T : Component {
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(typeof(T)));

			return StaticComponentPoolManager.Instance.Acquire<T>(position, rotation);
		}

		/// <summary>
		/// Acquires an instance of the component <typeparamref name="T"/> from a pool, and sets its parent transform, position and rotation.
		/// </summary>
		/// <param name="parent">The transform to which the instance should be parented.</param>
		/// <param name="position">The position to set the instance's transform to.</param>
		/// <param name="rotation">The rotation to set the instance's transform to.</param>
		/// <typeparam name="T">The type of component to acquire an instance of.</typeparam>
		/// <returns>An instance of the component <typeparamref name="T"/> acquired from the pool.</returns>
		public static Component Acquire<T>(Transform parent, Vector3 position, Quaternion rotation) where T : Component {
			Contract.Requires(StaticComponentPoolManager.Instance.CanAcquire(typeof(T)));

			return StaticComponentPoolManager.Instance.Acquire<T>(parent, position, rotation);
		}
		#endregion

		#region Release(Component) method.
		/// <summary>
		/// Releases an instance of a component that was previously acquired using <see cref="Acquire(Type)"/> back to its pool.
		/// </summary>
		/// <param name="instance">The instance to release back to the pool.</param>
		public static void Release(this Component instance) {
			Contract.RequiresNotNull(instance, "instance");

			StaticComponentPoolManager.Instance.Release(instance);
		}
		#endregion

	}

}