using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace VSVRMod2.Helper
{
    public class FSMHelper
    {
        public static FsmState GetStateByName(string name, PlayMakerFSM fsm)
        {
            foreach (FsmState state in fsm.FsmStates) {
                if (state.Name == name)
                {
                    return state;
                }
            }
            VSVRMod.logger.LogError("FSM state " + name + " not found!");
            return null;
        }

        public static List<FsmStateAction> GetActionsByType(Type type, FsmState fsmState)
        {
            List<FsmStateAction> actions = new List<FsmStateAction>();
            foreach (FsmStateAction action in fsmState.Actions)
            {
                if (type.IsInstanceOfType(action))
                {
                    actions.Add(action);
                }
            }
            return actions;
        }

        public static NamedVariable GetVariableByName(string name, PlayMakerFSM fsm)
        {
            foreach (NamedVariable namedVar in fsm.FsmVariables.GetAllNamedVariables())
            {
                if (namedVar.Name == name)
                {
                    return namedVar;
                }
            }
            VSVRMod.logger.LogError("FSM named var " + name + " not found!");
            return null;
        }

        public static Fsm GetNestedFSM(Fsm fsm, string stateName, FieldInfo runFsmField)
        {
            FsmState state = fsm.States.FirstOrDefault(s => s.Name == stateName);
            if (state == null)
            {
                VSVRMod.logger.LogError($"FixMissingPreText: '{stateName}' state not found");
                return null;
            }

            List<FsmStateAction> actions = GetActionsByType(typeof(RunFSMAction), state);
            if (actions == null || actions.Count == 0)
            {
                VSVRMod.logger.LogError($"FixMissingPreText: No RunFSMAction found in '{stateName}'");
                return null;
            }

            Fsm nestedFsm = (Fsm)runFsmField.GetValue((RunFSMAction)actions[0]);
            if (nestedFsm == null)
            {
                VSVRMod.logger.LogError($"FixMissingPreText: runFsm is null in '{stateName}'");
                return null;
            }

            return nestedFsm;
        }
    }
}
