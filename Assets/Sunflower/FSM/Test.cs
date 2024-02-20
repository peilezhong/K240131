

using Sunflower.Core;

namespace Sunflower.FSM
{
    [SimpleFSM(typeof(State1), typeof(State2), typeof(State3))]
    public class Test
    {

    }

    public class State1 : ISimpleFSMState
    {
        public void OnCreate()
        {
            Log.Debug("State1 OnCreate");
        }

        public void OnEnter()
        {
            Log.Debug("State1 OnEnter");
        }

        public void OnExit()
        {
            Log.Debug("State1 OnExit");
        }

        public void OnUpdate()
        {
            Log.Debug("State1 OnUpdate");
        }
    }

    public class State2 : ISimpleFSMState
    {
        public void OnCreate()
        {
            Log.Debug("State1 OnCreate");
        }

        public void OnEnter()
        {
            Log.Debug("State1 OnEnter");
        }

        public void OnExit()
        {
            Log.Debug("State1 OnExit");
        }

        public void OnUpdate()
        {
            Log.Debug("State1 OnUpdate");
        }
    }

    public class State3 : ISimpleFSMState
    {
        public void OnCreate()
        {
            Log.Debug("State1 OnCreate");
        }

        public void OnEnter()
        {
            Log.Debug("State1 OnEnter");
        }

        public void OnExit()
        {
            Log.Debug("State1 OnExit");
        }

        public void OnUpdate()
        {
            Log.Debug("State1 OnUpdate");
        }
    }
}
