namespace BTG
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Splines;
    using Random = UnityEngine.Random;

    public class MoveAlongSpline : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _movingAlongSpline;

        [SerializeField]
        private Transform _toFollow;

        [SerializeField]
        private SplineContainer _splineContainer;

        [SerializeField]
        private float _regularSpeed;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _chargeSpeed;

        [SerializeField]
        private float _chargeMinimumTime;

        [SerializeField]
        private float _chargeMaximumTime;

        [SerializeField]
        private float _bodyPartDistance;

        [SerializeField]
        private float _chasingDistance = 15;

        private float _totalSplineLength;

        private List<float> _currentPositionsOnSpline = new ();

        [SerializeField]
        private float _circleDistance;

        [SerializeField]
        private Sprite _topSprite;

        [SerializeField]
        private Sprite _topRightSprite;

        [SerializeField]
        private Sprite _rightSprite;

        [SerializeField]
        private Sprite _bottomRightSprite;

        [SerializeField]
        private Sprite _bottomSprite;

        [SerializeField]
        private Sprite _bottomLeftSprite;

        [SerializeField]
        private Sprite _leftSprite;

        [SerializeField]
        private Sprite _topLeftSprite;

        private VectorHelper2D.Direction _circleDirection;

        private CentipedeState _centipedeState;

        private bool _firstCircleNodeChose = false;

        private float _timeBeforeCharge;

        private float _chargingTimer;

        private enum CentipedeState
        {
            Chasing = 0,
            Circling = 1,
            Ramming = 2,
        }

        protected void Start()
        {
            _totalSplineLength = _splineContainer.CalculateLength();
            InitializeBodyPosition();
            _speed = _regularSpeed;
        }

        protected void Update()
        {
            // Distance crossed by each transform on the spline
            var moved = Time.deltaTime * _speed;

            // Move all following transform along the spline
            for (var i = 0; i < _movingAlongSpline.Count; i++)
            {
                _currentPositionsOnSpline[i] += moved / _totalSplineLength;
                _movingAlongSpline[i].position = _splineContainer.EvaluatePosition(_currentPositionsOnSpline[i]);
                Vector3 tangent = _splineContainer.EvaluateTangent(_currentPositionsOnSpline[i]);
                SetSpriteBasedOnTangent(_movingAlongSpline[i].GetComponent<SpriteRenderer>(), tangent);
            }

            _chargingTimer += Time.deltaTime;

            // If head is about to reach the spline end, create a new knot in the spline
            if (IsReachingSplineEndNextStep(_currentPositionsOnSpline.Last(), moved))
            {
                ChooseNextState(_toFollow.position, _splineContainer.Spline.Knots.Last().Position);

                var nextNodePosition = ChooseNextNode(_toFollow.position, _splineContainer.Spline.Knots.Last().Position);
                AddNodeToSpline(nextNodePosition);
            }
        }

        private void InitializeBodyPosition()
        {
            var normalizedDistance = _bodyPartDistance / _totalSplineLength;
            for (var i = 0; i < _movingAlongSpline.Count; i++)
            {
                _currentPositionsOnSpline.Add(normalizedDistance * i);
                _movingAlongSpline[i].position = _splineContainer.EvaluatePosition(normalizedDistance * i);
            }
        }

        // Check if a given normalized spline position is after the penultimate knot.
        private bool IsReachingSplineEndNextStep(float currentSplinePosition, float moved)
        {
            return currentSplinePosition + (moved / _totalSplineLength) >= 1;
        }

        private void AddNodeToSpline(Vector3 nodePosition)
        {
            var spline = _splineContainer.Spline;
            var lengthBeforeAddingNode = _totalSplineLength;

            spline.Add(nodePosition);

            _totalSplineLength = _splineContainer.CalculateLength();

            // Gotta recompute the correct spline position for each body parts since the spline length changed.
            for (var i = 0; i < _movingAlongSpline.Count; i++)
            {
                _currentPositionsOnSpline[i] *= lengthBeforeAddingNode / _totalSplineLength;
            }
        }

        private Vector3 NextPositionBehindPlayer(Vector3 playerPosition, Vector3 lastPosition)
        {
            var direction = playerPosition - lastPosition;
            return playerPosition + (direction.normalized * 3);
        }

        private Vector3 ChooseNextNode(Vector3 playerPosition, Vector3 lastPosition)
        {
            switch (_centipedeState)
            {
                case CentipedeState.Chasing:
                    var direction = playerPosition - lastPosition;
                    return lastPosition + (direction.normalized * 3);
                case CentipedeState.Circling:
                    return ChooseCirclingNode(playerPosition, lastPosition);
                case CentipedeState.Ramming:
                    return NextPositionBehindPlayer(playerPosition, lastPosition);
                default:
                    return Vector3.zero;
            }
        }

        private void ChooseNextState(Vector3 playerPosition, Vector3 lastPosition)
        {
            // If centipede too far from its target it goes in chasing mode
            if (Vector3.Distance(playerPosition, lastPosition) > _chasingDistance)
            {
                _centipedeState = CentipedeState.Chasing;
                _speed = _regularSpeed;
            }
            else
            {
                // If not already circling, start the charge timer
                if (_centipedeState != CentipedeState.Circling)
                {
                    _firstCircleNodeChose = true;
                    StartChargeTimer();
                }

                // if charge is ready, go ramming, otherwise keep circling
                if (_chargingTimer > _timeBeforeCharge)
                {
                    _speed = _chargeSpeed;
                    _centipedeState = CentipedeState.Ramming;
                }
                else
                {
                    _speed = _regularSpeed;
                    _centipedeState = CentipedeState.Circling;
                }
            }
        }

        private void StartChargeTimer()
        {
            _chargingTimer = Time.time;
            _timeBeforeCharge = Time.time + Random.Range(_chargeMinimumTime, _chargeMaximumTime);
        }

        private Vector3 ChooseCirclingNode(Vector3 playerPosition, Vector3 lastPosition)
        {
            var directionFromPlayerToHead =
                new Vector2(playerPosition.x - lastPosition.x, playerPosition.y - lastPosition.y);

            // if not circling yet, choose as a node the closest cardinal point at the defined circling distance.
            if (_firstCircleNodeChose)
            {
                _circleDirection = VectorHelper2D.ClosestCardinalOrDiagonal(directionFromPlayerToHead);
                _firstCircleNodeChose = false;
            }
            else
            {
                _circleDirection = VectorHelper2D.NextClockWiseDirection(_circleDirection);
            }

            var nodeDirection = VectorHelper2D.VectorFromDirection(_circleDirection);
            var nodeDirection3D = new Vector3(nodeDirection.x, nodeDirection.y, 0);

            return playerPosition + (nodeDirection3D * _circleDistance);
        }

        // Based on the tangent at the spline position, set the sprite to look in the right direction.
        private void SetSpriteBasedOnTangent(SpriteRenderer renderer, Vector3 tangent)
        {
            var tangent2D = new Vector2(tangent.x, tangent.y);
            switch (VectorHelper2D.ClosestCardinalOrDiagonal(tangent2D))
            {
                case VectorHelper2D.Direction.Top:
                    renderer.sprite = _topSprite;
                    break;
                case VectorHelper2D.Direction.TopRight:
                    renderer.sprite = _topRightSprite;
                    break;
                case VectorHelper2D.Direction.Right:
                    renderer.sprite = _rightSprite;
                    break;
                case VectorHelper2D.Direction.BottomRight:
                    renderer.sprite = _bottomRightSprite;
                    break;
                case VectorHelper2D.Direction.Bottom:
                    renderer.sprite = _bottomSprite;
                    break;
                case VectorHelper2D.Direction.BottomLeft:
                    renderer.sprite = _bottomLeftSprite;
                    break;
                case VectorHelper2D.Direction.Left:
                    renderer.sprite = _leftSprite;
                    break;
                case VectorHelper2D.Direction.TopLeft:
                    renderer.sprite = _topLeftSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
