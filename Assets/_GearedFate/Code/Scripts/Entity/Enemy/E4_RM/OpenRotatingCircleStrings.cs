namespace BTG
{
    using Game.Core.Rendering;
    using UnityEngine;

    public class OpenRotatingCircleStrings : MonoBehaviour
    {
        [SerializeField]
        private MultiLineRenderer2D _firstString;

        [SerializeField]
        private MultiLineRenderer2D _secondString;

        [SerializeField]
        private float _radius;

        private float _angle;

        [SerializeField]
        private int _firstStringPoints;

        [SerializeField]
        private int _secondStringPoints;

        [SerializeField]
        private int _firstEmptySpacePoints;

        [SerializeField]
        private int _secondEmptySpacePoints;

        [SerializeField]
        private float _rotatingTime;

        [SerializeField]
        private float _tolerance = 0.2f;

        [SerializeField]
        private float _damage = 1f;

        [SerializeField]
        private float _knockback = 3f;

        [SerializeField]
        private bool _turnClockwise;

        private Transform _target;

        private float _nextRotationTime;
        private float _startingAngle;
        private int _circlePointsNumber;

        public void SetDamage(float damage)
        {
            _damage = damage;
        }

        public void SetKnockBack(float knockback)
        {
            _knockback = knockback;
        }

        public void SetTolerance(float tolerance)
        {
            _tolerance = tolerance;
        }

        public void SetRotationSpeed(float timeBetweenRotate)
        {
            _rotatingTime = timeBetweenRotate;
        }

        public void SetRadius(float radius)
        {
            _radius = radius;
        }

        public void SetAngle(float angle)
        {
            _startingAngle = angle;
        }

        public void SetDirection(bool clockwise)
        {
            _turnClockwise = clockwise;
        }

        private static bool IsOnCircle(Vector2 position, Vector2 center, float radius, float tolerance)
        {
            var distanceToCenter = Vector2.Distance(position, center);
            return Mathf.Abs(distanceToCenter - radius) < tolerance;
        }

        private static Vector2 ClosestPointOnCircle(Vector2 position, Vector2 center, float radius)
        {
            var direction = (position - center).normalized; // Get direction
            return center + (direction * radius); // Scale and offset
        }

        private static bool IsPointInArc(Vector2 center, float radius, Vector2 startPoint, Vector2 endPoint, Vector2 point)
        {
            // Check if the point is on the circle
            var distSq = (point - center).sqrMagnitude;
            if (!Mathf.Approximately(distSq, radius * radius))
            {
                return false; // Not on the circle
            }

            // Compute the point's angle relative to the center
            var pointAngle = Mathf.Atan2(point.y - center.y, point.x - center.x);
            pointAngle = (pointAngle + (2 * Mathf.PI)) % (2 * Mathf.PI); // Normalize to [0, 2 pi]

            // Compute start and end angle
            var startAngle = Mathf.Atan2(startPoint.y - center.y, startPoint.x - center.x);
            startAngle = (startAngle + (2 * Mathf.PI)) % (2 * Mathf.PI);

            var endAngle = Mathf.Atan2(endPoint.y - center.y, endPoint.x - center.x);
            endAngle = (endAngle + (2 * Mathf.PI)) % (2 * Mathf.PI);

            if (startAngle < endAngle)
            {
                return startAngle <= pointAngle && pointAngle <= endAngle;
            }
            else
            {
                return startAngle < pointAngle || endAngle > pointAngle;
            }
        }

        private void Start()
        {
            _firstString.CurrentCamera = FindFirstObjectByType<Camera>();
            _secondString.CurrentCamera = FindFirstObjectByType<Camera>();
            _circlePointsNumber = _firstStringPoints + _secondStringPoints + _firstEmptySpacePoints + _secondEmptySpacePoints;
            _angle = 2 * Mathf.PI / (_circlePointsNumber - 1);
            _nextRotationTime = Time.time + _rotatingTime;
            _target = FindFirstObjectByType<Player>().transform;
        }

        // Update is called once per frame
        private void Update()
        {
            _circlePointsNumber = _firstStringPoints + _secondStringPoints + _firstEmptySpacePoints + _secondEmptySpacePoints;

            if (Time.time > _nextRotationTime)
            {
                UpdateAngle();
                _nextRotationTime = Time.time + _rotatingTime;
            }

            ComputeStringsVisuals();
            IsOnCircle();
        }

        private void ComputeStringsVisuals()
        {
            _firstString.Points.Clear();
            _secondString.Points.Clear();

            for (var i = 0; i < _circlePointsNumber; i++)
            {
                if (i < _firstStringPoints)
                {
                    _firstString.Points.Add(new Vector2(
                        Mathf.Cos(_startingAngle + (_angle * i)) * _radius,
                        Mathf.Sin(_startingAngle + (_angle * i)) * _radius));
                }

                if (i >= _firstStringPoints + _firstEmptySpacePoints &&
                    i < _firstStringPoints + _firstEmptySpacePoints + _secondStringPoints)
                {
                    _secondString.Points.Add(new Vector2(
                        Mathf.Cos(_startingAngle + (_angle * i)) * _radius,
                        Mathf.Sin(_startingAngle + (_angle * i)) * _radius));
                }
            }

            _firstString.SetMaxPoints(_firstStringPoints);
            _firstString.ApplyPointPositionChanges();
            _firstString.RefreshMaterial();

            _secondString.SetMaxPoints(_secondStringPoints);
            _secondString.ApplyPointPositionChanges();
            _secondString.RefreshMaterial();
        }

        private void UpdateAngle()
        {
            if (_turnClockwise)
            {
                _startingAngle -= _angle;
            }
            else
            {
                _startingAngle += _angle;
            }
        }

        private void IsOnCircle()
        {
            if (IsOnCircle(_target.position, transform.position, _radius, _tolerance))
            {
                var closestPointOnCircle = ClosestPointOnCircle(_target.position, transform.position, _radius);
                var startFirstEmptyPoint = _firstString.Points[^1] + (Vector2)transform.position;
                var endFirstEmptyPoint = _secondString.Points[0] + (Vector2)transform.position;

                var startSecondEmptyPoint = _secondString.Points[^1] + (Vector2)transform.position;
                var endSecondEmptyPoint = _firstString.Points[0] + (Vector2)transform.position;

                if (IsPointInArc(
                        transform.position,
                        _radius,
                        startFirstEmptyPoint,
                        endFirstEmptyPoint,
                        closestPointOnCircle)
                    || IsPointInArc(
                        transform.position,
                        _radius,
                        startSecondEmptyPoint,
                        endSecondEmptyPoint,
                        closestPointOnCircle))
                {
                    return;
                }

                _target.GetComponent<Player>().TakeDamage(_damage);
                _target.GetComponent<Player>().Knockback.GetKnockedBack(closestPointOnCircle, _knockback);
            }
        }
    }
}
