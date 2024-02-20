using System.Collections.Generic;
using UnityEngine;

namespace Sunflower.UI
{
    public interface IPanel
    {
        
    }
    
    public interface IPanelInit
    {
        public void Init();
    }
    
    public interface IPanelInit<A>
    {
        public void Init(A a);
    }
    
    public interface IPanelInit<A, B>
    {
        public void Init(A a, B b);
    }
    
    public interface IPanelInit<A, B, C>
    {
        public void Init(A a, B b, C c);
    }
    
    public interface IPanelReset
    {
        public void Reset();
    }
    
    public interface IPanelReset<A>
    {
        public void Reset(A a);
    }
    
    public interface IPanelReset<A, B>
    {
        public void Reset(A a, B b);
    }
    
    public interface IPanelReset<A, B, C>
    {
        public void Reset(A a, B b, C c);
    }
    
    public interface IPanelHide
    {
        public void Hide();
    }
    
    public interface IPanelDestroy
    {
        public void Destroy();
    }

    public class BasePanle
    {
        // UnityEngine.UI.Button
        public GameObject GO;
        public Dictionary<string, GameObject> Nodes = new();

        public void Init()
        {
            this.CollectNodes();
        }

        public void SetGO(GameObject go)
        {
            this.GO = go;
        }

        public void CollectNodes()
        {
            GameObject[] gos = this.GO.GetComponentsInChildren<GameObject>();
            foreach (GameObject go in gos)
            {
                if (go.name.StartsWith("sui_"))
                {
                    this.Nodes.Add(go.name.Split("_")[1], go);
                }
            }
        }

        public GameObject GetNode(string name)
        {
            if (!this.Nodes.ContainsKey(name))
            {
                return null;
            }

            return this.Nodes[name];
        }

        public void Destroy()
        {
            this.Nodes.Clear();
            GameObject.Destroy(this.GO);
        }
    }
    
    public abstract class GamePanel : BasePanle
    {

    }

    public abstract class PopPanle : BasePanle
    {

    }
}