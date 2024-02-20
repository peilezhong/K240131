using Sunflower.Helper;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sunflower.Core.Singleton
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                string filePath = GetFilePath();
                
                if (!_instance)
                {
                    Debug.LogWarning(Path.GetFileNameWithoutExtension(PathHelper.Last(filePath)));
                    _instance = Resources.Load<T>(Path.GetFileNameWithoutExtension(PathHelper.Last(filePath)));
                    if(!_instance )
                    {
#if UNITY_EDITOR
                        _instance = ScriptableObject.CreateInstance<T>();
                        AssetDatabase.CreateAsset(_instance, filePath);
#else
                        Debug.LogError($"can not found {typeof(T)}");
#endif
                    }
                }
                return _instance;
            }
        }


        public void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
        protected static string GetFilePath()
        {
            return typeof(T).GetCustomAttributes(false)
                .Where(v => v is FilePathAttribute)
                .Cast<FilePathAttribute>()
                .FirstOrDefault()
                ?.filepath;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        internal string filepath;
        /// <summary>
        /// 单例存放路径
        /// </summary>
        /// <param name="path">相对 项目根目录 路径</param>
        public FilePathAttribute(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid relative path (it is empty)");
            }
            if (path[0] == '/')
            {
                path = path.Substring(1);
            }
            filepath = path;
        }
    }
}
