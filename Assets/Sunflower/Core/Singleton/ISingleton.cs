
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
}


