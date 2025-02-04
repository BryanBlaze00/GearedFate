using UnityEngine;

namespace BTG
{
    public class GCDeathState : GCBaseState
    {
        public GCDeathState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId) :
            base(fsm, enemy, animId)
        {
        }

        public override void OnEnter()
        {
            this.PlayAnimation();
            this.GreatCreator.Agent.isStopped = true;
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
