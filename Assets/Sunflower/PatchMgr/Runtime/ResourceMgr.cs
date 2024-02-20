using Assets.Sunflower.Core;
using Assets.Sunflower.Helper;
using Sunflower.Core;
using Sunflower.PatchMgr.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Sunflower.PatchManager.Runtime
{
    public class CacheObejct: IRelease
    {
        public uint crc;
        public string path;
        public int insid;
        public GameObject obj;

        public CacheObejct()
        {

        }
        public void Release()
        {
            this.crc = 0;
            this.insid = 0;
            this.path = "";
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
            this.obj = null;
        }
    }
    public class ResourceMgr : Singleton<ResourceMgr>, ISingletonAwake, ISingletonReverseDispose
    {
        private SimpleObjectPool<CacheObejct> _cacheObejctPool = new(300);
        private Dictionary<int, CacheObejct> _allCacheObject = new();
        private Dictionary<uint, List<int>> _crc2GameObjectInstId = new();
        private Dictionary<uint, UnityEngine.Object> _crc2AssetObject = new();

        public void Awake()
        {
            SingletonMgr.Instance.AddSingletonWithAwake<PatchMgrV2>();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            uint crc = Crc32.GetCrc32(path);
            if (_crc2AssetObject.ContainsKey(crc))
            {
                return _crc2AssetObject[crc] as T;
            }
            PatchAsset pAsset =  PatchMgrV2.Instance.LoadPatchAsset(crc);
            AssetBundle bundle = PatchMgrV2.Instance.LoadAssetBundleByPatchAsset(pAsset);
            T asset =  bundle.LoadAsset<T>(path);
            _crc2AssetObject.Add(crc, asset);
            return asset;
        }

        public void LoadAsync<T>(string path, System.Action<T> callback) where T : UnityEngine.Object
        {
            uint crc = Crc32.GetCrc32(path);
            if (_crc2AssetObject.ContainsKey(crc))
            {
                callback?.Invoke(_crc2AssetObject[crc] as T);
            }
            else
            {
                PatchAsset pAsset = PatchMgrV2.Instance.LoadPatchAsset(path);
                UnityMono.Instance.StartCoroutine(PatchMgrV2.Instance.LoadAssetBundleByPatchAssetAsync(pAsset, (ab) =>
                {
                    if (ab == null)
                    {
                        Log.Error($"LoadResourceAsync failed, Please check path: {path}!");
                    }
                    T asset = ab.LoadAsset<T>(path);
                    callback?.Invoke(asset);
                }));
            }
        }

        public GameObject Instantiate(string path, GameObject obj, Transform parent)
        {
            GameObject go = GameObject.Instantiate(obj, parent, false);
            CacheObejct cGo = _cacheObejctPool.Spawn();
            cGo.obj = go;
            cGo.path = path;
            cGo.crc = Crc32.GetCrc32(path);
            cGo.insid = go.GetInstanceID();
            _allCacheObject.Add(cGo.insid, cGo);
            if (!_crc2GameObjectInstId.ContainsKey(cGo.crc))
            {
                _crc2GameObjectInstId[cGo.crc] = new List<int>();
            }
            _crc2GameObjectInstId[cGo.crc].Add(cGo.insid);
            return go;
        }

        public GameObject Instantiate(string path, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion quaternion)
        {
            GameObject obj = Load<GameObject>(path);
            if (obj != null)
            {
                GameObject inst = Instantiate(path, obj, parent);
                inst.transform.localPosition = localPosition;
                inst.transform.localScale = localScale;
                inst.transform.localRotation = quaternion;
                return inst;
            }
            else
            {
                Log.Error($"GameObject load failed, Please check path: {path}!");
                return null;
            }
        }

        public void Release(GameObject obj, bool destroy = false)
        {
            int instId = obj.GetInstanceID();
            if (_allCacheObject.ContainsKey(instId) && _allCacheObject[instId] != null)
            {
                if (destroy)
                {
                    GameObject.Destroy(obj);
                }
                CacheObejct cGo = _allCacheObject[instId];
                _allCacheObject.Remove(instId);
                _cacheObejctPool.Recycl(cGo);
            }
            else
            {
                Log.Error($"Can not find {instId} in cache!");
                return;
            }
        }

        public void PreLoadObj(string path, int count = 1)
        {
            GameObject obj = Load<GameObject>(path);
            if (obj != null)
            {
                GameObject inst = Instantiate(path, obj, null);
            }
            else
            {
                Log.Error($"GameObject preload failed, Please check path: {path}!");
            }
        }

        public void PreLoadRes<T>(string path) where T : UnityEngine.Object
        {
            Load<T>(path);
        }

        public void Release(Texture texture)
        {
            Resources.UnloadAsset(texture);
        }

        public Sprite LoadSprite(string path)
        {
            if (path.EndsWith(".png") == false) path += ".png";
            return Load<Sprite>(path);
        }

        public Texture LoadTexture(string path)
        {
            if (path.EndsWith(".jpg") == false) path += ".jpg";
            return Load<Texture>(path);
        }

        public AudioClip LoadAudio(string path)
        {
            return Load<AudioClip>(path);
        }

        public TextAsset LoadTextAsset(string path)
        {
            return Load<TextAsset>(path);
        }

        public Sprite LoadAtlasSprite(string atlasPath, string spriteName)
        {
            if (atlasPath.EndsWith(".spriteatlas") == false) atlasPath += ".spriteatlas";
            return LoadSpriteFormAltas(Load<SpriteAtlas>(atlasPath), spriteName);
        }

        private Sprite LoadSpriteFormAltas(SpriteAtlas spriteAtlas, string name)
        {
            if (spriteAtlas == null)
            {
                Log.Error($"spriteAtlas name is null: {name}");
                return null;
            }

            Sprite sprite = spriteAtlas.GetSprite(name);
            if (sprite != null)
            {
                return sprite;
            }
            Log.Error($"{name} is not in {spriteAtlas}!");
            return null;
        }

        public void LoadTextureAsync(string path, Action<Texture> callback)
        {
            LoadAsync<Texture>(path, (t) =>
            {
                callback?.Invoke(t);
            });
        }

        public void LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> callback = null)
        {
            if (path.EndsWith(".png") == false) path += ".png";
            LoadAsync<Sprite>(path, (t) =>
            {
                callback?.Invoke(t);
            });
        }

        public void ClearRes()
        {
            foreach(var kv in _crc2GameObjectInstId)
            {
                List<int> instIds = kv.Value;
                foreach(int id in instIds)
                {
                    if (_allCacheObject.ContainsKey(id))
                    {
                        CacheObejct co = _allCacheObject[id];
                        if (co != null)
                        {
                            GameObject.Destroy(co.obj);
                            _cacheObejctPool.Recycl(co);
                        }
                    }
                }
            }
            _crc2GameObjectInstId.Clear();
            _cacheObejctPool.Clear();

            foreach (var kv in _crc2AssetObject)
            {
                PatchMgrV2.Instance.ReleasePatchAsset(kv.Key);
            }
        }
    }
}

