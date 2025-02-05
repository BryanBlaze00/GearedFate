namespace BTG
{
    using UnityEngine;

    public class CentipedeChaseState : CentipedeBaseState
    {
        private readonly int _animId;

        private readonly float _chasingDistance;

        public CentipedeChaseState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId,
            SteamCentipede steamCentipede, float chasingDistance)
            : base(fsm, steamCentipede)
        {
            _animId = animationId;
            _chasingDistance = chasingDistance;
        }

        public override void OnEnter()
        {
            Centipede.SetSpeed(Centipede.RegularSpeed);
            Centipede.SetAnimations(_animId, true);
            Centipede.IsAttacking = false;

            if (Centipede.IsReachingTrajectoryEndNextStep())
            {
                Centipede.ExpandTrajectory(ComputeChaseAimPosition());
            }
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            Centipede.MoveAlongTrajectory();
            if (!Centipede.IsReachingTrajectoryEndNextStep())
            {
                return;
            }

            if (Centipede.DistanceToTarget < _chasingDistance)
            {
                fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Circle]);
            }
            else
            {
                Centipede.ExpandTrajectory(ComputeChaseAimPosition());
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        private Vector2 ComputeChaseAimPosition()
        {
            return Centipede.HeadPosition - (Centipede.VectorToTarget.normalized * 3);
        }
    }
}
