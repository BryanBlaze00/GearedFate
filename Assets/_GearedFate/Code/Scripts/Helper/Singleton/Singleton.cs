namespace UnityUtils
{
    using UnityEngine;

    public class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        public static T TryGetInstance()
        {
            return HasInstance ? _instance : null;
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
