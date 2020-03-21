namespace Dcrew.ObjectPool
{
    /// <summary>An interface to implement if you need a <see cref="OnSpawn()"/> or <see cref="OnFree()"/> method for an object that is pooled using <see cref="Pool{T}"/>. NOT REQUIRED</summary>
    public interface IPoolable
    {
        /// <summary>Called when this object is spawned using <see cref="Pool{T}.Spawn"/></summary>
        void OnSpawn();
        /// <summary>Called when <see cref="Pool{T}.Free(T)"/> is called on this object</summary>
        void OnFree();
    }
}