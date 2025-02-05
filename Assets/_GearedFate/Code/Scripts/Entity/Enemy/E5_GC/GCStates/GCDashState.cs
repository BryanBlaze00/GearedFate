namespace BTG
{
    using System.Linq;
    using UnityEngine;

    public class GCDashState : GCBaseState
    {
        private Vector2 _dashStartingPosition;

        public GCDashState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId)
            : base(fsm, enemy, animId)
        {
            var smb = GreatCreator.Animator.GetBehaviours<MultiStepSmb>().First(x => x.Id == "Dash");
            smb.OnStepReached += HandleDashStepReached;
        }

        private void HandleDashStepReached(int step)
        {
            switch (step)
            {
                case 0:
                    HandleStartDash();
                    break;
                case 1:
                    HandleEndDash();
                    break;
                case 2:
                    HandleAnimationEnd();
                    break;
            }
        }

        private void HandleAnimationEnd()
        {
            GreatCreator.Agent.isStopped = false;
            Fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
        }

        private void HandleEndDash()
        {
            GreatCreator.Rigidbody.linearVelocity = Vector2.zero;
        }

        private void HandleStartDash()
        {
            _dashStartingPosition = GreatCreator.transform.position;
            GreatCreator.SetAnimationMoveParameters(-GreatCreator.DirectionToTarget);
            GreatCreator.Rigidbody.AddForce(-GreatCreator.DashForce * GreatCreator.DirectionToTarget);
        }

        public override void OnEnter()
        {
            PlayAnimation();
            GreatCreator.Agent.isStopped = true;
            GreatCreator.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public override void OnExit()
        {
            Debug.Log(
                $"distance from start to end dash {Vector2.Distance(_dashStartingPosition, GreatCreator.transform.position)}");
            if (Vector2.Distance(_dashStartingPosition, GreatCreator.transform.position) < 2)
            {
                GreatCreator.StartCoroutine(GoToCenter());
            }

            GreatCreator.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
