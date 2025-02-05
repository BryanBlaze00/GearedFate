namespace BTG
{
    using Game.Core.Rendering;
    using UnityEngine;

    public class RotatingLine : MonoBehaviour
    {
        [SerializeField]
        private MultiLineRenderer2D _line;

        [Tooltip("Rotation speed in radian/s")]
        [SerializeField]
        private float _rotationSpeed;

        [Tooltip("the whole line has two time this lenght")]
        [SerializeField]
        private float _radius;

        private float _angle;

        [SerializeField]
        private float _tolerance = 0.1f;

        private Transform _target;

        [SerializeField]
        private float _damage = 2f;

        [SerializeField]
        private float _knockback = 3f;

        private float _nextRotationTime;

        [SerializeField]
        private float _rotatingTime = 0.1f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _target = FindFirstObjectByType<Player>().transform;
            _line.CurrentCamera = FindFirstObjectByType<Camera>();
        }

        // Update is called once per frame
        private void Update()
        {
            _angle += _rotationSpeed * Time.deltaTime;

            if (Time.time > _nextRotationTime)
            {
                _nextRotationTime = Time.time + _rotatingTime;
                _angle += _rotationSpeed * _rotatingTime;
                _line.Points[0] = new Vector2(Mathf.Cos(_angle) * _radius, Mathf.Sin(_angle) * _radius);
                _line.Points[1] = -new Vector2(Mathf.Cos(_angle) * _radius, Mathf.Sin(_angle) * _radius);
                _line.ApplyPointPositionChanges();
                _line.RefreshMaterial();
            }

            IsOnLine();
        }

        public void SetAngle(float angle)
        {
            _angle = angle;
        }

        public void SetSpeed(float speed)
        {
            _rotationSpeed = speed;
        }

        private void IsOnLine()
        {
            var closestPointOnLine = GetClosestPointOnLine(
                _line.Points[0] + (Vector2)transform.parent.position,
                _line.Points[1] + (Vector2)transform.parent.position,
                _target.position);
            if (Vector2.Distance(closestPointOnLine, _target.position) < _tolerance)
            {
                _target.GetComponent<Player>().TakeDamage(_damage);
                _target.GetComponent<Player>().Knockback.GetKnockedBack(closestPointOnLine, _knockback);
            }
        }

        private static Vector2 GetClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
        {
            var AB = B - A; // Line direction
            var AP = P - A; // Vector from A to P

            var t = Vector2.Dot(AP, AB) / Vector2.Dot(AB, AB); // Projection factor
            t = Mathf.Clamp01(t); // Clamp to segment

            return A + (t * AB); // Closest point on the segment
        }
    }
}
