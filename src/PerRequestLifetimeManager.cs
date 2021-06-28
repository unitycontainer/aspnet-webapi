using System;
using Unity.Lifetime;

namespace Unity.AspNet.WebApi
{
    public class PerRequestLifetimeManager : LifetimeManager,
                                             IInstanceLifetimeManager,
                                             IFactoryLifetimeManager,
                                             ITypeLifetimeManager
    {
        private readonly object _lifetimeKey = new object();

        /// <summary>
        /// Retrieves a value from the backing store associated with this lifetime policy.
        /// </summary>
        /// <returns>The desired object, or null if no such object is currently stored.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return UnityPerRequestHttpModule.GetValue(_lifetimeKey);
        }

        /// <summary>
        /// Stores the given value into the backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container"></param>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            UnityPerRequestHttpModule.SetValue(_lifetimeKey, newValue);
        }

        /// <summary>
        /// Removes the given object from the backing store.
        /// </summary>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            var disposable = GetValue() as IDisposable;

            disposable?.Dispose();

            UnityPerRequestHttpModule.SetValue(_lifetimeKey, null);
        }

        /// <summary>
        /// Creates clone
        /// </summary>
        /// <returns></returns>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerRequestLifetimeManager();
        }
    }
}
