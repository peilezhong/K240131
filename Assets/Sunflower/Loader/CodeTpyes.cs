using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sunflower.Core;

namespace Sunflower.Loader
{
    public class CodeTpyes: Singleton<CodeTpyes>, ISingletonAwake<Assembly[]>
    {
        private readonly Dictionary<string, Type> allTypes = new();
        private readonly Dictionary<Type, HashSet<Type>> types = new();
        
        public void Awake(Assembly[] assemblies)
        {
            Dictionary<string, Type> addTypes = AssemblyHelper.GetAssemblyTypes(assemblies);
            foreach ((string fullName, Type type) in addTypes)
            {
                this.allTypes[fullName] = type;
                if (type.IsAbstract)
                {
                    continue;
                }
                
                // 记录所有的有SunflowerAttribute标记的的类型
                object[] objects = type.GetCustomAttributes(typeof(SunflowerAttribute), true);
                if (objects == null || objects.Length == 0)
                {
                    continue;
                }
                foreach (object o in objects)
                {
                    Type t = o.GetType();
                    if (this.types.ContainsKey(t))
                    {
                        this.types[t].Add(type);
                    }
                    else
                    {
                        this.types[t] = new HashSet<Type>();
                        this.types[t].Add(type);
                    }
                }
            }
        }

        public HashSet<Type> GetTypesByAttribute(Type attr)
        {
            if (!this.types.ContainsKey(attr))
            {
                return new HashSet<Type>();
            }

            return this.types[attr];
        }
        
        public Type GetTypeByAttribute(Type attr)
        {
            if (!this.types.ContainsKey(attr))
            {
                Log.Debug("");
                return null;
            }

            return this.types[attr].First();
        }

        public Dictionary<string, Type> GetTypes()
        {
            return this.allTypes;
        }

        public Type GetType(string typeName)
        {
            if (!this.allTypes.ContainsKey(typeName))
            {
                Log.Error($"Can not found the Type of {typeName}");
            }

            return this.allTypes[typeName];
        }
    }
}