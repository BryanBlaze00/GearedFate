using System.Linq;
using UnityEngine;

namespace BTG
{
    public class GCDashState : GCBaseState
    {
        private Vector2 _dashStartingPosition;

        public GCDashState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy,
            int animId) : base(fsm,
            enemy, animId)
        {
            var smb = this.GreatCreator.Animator.GetBehaviours<MultiStepSmb>().First(x => x.Id == "Dash");
            smb.OnStepReached += this.HandleDashStepReached;
        }

        private void HandleDashStepReached(int step)
        {
            switch (step)
            {
                case 0:
                    this.HandleStartDash();
                    break;
                case 1:
                    this.HandleEndDash();
                    break;
                case 2:
                    this.HandleAnimationEnd();
                    break;
            }
        }

        private void HandleAnimationEnd()
        {
            this.GreatCreator.Agent.isStopped = false;
            this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
        }

        private void HandleEndDash()
        {
            this.GreatCreator.Rigidbody.linearVelocity = Vector2.zero;
        }

        private void HandleStartDash()
        {
            this._dashStartingPosition = this.GreatCreator.transform.position;
            this.GreatCreator.SetAnimationMoveParameters(-this.GreatCreator.DirectionToTarget);
            this.GreatCreator.Rigidbody.AddForce(-this.GreatCreator.DashForce * this.GreatCreator.DirectionToTarget);
        }

        public override void OnEnter()
        {
            this.PlayAnimation();
            this.GreatCreator.Agent.isStopped = true;
            this.GreatCreator.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public override void OnExit()
        {
            Debug.Log(
                $"distance from start to end dash {Vector2.Distance(this._dashStartingPosition, this.GreatCreator.transform.position)}");
            if (Vector2.Distance(this._dashStartingPosition, this.GreatCreator.transform.position) < 2) this.GreatCreator.StartCoroutine(this.GoToCenter());

            this.GreatCreator.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
