using System;
using System.Collections.Generic;
using UnityEngine;

namespace Warehouse.Services.Warehouse.Services
{
    public class PersistenceManager : MonoBehaviour
    {
        public static PersistenceManager Instance { get; private set; }

        private readonly Dictionary<Type, UnityEngine.Object> _persistentObjects = new();
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
#if UNITY_EDITOR
#else
            DontDestroyOnLoad(gameObject);
#endif
        }

        /// <summary>
        /// Registers object and marks it as persistent.
        /// </summary>
        public T Register<T>(T obj) where T : UnityEngine.Object
        {
            var type = typeof(T);

            if (_persistentObjects.TryGetValue(type, out var o))
            {
                Debug.LogWarning($"[PersistenceManager] Object type of {type} is already registered.");
                return (T) o;
            }
            _persistentObjects[type] = obj;

            switch (obj)
            {
                case Component component:
                    DontDestroyOnLoad(component.gameObject);
                    break;
                case GameObject go:
                    DontDestroyOnLoad(go);
                    break;
            }

            return obj;
        }

        /// <summary>
        /// Returns persistent object of type T if registered
        /// </summary>
        public T Get<T>() where T : UnityEngine.Object
        {
            if (_persistentObjects.TryGetValue(typeof(T), out var obj))
            {
                return (T) obj;
            }

            return null;
        }

        /// <summary>
        /// Returns persistent object or throws error if not found
        /// </summary>
        public T Require<T>() where T : UnityEngine.Object
        {
            var obj = Get<T>();

            if (obj == null)
            {
                throw new InvalidOperationException(
                    $"[PersistenceManager] No persistent object of type {typeof(T)} found"
                    );
            }

            return obj;
        }

        /// <summary>
        /// Removes object from registry (does not destroy object in process) 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Unregister<T>() where T : UnityEngine.Object
        {
            return _persistentObjects.Remove(typeof(T));
        }
        
        /// <summary>
        /// Destroys object from DontDestroyOnLoad and removes it from registry
        /// </summary>
        public bool DestroyPersistent<T>() where T : UnityEngine.Object
        {
            var obj = Get<T>();
            if (obj == null) return false;
            switch (obj)
            {
                case Component component:
                    DontDestroyOnLoad(component.gameObject);
                    break;
                case GameObject go:
                    DontDestroyOnLoad(go);
                    break;
            }
                
            return _persistentObjects.Remove(typeof(T));
        }
    }
}