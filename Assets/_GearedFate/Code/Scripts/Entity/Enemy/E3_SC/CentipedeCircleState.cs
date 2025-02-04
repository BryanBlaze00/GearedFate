namespace BTG
{
    using UnityEngine;

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
            _animId = animationId;
            _chasingDistance = chasingDistance;
            _circlingDistance = circlingDistance;
            _minTimeBeforeCharge = minTimeBeforeCharge;
            _maxTimeBeforeCharge = maxTimeBeforeCharge;
        }

        public override void OnEnter()
        {
            Centipede.SetSpeed(Centipede.RegularSpeed);
            Centipede.SetAnimations(_animId, true);
            Centipede.IsAttacking = false;
            _firstCircling = true;

            if (Centipede.IsReachingTrajectoryEndNextStep())
            {
                Centipede.ExpandTrajectory(ComputeCircleAimPosition());
                _firstCircling = false;
            }

            _eggLayingTimer = Time.time;
            _chargingTimer = 0f;
            _timeBeforeCharge = Random.Range(_minTimeBeforeCharge, _maxTimeBeforeCharge);
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            _chargingTimer += Time.deltaTime;
            Centipede.MoveAlongTrajectory();

            if (Time.time > _eggLayingTimer + _layingEggRate)
            {
                Centipede.LayEgg();
                _eggLayingTimer = Time.time;
            }

            if (!Centipede.IsReachingTrajectoryEndNextStep())
            {
                return;
            }

            if (Centipede.DistanceToTarget > _chasingDistance)
            {
                fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Chase]);
            }
            else
            {
                if (_chargingTimer > _timeBeforeCharge)
                {
                    // choose randomly between charging or death circle
                    if (Random.Range(0, 10) > 3f)
                    {
                        fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Charge]);
                    }
                    else
                    {
                        fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.DeathCircle]);
                    }
                }

                Centipede.ExpandTrajectory(ComputeCircleAimPosition());
                _firstCircling = false;
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        public void SetChargeTimeBounds(float a, float b)
        {
            _minTimeBeforeCharge = a;
            _maxTimeBeforeCharge = b;
        }

        private Vector2 ComputeCircleAimPosition()
        {
            var directionFromPlayerToHead = Centipede.VectorToTarget;

            // if not circling yet, choose as a node the closest cardinal point at the defined circling distance.
            if (_firstCircling)
            {
                _circleDirection = VectorHelper2D.ClosestCardinalOrDiagonal(directionFromPlayerToHead);
            }
            else
            {
                _circleDirection = VectorHelper2D.NextClockWiseDirection(_circleDirection);
            }

            var nodeDirection = VectorHelper2D.VectorFromDirection(_circleDirection);

            return (Vector2)Centipede.Target.position + (nodeDirection * _circlingDistance);
        }
    }
}
