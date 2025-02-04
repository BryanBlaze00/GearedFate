using System;
using System.Threading;
using Game.Core.Rendering;
using UnityEngine;

namespace BTG
{
    public class OpenRotatingCircleStrings : MonoBehaviour
    {
        [SerializeField] private MultiLineRenderer2D _firstString;

        [SerializeField] private MultiLineRenderer2D _secondString;

        [SerializeField] private float _radius;

        private float _angle;

        [SerializeField] private int _firstStringPoints;

        [SerializeField] private int _secondStringPoints;

        [SerializeField] private int _firstEmptySpacePoints;

        [SerializeField] private int _secondEmptySpacePoints;

        [SerializeField] private float _rotatingTime;

        [SerializeField] private float _tolerance = 0.2f;

        [SerializeField] private float _damage = 1f;

        [SerializeField] private float _knockback = 3f;

        [SerializeField] private bool _turnClockwise;

        private Transform _target;

        private float _nextRotationTime;
        private float _startingAngle;
        private int _circlePointsNumber;

        private void Start()
        {
            this._firstString.CurrentCamera = FindFirstObjectByType<Camera>();
            this._secondString.CurrentCamera = FindFirstObjectByType<Camera>();
            this._circlePointsNumber = this._firstStringPoints + this._secondStringPoints + this._firstEmptySpacePoints + this._secondEmptySpacePoints;
            this._angle = 2 * Mathf.PI / (this._circlePointsNumber - 1);
            this._nextRotationTime = Time.time + this._rotatingTime;
            this._target = FindFirstObjectByType<Player>().transform;
        }

        public void SetDamage(float damage)
        {
            this._damage = damage;
        }

        public void SetKnockBack(float knockback)
        {
            this._knockback = knockback;
        }

        public void SetTolerance(float tolerance)
        {
            this._tolerance = tolerance;
        }

        public void SetRotationSpeed(float timeBetweenRotate)
        {
            this._rotatingTime = timeBetweenRotate;
        }

        public void SetRadius(float radius)
        {
            this._radius = radius;
        }

        public void SetAngle(float angle)
        {
            this._startingAngle = angle;
        }

        public void SetDirection(bool clockwise)
        {
            this._turnClockwise = clockwise;
        }

        // Update is called once per frame
        private void Update()
        {
            this._circlePointsNumber = this._firstStringPoints + this._secondStringPoints + this._firstEmptySpacePoints + this._secondEmptySpacePoints;

            if (Time.time > this._nextRotationTime)
            {
                this.UpdateAngle();
                this._nextRotationTime = Time.time + this._rotatingTime;
            }

            this.ComputeStringsVisuals();
            this.IsOnCircle();
        }

        private void ComputeStringsVisuals()
        {
            this._firstString.Points.Clear();
            this._secondString.Points.Clear();

            for (var i = 0; i < this._circlePointsNumber; i++)
            {
                if (i < this._firstStringPoints)
                    this._firstString.Points.Add(new Vector2(Mathf.Cos(this._startingAngle + this._angle * i) * this._radius,
                        Mathf.Sin(this._startingAngle + this._angle * i) * this._radius));
                if (i >= this._firstStringPoints + this._firstEmptySpacePoints &&
                    i < this._firstStringPoints + this._firstEmptySpacePoints + this._secondStringPoints)
                    this._secondString.Points.Add(new Vector2(Mathf.Cos(this._startingAngle + this._angle * i) * this._radius,
                        Mathf.Sin(this._startingAngle + this._angle * i) * this._radius));
            }

            this._firstString.SetMaxPoints(this._firstStringPoints);
            this._firstString.ApplyPointPositionChanges();
            this._firstString.RefreshMaterial();

            this._secondString.SetMaxPoints(this._secondStringPoints);
            this._secondString.ApplyPointPositionChanges();
            this._secondString.RefreshMaterial();
        }

        private void UpdateAngle()
        {
            if (this._turnClockwise)
                this._startingAngle -= this._angle;
            else
                this._startingAngle += this._angle;
        }

        private void IsOnCircle()
        {
            if (IsOnCircle(this._target.position, this.transform.position, this._radius, this._tolerance))
            {
                var closestPointOnCircle = ClosestPointOnCircle(this._target.position, this.transform.position, this._radius);
                var startFirstEmptyPoint = this._firstString.Points[^1] + (Vector2)this.transform.position;
                var endFirstEmptyPoint = this._secondString.Points[0] + (Vector2)this.transform.position;

                var startSecondEmptyPoint = this._secondString.Points[^1] + (Vector2)this.transform.position;
                var endSecondEmptyPoint = this._firstString.Points[0] + (Vector2)this.transform.position;


                if (IsPointInArc(
                        this.transform.position,
                        this._radius, startFirstEmptyPoint, endFirstEmptyPoint,
                        closestPointOnCircle)
                    || IsPointInArc(
                        this.transform.position,
                        this._radius, startSecondEmptyPoint, endSecondEmptyPoint,
                        closestPointOnCircle))
                    return;

                this._target.GetComponent<Player>().TakeDamage(this._damage);
                this._target.GetComponent<Player>().Knockback.GetKnockedBack(closestPointOnCircle, this._knockback);
            }
        }

        private static bool IsOnCircle(Vector2 position, Vector2 center, float radius, float tolerance)
        {
            var distanceToCenter = Vector2.Distance(position, center);
            return Mathf.Abs(distanceToCenter - radius) < tolerance;
        }

        private static Vector2 ClosestPointOnCircle(Vector2 position, Vector2 center, float radius)
        {
            var direction = (position - center).normalized; // Get direction
            return center + direction * radius; // Scale and offset
        }

        private static bool IsPointInArc(Vector2 center, float radius, Vector2 startPoint, Vector2 endPoint,
            Vector2 point)
        {
            // Check if the point is on the circle
            var distSq = (point - center).sqrMagnitude;
            if (!Mathf.Approximately(distSq, radius * radius))
                return false; // Not on the circle

            // Compute the point's angle relative to the center
            var pointAngle = Mathf.Atan2(point.y - center.y, point.x - center.x);
            pointAngle = (pointAngle + 2 * Mathf.PI) % (2 * Mathf.PI); // Normalize to [0, 2 pi]

            // Compute start and end angle
            var startAngle = Mathf.Atan2(startPoint.y - center.y, startPoint.x - center.x);
            startAngle = (startAngle + 2 * Mathf.PI) % (2 * Mathf.PI);

            var endAngle = Mathf.Atan2(endPoint.y - center.y, endPoint.x - center.x);
            endAngle = (endAngle + 2 * Mathf.PI) % (2 * Mathf.PI);

            if (startAngle < endAngle)
                return startAngle <= pointAngle && pointAngle <= endAngle;
            else
                return startAngle < pointAngle || endAngle > pointAngle;
        }
    }
}
