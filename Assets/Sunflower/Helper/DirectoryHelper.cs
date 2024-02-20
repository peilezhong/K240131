using System.IO;
using UnityEngine;


namespace Assets.Sunflower.Helper
{
    public class DirectoryHelper
    {
        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Directory {path} is not exist!");
                return;
            }
            else
            {
                Directory.Delete(path, true);
            }
        }

        public static void DeleteAllFiles(string path)
        {
            string[] files = Directory.GetFiles(path); 
            foreach (string file in files)
            {
                File.Delete(file);
            } 
            string[] subfolders = Directory.GetDirectories(path);
            foreach (string subfolder in subfolders)
            {
                DeleteAllFiles(subfolder);
            }
        }

        public static void ClearDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            string[] subfolders = Directory.GetDirectories(path);
            foreach (string subfolder in subfolders)
            {
                Directory.Delete(subfolder, true);
            }
        }

    }
}
