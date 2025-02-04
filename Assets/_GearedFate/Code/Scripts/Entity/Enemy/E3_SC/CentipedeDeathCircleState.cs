namespace BTG
{
    using UnityEngine;

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
            _animId = animationId;
        }

        public override void OnEnter()
        {
            Centipede.IsAttacking = true;
            Centipede.SetSpeed(Centipede.ChargeSpeed);
            Centipede.SetAnimations(_animId, true);
            _originalTargetPosition = Centipede.Target.position;
            _initialDistanceToTarget = Centipede.DistanceToTarget;
            _firstCircling = true;
            _timer = 0;

            if (Centipede.IsReachingTrajectoryEndNextStep())
            {
                Centipede.ExpandTrajectory(ComputeDeathCirclePosition());
            }
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            _timer += Time.deltaTime;
            Centipede.MoveAlongTrajectory();

            if (!Centipede.IsReachingTrajectoryEndNextStep())
            {
                return;
            }

            if (Vector2.Distance(Centipede.Target.position, _originalTargetPosition) > _initialDistanceToTarget)
            {
                fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Charge]);
            }
            else
            {
                if (_timer > _timeToReachMinimalDistance)
                {
                    fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Charge]);
                }

                Centipede.ExpandTrajectory(ComputeDeathCirclePosition());
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        private Vector2 ComputeDeathCirclePosition()
        {
            var directionFromPlayerToHead = Centipede.VectorToTarget;

            // if not circling yet, choose as a node the closest cardinal point at the defined circling distance.
            if (_firstCircling)
            {
                _circleDirection = VectorHelper2D.ClosestCardinalOrDiagonal(directionFromPlayerToHead);
                _firstCircling = false;
            }
            else
            {
                _circleDirection = VectorHelper2D.NextClockWiseDirection(_circleDirection);
            }

            var nodeDirection = VectorHelper2D.VectorFromDirection(_circleDirection);

            var circlingDistance = (_initialDistanceToTarget / 2 * (1 - (_timer / _timeToReachMinimalDistance))) + (_finalDistanceToTarget * _timer / _timeToReachMinimalDistance);

            return (Vector2)_originalTargetPosition + (nodeDirection * circlingDistance);
        }
    }
}
