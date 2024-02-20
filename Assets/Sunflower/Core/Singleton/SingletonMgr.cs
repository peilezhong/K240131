using System;
using System.Collections.Generic;

namespace Sunflower.Core
{
    public class SingletonMgr: SimpleSingleton<SingletonMgr>
    {
        private readonly Stack<Type> _singletonStack = new();
        private readonly Dictionary<Type, ISingleton> _singletons = new ();
        private readonly Stack<Type> _singletonWithUpdate = new();
        
        public void Awake()
        {
            // foreach (Type type in this.singletonWithUpdate)
            // {
            //     if (this.singletons.ContainsKey(type) && type is ISingletonUpdate)
            //     {
            //         (this.singletons[type] as ISingletonAwake).Awake();
            //     }
            // }
        }

        public void Update()
        {
            foreach (Type type in this._singletonWithUpdate)
            {
                if (this._singletons.ContainsKey(type) && this._singletons[type] is ISingletonUpdate)
                {
                    (this._singletons[type] as ISingletonUpdate).Update();
                }
            }
        }

        public void AddSingleton(ISingleton singleton)
        {
            lock (this)
            {
                Type type = singleton.GetType();
                if (_singletons.ContainsKey(type))
                {
                    return;
                }
                
                if (singleton is ISingletonReverseDispose)
                {
                    _singletonStack.Push(type);
                }
                
                if (singleton is ISingletonUpdate)
                {
                    _singletonWithUpdate.Push(type);
                }
                _singletons[type] = singleton;
            }
            singleton.Register();
        }
        
        public void AddSingletons(ISingleton[] singletons)
        {
            foreach (var singleton in singletons)
            {
                AddSingleton(singleton);
            }
        }

        public T AddSingleton<T>() where T : ISingleton, new()
        {
            T singleton = new();
            AddSingleton(singleton);
            return singleton;
        }

        public T AddSingletonWithAwake<T>() where T : ISingleton, ISingletonAwake, new()
        {
            T singleton = new();
            singleton.Awake();
            Log.LogGreen($"Singleton {typeof(T).Name} Awake!");
            AddSingleton(singleton);
            return singleton;
        }
        
        public T AddSingletonWithAwake<T, A>(A a) where T : ISingleton, ISingletonAwake<A>, new()
        {
            T singleton = new();
            singleton.Awake(a);
            Log.LogGreen($"Singleton {typeof(T).Name} Awake!");
            AddSingleton(singleton);
            return singleton;
        }
        
        public T AddSingletonWithAwake<T, A, B>(A a, B b) where T : ISingleton, ISingletonAwake<A, B>, new()
        {
            T singleton = new();
            singleton.Awake(a, b);
            Log.LogGreen($"Singleton {typeof(T).Name} Awake!");
            AddSingleton(singleton);
            return singleton;
        }
        
        public T AddSingletonWithAwake<T, A, B, C>(A a, B b, C c) where T : ISingleton, ISingletonAwake<A, B, C>, new()
        {
            T singleton = new();
            singleton.Awake(a, b, c);
            Log.LogGreen($"Singleton {typeof(T).Name} Awake!");
            AddSingleton(singleton);
            return singleton;
        }
        
        public void Dispose()
        {
            base.Dispose();
            lock (this)
            {
                this._singletonWithUpdate.Clear();
                while (this._singletonStack.Count > 0)
                {
                    Type type = this._singletonStack.Pop();
                    if (this._singletons.ContainsKey(type))
                    {
                        this._singletons[type].Dispose();
                        this._singletons.Remove(type);
                    }
                }

                foreach (var kv in this._singletons)
                {
                    kv.Value.Dispose();
                }
                this._singletons.Clear();
            }
        }
    }
}