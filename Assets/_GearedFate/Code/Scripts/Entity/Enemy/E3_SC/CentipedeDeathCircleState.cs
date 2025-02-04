using UnityEngine;

namespace BTG
{
    public class CentipedeDeathCircleState : CentipedeBaseState
    {
        private readonly int _animId;

        private bool _firstCircling;

        private Vector3 _originalTargetPosition;

        private VectorHelper2D.Direction _circleDirection;

        private float _initialDistanceToTarget;

        private float _finalDistanceToTarget = 1f;

        private float _timeToReachMinimalDistance = 7f;

        private float _timer;

        public CentipedeDeathCircleState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId,
            SteamCentipede steamCentipede) : base(fsm, steamCentipede)
        {
            this._animId = animationId;
        }

        public override void OnEnter()
        {
            this.Centipede.IsAttacking = true;
            this.Centipede.SetSpeed(this.Centipede.ChargeSpeed);
            this.Centipede.SetAnimations(this._animId, true);
            this._originalTargetPosition = this.Centipede.Target.position;
            this._initialDistanceToTarget = this.Centipede.DistanceToTarget;
            this._firstCircling = true;
            this._timer = 0;

            if (this.Centipede.IsReachingTrajectoryEndNextStep()) this.Centipede.ExpandTrajectory(this.ComputeDeathCirclePosition());
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            this._timer += Time.deltaTime;
            this.Centipede.MoveAlongTrajectory();


            if (!this.Centipede.IsReachingTrajectoryEndNextStep()) return;

            if (Vector2.Distance(this.Centipede.Target.position, this._originalTargetPosition) > this._initialDistanceToTarget)
            {
                this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Charge]);
            }
            else
            {
                if (this._timer > this._timeToReachMinimalDistance) this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Charge]);
                this.Centipede.ExpandTrajectory(this.ComputeDeathCirclePosition());
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        private Vector2 ComputeDeathCirclePosition()
        {
            var directionFromPlayerToHead = this.Centipede.VectorToTarget;

            // if not circling yet, choose as a node the closest cardinal point at the defined circling distance.
            if (this._firstCircling)
            {
                this._circleDirection = VectorHelper2D.ClosestCardinalOrDiagonal(directionFromPlayerToHead);
                this._firstCircling = false;
            }
            else
            {
                this._circleDirection = VectorHelper2D.NextClockWiseDirection(this._circleDirection);
            }

            var nodeDirection = VectorHelper2D.VectorFromDirection(this._circleDirection);

            var circlingDistance = this._initialDistanceToTarget / 2 * (1 - this._timer / this._timeToReachMinimalDistance) + this._finalDistanceToTarget * this._timer / this._timeToReachMinimalDistance;

            return (Vector2)this._originalTargetPosition + nodeDirection * circlingDistance;
        }
    }
}
