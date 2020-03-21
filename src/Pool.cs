using System;

namespace Dcrew.ObjectPool
{
    /// <summary>An object pool of <typeparamref name="T"/>. See <see cref="IPoolable"/></summary>
    public static class Pool<T> where T : class, new()
    {
        const int _defCap = 4;

        /// <summary>Returns the amount of free <typeparamref name="T"/> objects</summary>
        public static int Count { get; private set; }

        /// <summary>Returns the size of the pool in total</summary>
        public static int Size => _arr.Length;

        static T[] _arr = new T[0];

        /// <summary>Ensures there is <paramref name="size"/> amount of free <typeparamref name="T"/> objects</summary>
        public static void EnsureCount(int size)
        {
            if (Count >= size)
                return;
            lock (_arr)
            {
                SetArrSize(size);
                var n = size - Count;
                for (var i = 0; i < n; i++)
                    _arr[Count++] = new T();
            }
        }
        /// <summary>Expands the amount of <typeparamref name="T"/> objects by <paramref name="amount"/></summary>
        public static void ExpandSize(int amount)
        {
            lock (_arr)
            {
                SetArrSize(_arr.Length + amount);
                for (var i = 0; i < amount; i++)
                    _arr[Count++] = new T();
            }
        }

        /// <summary>Returns a free instance of <typeparamref name="T"/> and auto-expands if there's none available</summary>
        public static T Spawn()
        {
            T obj;
            lock (_arr)
            {
                if (Count == 0)
                    ExpandSize(_defCap); // This method also locks _arr, but the lock is http://en.wikipedia.org/wiki/Reentrant_mutex
                obj = _arr[--Count];
                _arr[Count] = default;
            }
            if (obj is IPoolable p)
                p.OnSpawn();
            return obj;
        }
        /// <summary>Frees <paramref name="obj"/> for use when <see cref="Spawn"/> is called
        /// Call <see cref="Free(T)"/> on <paramref name="obj"/> when you're done with it</summary>
        public static void Free(T obj)
        {
            lock (_arr)
            {
                if (Count == _arr.Length)
                    SetArrSize(_arr.Length + _defCap);
                _arr[Count++] = obj;
            }
            if (obj is IPoolable p)
                p.OnFree();
        }

        static void SetArrSize(int amount)
        {
            var newArr = new T[amount];
            Array.Copy(_arr, 0, newArr, 0, Count);
            _arr = newArr;
        }
    }
}