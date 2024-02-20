using UnityEngine;

namespace Client.Starter
{
    public class MonoSingletonDontDestroy<T> : MonoBehaviour where T : MonoSingletonDontDestroy<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
                        GameObject.DontDestroyOnLoad(obj);
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public virtual void Dispose()
        {
            Destroy(instance.gameObject);
        }
    }
}
