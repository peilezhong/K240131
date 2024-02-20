
#if UNITY_EDITOR
using Sunflower.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sunflower.Editor
{
    public class PlatformSettings : SimpleSingleton<PlatformSettings>
    {
        public Dictionary<BuildTarget, Dictionary<string, string>> Settings;

        public void SetPlatformSettingByTarget(BuildTarget target, string k, string v)
        {
            if(!Settings.ContainsKey(target))
            {
                Settings.Add(target, new Dictionary<string, string>());
            }
            Settings[target].Add(k, v);
        }

        public void RemoveSettingByTarget(BuildTarget target)
        {
            Settings.Remove(target);
        }

        public void RemoveSettingByTarget(BuildTarget target, string k, string v)
        {
            if (!Settings.ContainsKey(target))
            {
                Debug.LogWarning($"Can not find platform {target} setting!");
            }
            else
            {
                if (!Settings[target].ContainsKey(k))
                {
                    Debug.LogWarning($"Can not find platform {target} setting {k}!");
                }
                else
                {
                    Settings[target].Remove(k);
                }
            }
        }

        public Dictionary<string, string> GetPlatformSettingByTarget(BuildTarget target)
        {
            if(!Settings.ContainsKey(target))
            {
                Debug.LogWarning($"Can not find platform {target} setting!");
                return null;
            }
            return Settings[target];
        }
    }
}
#endif
