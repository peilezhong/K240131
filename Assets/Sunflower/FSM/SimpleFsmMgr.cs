
using Sunflower.Core;
using System;
using System.Collections.Generic;

namespace Sunflower.FSM
{
    public class SimpleFsmMgr : Singleton<SimpleFsmMgr>, ISingletonUpdate
    {

        private Dictionary<Guid, SimpleFSM> _fsmId2Insts = new();

        public void RegisterFSM(SimpleFSM fsm)
        {
            if (_fsmId2Insts.ContainsKey(fsm.Id))
            {
                Log.Warning($"FSM {fsm.Id} have been register!");
            }
            _fsmId2Insts.Add(fsm.Id, fsm);
        }

        public void UnRegisterFSM(Guid id)
        {
            if (!_fsmId2Insts.ContainsKey(id))
            {
                Log.Warning($"Can not find fsm {id}!");
            }
            _fsmId2Insts.Remove(id);
        }

        public void Update()
        {
            foreach(var fsm in _fsmId2Insts.Values)
            {
                fsm.CurState.OnUpdate();
            }
        }
    }
}
