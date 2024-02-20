namespace Sunflower.Core
{
    public abstract class AAddon
    {
        public abstract void Start();

        public abstract void Pause();

        public abstract void Resume();

        public abstract void Destroy();
    }

    public interface IAddonUpdate
    {
        public void Update();
    }

    public enum AddonState
    {
        Active,
        Sleep,
        Destroyed
    }

    public abstract class Addon: AAddon
    {

        private AddonState state;

        public AddonState State
        {
            get
            {
                return this.state;
            }
            private set{}
        }
        
        public override void Start()
        {
            this.state = AddonState.Active;
            Log.LogGreen($"Addon {this.GetType().Name} Start!");
        }

        public override void Pause()
        {
            this.state = AddonState.Sleep;
            Log.LogGreen($"Addon {this.GetType().Name} Pause!");
        }

        public override void Resume()
        {
            this.state = AddonState.Active;
            Log.LogGreen($"Addon {this.GetType().Name} Resume!");
        }

        public override void Destroy()
        {
            this.state = AddonState.Destroyed;
            Log.LogGreen($"Addon {this.GetType().Name} Destroy!");
        }
    }
}