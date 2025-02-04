using UnityEngine;

namespace BTG
{
    public class CentipedeChaseState : CentipedeBaseState
    {
        private readonly int _animId;

        private readonly float _chasingDistance;

        public CentipedeChaseState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId,
            SteamCentipede steamCentipede, float chasingDistance) : base(fsm, steamCentipede)
        {
            this._animId = animationId;
            this._chasingDistance = chasingDistance;
        }

        public override void OnEnter()
        {
            this.Centipede.SetSpeed(this.Centipede.RegularSpeed);
            this.Centipede.SetAnimations(this._animId, true);
            this.Centipede.IsAttacking = false;

            if (this.Centipede.IsReachingTrajectoryEndNextStep()) this.Centipede.ExpandTrajectory(this.ComputeChaseAimPosition());
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            this.Centipede.MoveAlongTrajectory();
            if (!this.Centipede.IsReachingTrajectoryEndNextStep()) return;

            if (this.Centipede.DistanceToTarget < this._chasingDistance)
                this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Circle]);
            else
                this.Centipede.ExpandTrajectory(this.ComputeChaseAimPosition());
        }

        public override void OnPhysicsUpdate()
        {
        }

        private Vector2 ComputeChaseAimPosition()
        {
            return this.Centipede.HeadPosition - this.Centipede.VectorToTarget.normalized * 3;
        }
    }
}
