using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace BTG
{
    public class GCSpinState : GCBaseState
    {
        public GCSpinState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId) :
            base(fsm, enemy, animId)
        {
            var smb = this.GreatCreator.Animator.GetBehaviours<MultiStepSmb>().First(x => x.Id == "Spin");
            smb.OnStepReached += this.HandleEndSpin;
        }

        private void HandleEndSpin(int obj)
        {
            this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.RunAway]);
        }


        public override void OnEnter()
        {
            this.PlayAnimation();
            this.GreatCreator.StartCoroutine(this.GreatCreator.ShootProjectiles());
            this.GreatCreator.Agent.isStopped = true;
        }

        public override void OnExit()
        {
            this.GreatCreator.Agent.isStopped = false;
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }

        public void SetAnimation(int stringToHash)
        {
            this.SetAnimationId(stringToHash);
        }
    }
}
