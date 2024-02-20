using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Sunflower.Core.Singleton
{
    public class SimpleMonoSingleton<T> : MonoBehaviour where T : SimpleMonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if(_instance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
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
