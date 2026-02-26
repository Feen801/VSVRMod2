using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace VSVRMod2.Helper
{
    class SessionBugfixes
    {
        public static void FixAll()
        {
            if (VRConfig.fixMissingPreText.Value)
            {
                VSVRMod.logger.LogInfo("Fixing missing pretext");
                FixMissingPreText();
            }
        }

        public static void FixMissingPreText()
        {
            try
            {
                GameObject go = GameObjectHelper.GetGameObjectCheckFound("Root/GeneralCanvas/EventManager");
                if (go == null)
                {
                    VSVRMod.logger.LogError("FixMissingPreText: EventManager GameObject not found");
                    return;
                }

                PlayMakerFSM fsmComponent = go.GetComponent<PlayMakerFSM>();
                if (fsmComponent == null)
                {
                    VSVRMod.logger.LogError("FixMissingPreText: No PlayMakerFSM on EventManager");
                    return;
                }

                FieldInfo field = typeof(RunFSMAction).GetField("runFsm", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    VSVRMod.logger.LogError("FixMissingPreText: Could not find runFsm field via reflection - game may have updated");
                    return;
                }

                Fsm fsm = fsmComponent.Fsm;

                // Drill down: EventManager FSM -> "Play Tasks" -> RunFSMAction -> nested FSM
                fsm = FSMHelper.GetNestedFSM(fsm, "Play Tasks", field);
                if (fsm == null) return;

                // Drill down: nested FSM -> "PlayEvent" -> RunFSMAction -> nested FSM
                fsm = FSMHelper.GetNestedFSM(fsm, "PlayEvent", field);
                if (fsm == null) return;

                FsmState state = fsm.States.FirstOrDefault(s => s.Name == "Pre-Text");
                if (state == null)
                {
                    VSVRMod.logger.LogError("FixMissingPreText: 'Pre-Text' state not found");
                    return;
                }

                List<FsmStateAction> actions = FSMHelper.GetActionsByType(typeof(SetProperty), state);
                if (actions == null || actions.Count == 0)
                {
                    VSVRMod.logger.LogError("FixMissingPreText: No SetProperty action found in 'Pre-Text'");
                    return;
                }

                SetProperty propAction = (SetProperty)actions[0];
                FsmString preReqs = fsm.Variables.FindFsmString("EventPreRequirements");
                if (preReqs == null)
                {
                    VSVRMod.logger.LogError("FixMissingPreText: 'EventPreRequirements' variable not found");
                    return;
                }

                propAction.targetProperty.StringParameter = preReqs;
                VSVRMod.logger.LogInfo("FixMissingPreText: Successfully applied fix");
            }
            catch (Exception ex)
            {
                VSVRMod.logger.LogError($"FixMissingPreText: Unexpected error - {ex}");
            }
        }
    }
}
