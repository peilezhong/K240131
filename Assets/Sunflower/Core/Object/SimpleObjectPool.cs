using Sunflower.Core;
using System;
using System.Collections.Generic;

namespace Assets.Sunflower.Core
{
    public interface IRelease
    {
        public void Release();
    }
    public class SimpleObjectPool<T> where T : class, IRelease, new()
    {
        protected Stack<T> Pool = new Stack<T>();

        protected int MaxCount = 0;

        public int PoolCount
        {
            get { return Pool.Count; }
        }

        public SimpleObjectPool(int macCount)
        {
            MaxCount = macCount;
            for(int i = 0; i < MaxCount; i++)
            {
                Pool.Push(new T());
            }
        }

        public T Spawn()
        {
            if(Pool.Count > 0)
            {
                return Pool.Pop();
            }
            else
            {
                return new T();
            }
        }

        public void Recycl(T obj)
        {
            if (obj == null)
            {
                Log.Error("Recycl Obj failed,obj is null!");
                return;
            }
            obj.Release();
            Pool.Push(obj);
        }

        public void Clear()
        {
            Pool.Clear();
        }
    } 
}
