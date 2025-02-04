using UnityEngine;

namespace BTG
{
    public class CentipedeCircleState : CentipedeBaseState
    {
        private readonly int _animId;

        private readonly float _chasingDistance;

        private readonly float _circlingDistance;

        private bool _firstCircling;

        private VectorHelper2D.Direction _circleDirection;

        private float _chargingTimer;

        private float _timeBeforeCharge;

        private float _minTimeBeforeCharge;

        private float _maxTimeBeforeCharge;

        private float _layingEggRate = 1.5f;

        private float _eggLayingTimer;

        public CentipedeCircleState(
            FiniteStateMachine<SteamCentipede.CentipedeState> fsm,
            int animationId,
            SteamCentipede steamCentipede,
            float chasingDistance,
            float circlingDistance,
            float minTimeBeforeCharge,
            float maxTimeBeforeCharge) : base(fsm, steamCentipede)
        {
            this._animId = animationId;
            this._chasingDistance = chasingDistance;
            this._circlingDistance = circlingDistance;
            this._minTimeBeforeCharge = minTimeBeforeCharge;
            this._maxTimeBeforeCharge = maxTimeBeforeCharge;
        }

        public override void OnEnter()
        {
            this.Centipede.SetSpeed(this.Centipede.RegularSpeed);
            this.Centipede.SetAnimations(this._animId, true);
            this.Centipede.IsAttacking = false;
            this._firstCircling = true;

            if (this.Centipede.IsReachingTrajectoryEndNextStep())
            {
                this.Centipede.ExpandTrajectory(this.ComputeCircleAimPosition());
                this._firstCircling = false;
            }

            this._eggLayingTimer = Time.time;
            this._chargingTimer = 0f;
            this._timeBeforeCharge = Random.Range(this._minTimeBeforeCharge, this._maxTimeBeforeCharge);
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            this._chargingTimer += Time.deltaTime;
            this.Centipede.MoveAlongTrajectory();

            if (Time.time > this._eggLayingTimer + this._layingEggRate)
            {
                this.Centipede.LayEgg();
                this._eggLayingTimer = Time.time;
            }


            if (!this.Centipede.IsReachingTrajectoryEndNextStep()) return;

            if (this.Centipede.DistanceToTarget > this._chasingDistance)
            {
                this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Chase]);
            }
            else
            {
                if (this._chargingTimer > this._timeBeforeCharge)
                {
                    // choose randomly between charging or death circle
                    if (Random.Range(0, 10) > 3f)
                        this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Charge]);
                    else
                        this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.DeathCircle]);
                }

                this.Centipede.ExpandTrajectory(this.ComputeCircleAimPosition());
                this._firstCircling = false;
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        public void SetChargeTimeBounds(float a, float b)
        {
            this._minTimeBeforeCharge = a;
            this._maxTimeBeforeCharge = b;
        }

        private Vector2 ComputeCircleAimPosition()
        {
            var directionFromPlayerToHead = this.Centipede.VectorToTarget;

            // if not circling yet, choose as a node the closest cardinal point at the defined circling distance.
            if (this._firstCircling)
                this._circleDirection = VectorHelper2D.ClosestCardinalOrDiagonal(directionFromPlayerToHead);
            else
                this._circleDirection = VectorHelper2D.NextClockWiseDirection(this._circleDirection);

            var nodeDirection = VectorHelper2D.VectorFromDirection(this._circleDirection);

            return (Vector2)this.Centipede.Target.position + nodeDirection * this._circlingDistance;
        }
    }
}
