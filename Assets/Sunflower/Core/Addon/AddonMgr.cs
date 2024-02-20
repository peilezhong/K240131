using System.Collections.Generic;

namespace Sunflower.Core
{
    public class AddonMgr: Singleton<AddonMgr>, ISingletonUpdate
    {
        private List<string> addonsOrder = new();
        private Dictionary<string, Addon> addons = new();
        private List<string> addonsWithUpdate = new();
        
        public void Update()
        {
            foreach (string tag in addonsWithUpdate)
            {
                if (!this.addons.ContainsKey(tag))
                {
                    Log.Error("Can not found the addon instance in Dictionary<string, Addon> addons, please cheack the framework!!!");
                }
                
                if(this.addons[tag].State != AddonState.Active) continue;

                (this.addons[tag] as IAddonUpdate).Update();
            }
        }

        public void RegisterAddon(Addon addon, string tag)
        {
            if (this.addons.ContainsKey(tag))
            {
                Log.Error($"Can not repeat add addon {tag}!!!");
            }

            if (addon is IAddonUpdate)
            {
                this.addonsWithUpdate.Add(tag);
            }
            this.addonsOrder.Add(tag);
            this.addons[tag] = addon;
        }
        
        public T RegisterAddon<T>() where T : Addon, new()
        {
            string tag = typeof(T).FullName;
            if (this.addons.ContainsKey(tag))
            {
                Log.Error($"Can not repeat add addon {tag}!!!");
            }
            T a = new T();
            this.RegisterAddon(a, tag);
            return a;
        }

        public T GetAddon<T>() where T : Addon
        {
            string tag = typeof(T).FullName;
            if (!this.addons.ContainsKey(tag))
            {
                Log.Error($"Can not found addon {tag}");
            }
            return (T)this.addons[tag];
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (string tag in this.addonsOrder)
            {
                if (!this.addons.ContainsKey(tag))
                {
                    Log.Error("Can not found the addon instance in Dictionary<string, Addon> addons, please cheack the framework!!!");
                }
                this.addons[tag].Destroy();
            }
            this.addonsOrder.Clear();
            this.addonsWithUpdate.Clear();
            this.addons.Clear();
        }
    }
}