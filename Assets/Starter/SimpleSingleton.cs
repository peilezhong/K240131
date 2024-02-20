using System;

namespace Client.Starter
{
    public class SimpleSingleton<T> : IDisposable where T: SimpleSingleton<T>, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    return new T();
                }
                return instance;
            }
        }

        public void Dispose()
        {
            instance = null;
        }
    }
}