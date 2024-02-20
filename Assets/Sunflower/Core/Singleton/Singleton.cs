namespace Sunflower.Core
{
    public interface ISingletonReverseDispose//需要按顺序卸载的单例继承这个接口
    {
        
    }

    public interface ISingletonAwake
    {
        public void Awake();
    }
    
    public interface ISingletonUpdate
    {
        public void Update();
    }
    
    public interface ISingletonAwake<A>
    {
        public void Awake(A a);
    }
    
    public interface ISingletonAwake<A, B>
    {
        public void Awake(A a, B b);
    }
    
    public interface ISingletonAwake<A, B, C>
    {
        public void Awake(A a, B b, C c);
    }
    
    public abstract class ISingleton: DisposeObject
    {
        internal abstract void Register();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        private bool isDispoised;
        
        private static T _instance;
        
        public static T Instance{
            get
            {
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }
        
        internal override void Register()
        {
            Instance = (T)this;
        }

        protected virtual void Destroy()
        {
            
        }

        public bool IsDisposed()
        {
            return this.isDispoised;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (this.isDispoised)
            {
                return;
            }

            this.isDispoised = true;
            Instance = null;
        }
    }
}