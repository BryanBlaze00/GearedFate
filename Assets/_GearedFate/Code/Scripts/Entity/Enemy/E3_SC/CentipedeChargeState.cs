using UnityEngine;

namespace BTG
{
    public class CentipedeChargeState : CentipedeBaseState
    {
        private readonly int _animId;

        private bool _firstCircling;

        private VectorHelper2D.Direction _circleDirection;

        public CentipedeChargeState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId, SteamCentipede steamCentipede) : base(fsm, steamCentipede)
        {
            _animId = animationId;
        }

        public override void OnEnter()
        {
            Centipede.IsAttacking = true;
            Centipede.SetSpeed(Centipede.ChargeSpeed);
            Centipede.SetAnimations(_animId, true);

            if (Centipede.IsReachingTrajectoryEndNextStep())
            {
                Centipede.ExpandTrajectory(ComputeChargeAimPosition());
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

            fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Chase]);
        }

        public override void OnPhysicsUpdate()
        {
        }

        private Vector2 ComputeChargeAimPosition()
        {
            Vector3 direction = Centipede.VectorToTarget;
            return Centipede.Target.position - direction.normalized * 3;
        }
    }
}
