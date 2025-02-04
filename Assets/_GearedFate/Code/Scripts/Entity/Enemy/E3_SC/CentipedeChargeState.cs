using UnityEngine;

namespace BTG
{
    public class CentipedeChargeState : CentipedeBaseState
    {
        private readonly int _animId;

        private bool _firstCircling;

        private VectorHelper2D.Direction _circleDirection;

        public CentipedeChargeState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId,
            SteamCentipede steamCentipede) : base(fsm, steamCentipede)
        {
            this._animId = animationId;
        }

        public override void OnEnter()
        {
            this.Centipede.IsAttacking = true;
            this.Centipede.SetSpeed(this.Centipede.ChargeSpeed);
            this.Centipede.SetAnimations(this._animId, true);

            if (this.Centipede.IsReachingTrajectoryEndNextStep()) this.Centipede.ExpandTrajectory(this.ComputeChargeAimPosition());
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            this.Centipede.MoveAlongTrajectory();

            if (!this.Centipede.IsReachingTrajectoryEndNextStep()) return;

            this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Chase]);
        }

        public override void OnPhysicsUpdate()
        {
        }

        private Vector2 ComputeChargeAimPosition()
        {
            Vector3 direction = this.Centipede.VectorToTarget;
            return this.Centipede.Target.position - direction.normalized * 3;
        }
    }
}
