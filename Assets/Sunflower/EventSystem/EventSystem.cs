using Sunflower.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sunflower.EventSys
{
    public class EventSystem : Singleton<EventSystem>, ISingletonAwake, ISingletonUpdate
    {
        public static Dictionary<string, List<Delegate>> _stringEventKey2Actions = new();

        public static Dictionary<Type, List<Delegate>> _simpleEvent2Actions = new();

        public static Dictionary<Type, List<Delegate>> _simpleEventOne2Actions = new();

        public static Dictionary<Type, IEvent> _postEvents = new();
        public void Awake()
        {
            
        }

        //延迟触发
        public void Update()
        {
            foreach(var kv  in _postEvents)
            {
                IEvent e = kv.Value;
                Trigger(e);
            }
        }

        //注册事件
        public void RegisterEvent<T>(Action<T> action) where T : IEvent
        {
            lock (_simpleEvent2Actions)
            {
                Type type = typeof(T);
                if (!_simpleEvent2Actions.ContainsKey(type))
                {
                    _simpleEvent2Actions[type] = new List<Delegate>();
                }
                _simpleEvent2Actions[type].Add(action);
            }
        }

        //注册一次性事件
        public void RegisterEventOne<T>(Action<T> action) where T : IEvent
        {
            lock (_simpleEventOne2Actions)
            {
                Type type = typeof(T);
                if (!_simpleEventOne2Actions.ContainsKey(type))
                {
                    _simpleEventOne2Actions[type] = new List<Delegate>();
                }
                _simpleEventOne2Actions[type].Add(action);
            }
        }

        //反注册事件
        public void UnRegisterEvent<T>(Action<T> action, bool isEventOne = false) where T: IEvent
        {
            lock (_simpleEvent2Actions)
            {
                Type type = typeof(T);
                if (!_simpleEvent2Actions.ContainsKey(type))
                {
                    Log.Warning($"Can not find event {type}");
                    return;
                }
                if (_simpleEvent2Actions[type].Contains(action))
                {
                    _simpleEvent2Actions[type].Remove(action);
                }
                if (_simpleEvent2Actions[type].Count == 0)
                {
                    _simpleEvent2Actions.Remove(type);
                }
                lock (_simpleEventOne2Actions)
                {
                    Type t = typeof(T);
                    if (!_simpleEventOne2Actions.ContainsKey(t))
                    {
                        Log.Warning($"Can not find event {t}");
                        return;
                    }
                    if (_simpleEventOne2Actions[t].Contains(action))
                    {
                        _simpleEventOne2Actions[t].Remove(action);
                    }
                    if (_simpleEventOne2Actions[t].Count == 0)
                    {
                        _simpleEventOne2Actions.Remove(t);
                    }
                }
            }
        }

        //触发事件
        public void Trigger<T>(T e) where T : IEvent
        {
            Type type = typeof(T);
            lock (_simpleEvent2Actions)
            {
                
                if (!_simpleEvent2Actions.ContainsKey(type))
                {
                    Log.Warning($"Can not find event {type}");
                    return;
                }
                if (_simpleEvent2Actions[type].Count == 0)
                {
                    Log.Debug($"No action in event {type}");
                    _simpleEvent2Actions.Remove(type);
                    return;
                }
                foreach (Delegate action in _simpleEvent2Actions[type])
                {
                    action?.DynamicInvoke(e);
                }
            }

            lock (_simpleEventOne2Actions)
            {
                if (!_simpleEventOne2Actions.ContainsKey(type))
                {
                    Log.Warning($"Can not find event {type}");
                    return;
                }
                if (_simpleEventOne2Actions[type].Count == 0)
                {
                    Log.Debug($"No action in event {type}");
                    _simpleEventOne2Actions.Remove(type);
                    return;
                }
                foreach (Delegate action in _simpleEventOne2Actions[type])
                {
                    action?.DynamicInvoke(e);
                }
                _simpleEventOne2Actions[type].Clear();
                _simpleEventOne2Actions.Remove(type);
            }
            Log.Debug($"SimpleEvnet {type} be Trigger.");
        }

        //延迟触发事件, 下一次update才会触发事件
        public void PostTrigger<T>(T e) where T : IEvent
        {
            Type type = typeof(T);
            _postEvents.Add(type, e);
        }

        public override void Dispose()
        {
            base.Dispose();
            _simpleEvent2Actions.Clear();
            _simpleEvent2Actions = null;
        }

    }
}
