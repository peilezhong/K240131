using System;
using System.Collections.Generic;
using System.Linq;
using Sunflower.Core;
using UnityEngine;

namespace Sunflower.UI
{
    public class UIMgr: Singleton<UIMgr>, ISingletonAwake
    {
        private Dictionary<Type, BasePanle> panels = new();
        
        private Dictionary<UILayer, Stack<BasePanle>> panelStack = new();
        
        
        public void Awake()
        {
            
        }

        // private GameObject CreatePanelGameObject(Type type)
        // {
        //     object[] objs = type.GetCustomAttributes(typeof(UIAttribute), true);
        //     UIAttribute attr = (objs.First() as UIAttribute);
        //     // GameObject.Instantiate()
        // }

        public T Open<T>() where T : BasePanle, new ()
        {
            if (this.panels.ContainsKey(typeof(T)))
            {
                T cache = this.panels[typeof(T)] as T;
                if (cache is IPanelReset)
                {
                    (cache as IPanelReset).Reset();
                }
                return cache;
            }
            T p = new T();
            if (p is IPanelInit)
            {
                (p as IPanelInit).Init();
            }
            if (p is IPanelReset)
            {
                (p as IPanelReset).Reset();
            }
            this.panels.Add(typeof(T), p);
            return p;
        }
        
        public T Open<T, A>(A a) where T : BasePanle, new ()
        {
            if (this.panels.ContainsKey(typeof(T)))
            {
                T cache = this.panels[typeof(T)] as T;
                if (cache is IPanelReset<A>)
                {
                    (cache as IPanelReset<A>).Reset(a);
                }
                return cache;
            }
            T p = new T();
            if (p is IPanelInit<A>)
            {
                (p as IPanelInit<A>).Init(a);
            }
            return p;
        }
        
        public T Open<T, A, B>(A a, B b) where T : BasePanle, new ()
        {
            if (this.panels.ContainsKey(typeof(T)))
            {
                T cache = this.panels[typeof(T)] as T;
                if (cache is IPanelReset<A, B>)
                {
                    (cache as IPanelReset<A, B>).Reset(a, b);
                }
                return cache;
            }
            T p = new T();
            if (p is IPanelInit<A, B>)
            {
                (p as IPanelInit<A, B>).Init(a, b);
            }
            return p;
        }
        
        public T Open<T, A, B, C>(A a, B b, C c) where T : BasePanle, new ()
        {

            if (this.panels.ContainsKey(typeof(T)))
            {
                T cache = this.panels[typeof(T)] as T;
                if (cache is IPanelReset<A, B, C>)
                {
                    (cache as IPanelReset<A, B, C>).Reset(a, b, c);
                }
                return cache;
            }
            T p = new T();
            if (p is IPanelInit<A, B, C>)
            {
                (p as IPanelInit<A, B, C>).Init(a, b, c);
            }
            return p;
        }
        
        public void Close<T>() where T : BasePanle
        {
            
        }
        
        public void Remove<T>() where T: BasePanle
        {
            
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var kv in this.panels)
            {
                if (kv.Value is IPanelDestroy)
                {
                    (kv.Value as IPanelDestroy).Destroy();
                }
            }
            this.panels.Clear();
            this.panelStack.Clear();
        }
    }
}