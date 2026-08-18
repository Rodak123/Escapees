using UnityEngine;

namespace Rodak.Utils.Singleton
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T instance;
        private static bool applicationIsQuitting = false;

        public static bool HasInstance => instance != null && !applicationIsQuitting;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting) return null;

                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                }

                if (instance == null)
                {
                    throw new SingletonUninitializedException($"SingletonMonoBehaviour of type {typeof(T).Name} was not initialized.");
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }

        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }
}