using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace BTG
{
    public class MoveAlongSpline : MonoBehaviour
    {
        [SerializeField] private List<Transform> _movingAlongSpline;

        [SerializeField] private Transform _toFollow;

        [SerializeField] private SplineContainer _splineContainer;

        [SerializeField] private float _regularSpeed;

        [SerializeField] private float _speed;

        [SerializeField] private float _chargeSpeed;

        [SerializeField] private float _chargeMinimumTime;

        [SerializeField] private float _chargeMaximumTime;

        [SerializeField] private float _bodyPartDistance;

        [SerializeField] private float _chasingDistance = 15;

        private float _totalSplineLength;

        private List<float> _currentPositionsOnSpline = new();

        [SerializeField] private float _circleDistance;

        [SerializeField] private Sprite _topSprite;

        [SerializeField] private Sprite _topRightSprite;

        [SerializeField] private Sprite _rightSprite;

        [SerializeField] private Sprite _bottomRightSprite;

        [SerializeField] private Sprite _bottomSprite;

        [SerializeField] private Sprite _bottomLeftSprite;

        [SerializeField] private Sprite _leftSprite;

        [SerializeField] private Sprite _topLeftSprite;

        private VectorHelper2D.Direction _circleDirection;

        private CentipedeState _centipedeState;

        private bool _firstCircleNodeChose = false;

        private float _timeBeforeCharge;

        private float _chargingTimer;


        private enum CentipedeState
        {
            Chasing = 0,
            Circling = 1,
            Ramming = 2
        }

        protected void Start()
        {
            this._totalSplineLength = this._splineContainer.CalculateLength();
            this.InitializeBodyPosition();
            this._speed = this._regularSpeed;
        }

        protected void Update()
        {
            // Distance crossed by each transform on the spline
            var moved = Time.deltaTime * this._speed;

            // Move all following transform along the spline
            for (var i = 0; i < this._movingAlongSpline.Count; i++)
            {
                this._currentPositionsOnSpline[i] += moved / this._totalSplineLength;
                this._movingAlongSpline[i].position = this._splineContainer.EvaluatePosition(this._currentPositionsOnSpline[i]);
                Vector3 tangent = this._splineContainer.EvaluateTangent(this._currentPositionsOnSpline[i]);
                this.SetSpriteBasedOnTangent(this._movingAlongSpline[i].GetComponent<SpriteRenderer>(), tangent);
            }

            this._chargingTimer += Time.deltaTime;

            // If head is about to reach the spline end, create a new knot in the spline
            if (this.IsReachingSplineEndNextStep(this._currentPositionsOnSpline.Last(), moved))
            {
                this.ChooseNextState(this._toFollow.position, this._splineContainer.Spline.Knots.Last().Position);

                var nextNodePosition = this.ChooseNextNode(this._toFollow.position, this._splineContainer.Spline.Knots.Last().Position);
                this.AddNodeToSpline(nextNodePosition);
            }
        }

        private void InitializeBodyPosition()
        {
            var normalizedDistance = this._bodyPartDistance / this._totalSplineLength;
            for (var i = 0; i < this._movingAlongSpline.Count; i++)
            {
                this._currentPositionsOnSpline.Add(normalizedDistance * i);
                this._movingAlongSpline[i].position = this._splineContainer.EvaluatePosition(normalizedDistance * i);
            }
        }

        // Check if a given normalized spline position is after the penultimate knot.
        private bool IsReachingSplineEndNextStep(float currentSplinePosition, float moved)
        {
            return currentSplinePosition + moved / this._totalSplineLength >= 1;
        }

        private void AddNodeToSpline(Vector3 nodePosition)
        {
            var spline = this._splineContainer.Spline;
            var lengthBeforeAddingNode = this._totalSplineLength;

            spline.Add(nodePosition);

            this._totalSplineLength = this._splineContainer.CalculateLength();

            // Gotta recompute the correct spline position for each body parts since the spline length changed.
            for (var i = 0; i < this._movingAlongSpline.Count; i++) this._currentPositionsOnSpline[i] *= lengthBeforeAddingNode / this._totalSplineLength;
        }

        private Vector3 NextPositionBehindPlayer(Vector3 playerPosition, Vector3 lastPosition)
        {
            var direction = playerPosition - lastPosition;
            return playerPosition + direction.normalized * 3;
        }

        private Vector3 ChooseNextNode(Vector3 playerPosition, Vector3 lastPosition)
        {
            switch (this._centipedeState)
            {
                case CentipedeState.Chasing:
                    var direction = playerPosition - lastPosition;
                    return lastPosition + direction.normalized * 3;
                case CentipedeState.Circling:
                    return this.ChooseCirclingNode(playerPosition, lastPosition);
                case CentipedeState.Ramming:
                    return this.NextPositionBehindPlayer(playerPosition, lastPosition);
                default:
                    return Vector3.zero;
            }
        }

        private void ChooseNextState(Vector3 playerPosition, Vector3 lastPosition)
        {
            // If centipede too far from its target it goes in chasing mode
            if (Vector3.Distance(playerPosition, lastPosition) > this._chasingDistance)
            {
                this._centipedeState = CentipedeState.Chasing;
                this._speed = this._regularSpeed;
            }
            else
            {
                // If not already circling, start the charge timer
                if (this._centipedeState != CentipedeState.Circling)
                {
                    this._firstCircleNodeChose = true;
                    this.StartChargeTimer();
                }

                // if charge is ready, go ramming, otherwise keep circling
                if (this._chargingTimer > this._timeBeforeCharge)
                {
                    this._speed = this._chargeSpeed;
                    this._centipedeState = CentipedeState.Ramming;
                }
                else
                {
                    this._speed = this._regularSpeed;
                    this._centipedeState = CentipedeState.Circling;
                }
            }
        }

        private void StartChargeTimer()
        {
            this._chargingTimer = Time.time;
            this._timeBeforeCharge = Time.time + Random.Range(this._chargeMinimumTime, this._chargeMaximumTime);
        }

        private Vector3 ChooseCirclingNode(Vector3 playerPosition, Vector3 lastPosition)
        {
            var directionFromPlayerToHead =
                new Vector2(playerPosition.x - lastPosition.x, playerPosition.y - lastPosition.y);

            // if not circling yet, choose as a node the closest cardinal point at the defined circling distance.
            if (this._firstCircleNodeChose)
            {
                this._circleDirection = VectorHelper2D.ClosestCardinalOrDiagonal(directionFromPlayerToHead);
                this._firstCircleNodeChose = false;
            }
            else
            {
                this._circleDirection = VectorHelper2D.NextClockWiseDirection(this._circleDirection);
            }

            var nodeDirection = VectorHelper2D.VectorFromDirection(this._circleDirection);
            var nodeDirection3D = new Vector3(nodeDirection.x, nodeDirection.y, 0);

            return playerPosition + nodeDirection3D * this._circleDistance;
        }

        // Based on the tangent at the spline position, set the sprite to look in the right direction.
        private void SetSpriteBasedOnTangent(SpriteRenderer renderer, Vector3 tangent)
        {
            var tangent2D = new Vector2(tangent.x, tangent.y);
            switch (VectorHelper2D.ClosestCardinalOrDiagonal(tangent2D))
            {
                case VectorHelper2D.Direction.Top:
                    renderer.sprite = this._topSprite;
                    break;
                case VectorHelper2D.Direction.TopRight:
                    renderer.sprite = this._topRightSprite;
                    break;
                case VectorHelper2D.Direction.Right:
                    renderer.sprite = this._rightSprite;
                    break;
                case VectorHelper2D.Direction.BottomRight:
                    renderer.sprite = this._bottomRightSprite;
                    break;
                case VectorHelper2D.Direction.Bottom:
                    renderer.sprite = this._bottomSprite;
                    break;
                case VectorHelper2D.Direction.BottomLeft:
                    renderer.sprite = this._bottomLeftSprite;
                    break;
                case VectorHelper2D.Direction.Left:
                    renderer.sprite = this._leftSprite;
                    break;
                case VectorHelper2D.Direction.TopLeft:
                    renderer.sprite = this._topLeftSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
