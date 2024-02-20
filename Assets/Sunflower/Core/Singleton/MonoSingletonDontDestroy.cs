using UnityEngine;

namespace Sunflower.Core.Singleton
{
    public class MonoSingletonDontDestroy<T> : MonoBehaviour where T : MonoSingletonDontDestroy<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
                        GameObject.DontDestroyOnLoad(obj);
                        _instance = obj.AddComponent<T>();
                        _instance.Awake();
                    }
                }
                return _instance;
            }
        }

        public virtual void Awake()
        {

        }

        public virtual void Dispose()
        {
            Destroy(_instance.gameObject);
        }
    }
}
