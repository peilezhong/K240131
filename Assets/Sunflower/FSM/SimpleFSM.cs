

using Sunflower.Core;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Sunflower.FSM
{

    [AttributeUsage(AttributeTargets.Class)]
    public class SimpleFSMAttribute : SunflowerAttribute
    {
        public SimpleFSMAttribute(params Type[] states) 
        {
            this.states = states;
            if (states.Length > 0)
            {
                this.dafalutSate = states[0];
            }
        }

        public Type[] states { get; }
        public Type dafalutSate { get; }
    }
    public interface ISimpleFSMState
    {
        public void OnCreate();
        public void OnEnter();
        public void OnExit();

        public void OnUpdate();
    }

    public class SimpleFSM : IDObject
    {
        private ISimpleFSMState _curState;
        private ISimpleFSMState _lastState;
        private Dictionary<Type, ISimpleFSMState> _type2Node = new();

        private Dictionary<string, object> _blackboard = new();

        public ISimpleFSMState CurState { get { return _curState; } }
        public ISimpleFSMState LastState { get { return _lastState; } }

        public object GetFromBlackboard(string str)
        {
            object value;
            _blackboard.TryGetValue(str, out value);
            return value;
        }

        public void WriteToBlackboard(string str, object obj)
        {
            _blackboard.Add(str, obj);
        }

        public void Destroy()
        {
            _type2Node.Clear();
            SimpleFsmMgr.Instance.UnRegisterFSM(this.Id);
        }

        public void ChangeState(ISimpleFSMState state)
        {
            _lastState = _curState;
            _curState = state;
            _lastState.OnExit();
            _curState.OnExit();
        }
        public static SimpleFSM CreateFSM<T>() where T : SimpleFSM, new()
        {
            T fsm = new T();
            object[] objs = typeof(T).GetCustomAttributes(typeof(SimpleFSMAttribute), false);
            if (objs != null && objs.Length > 0)
            {
                SimpleFSMAttribute attribute = objs[0] as SimpleFSMAttribute;
                if (attribute != null)
                {
                    Type[] states = attribute.states;
                    if(states != null && states.Length > 0)
                    {
                        foreach (var s in states)
                        {
                            if (typeof(ISimpleFSMState).IsAssignableFrom(s))
                            {
                                fsm.AddNode(s);
                            }
                        }
                    }
                }
            }
            SimpleFsmMgr.Instance.RegisterFSM(fsm);
            return fsm;
        }

        public void AddNode<T>() where T : ISimpleFSMState, new()
        {
            Type t = typeof(T);
            if (_type2Node.ContainsKey(t))
            {
                Log.Warning($"Fsm have been contain node {t}");
                return;
            }
            ISimpleFSMState state = new T();
            state.OnCreate();
            _type2Node.Add(t, state);
        }

        public void AddNode(Type type)
        {
            if(type is not ISimpleFSMState)
            {
                Log.Warning($"{type} is not ISimpleFSMState");
                return;
            }
            if (_type2Node.ContainsKey(type))
            {
                Log.Warning($"Fsm have been contain node {type}");
                return;
            }
            ISimpleFSMState state = Activator.CreateInstance(type) as ISimpleFSMState;
            if (state != null)
            {
                state.OnCreate();
                _type2Node.Add(type, state);
            }
            else
            {
                Log.Warning($"Cannot create instance of state type {type}");
            }
        }
    }
}

