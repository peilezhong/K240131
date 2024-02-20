//using System;
//using UnityEngine;
//using System.IO;
//using Unity.VisualScripting.YamlDotNet.Serialization;

//namespace Sunflower.Helper
//{
//    public class YamlHelper
//    {
//        public static void SerializeYaml(System.Object obj, string path)
//        {
//            try
//            {
//                Serializer yamlSerializer = new Serializer();
//                string ouputYaml = yamlSerializer.Serialize(obj);
//                File.WriteAllText(path, ouputYaml);
//            }
//            catch (Exception e)
//            {
//                Debug.LogError(e);
//            }
//        }

//        public static T DeserializeYaml<T>(string path)
//        {
//            try
//            {
//                string yamlContent = String.Empty;
//                if(File.Exists(path))
//                {
//                    yamlContent = File.ReadAllText(path);
//                }

//                Deserializer yamlDeserializer = new Deserializer();
//                T obj = yamlDeserializer.Deserialize<T>(yamlContent);
//                return obj;
//            }
//            catch(Exception e)
//            {
//                Debug.LogError(e);
//                throw;
//            }
//        }
//    }
//}
