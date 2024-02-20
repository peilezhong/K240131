using System;

namespace Sunflower.Core
{
    public class SimpleSingleton<T> : IDisposable where T: SimpleSingleton<T>, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    return new T();
                }
                return _instance;
            }
        }

        public void Dispose()
        {
            _instance = null;
        }
    }
}