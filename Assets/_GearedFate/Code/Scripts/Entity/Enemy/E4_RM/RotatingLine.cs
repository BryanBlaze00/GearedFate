using Game.Core.Rendering;
using UnityEngine;

namespace BTG
{
    public class RotatingLine : MonoBehaviour
    {
        [SerializeField] private MultiLineRenderer2D _line;

        [Tooltip("Rotation speed in radian/s")] [SerializeField]
        private float _rotationSpeed;

        [Tooltip("the whole line has two time this lenght")] [SerializeField]
        private float _radius;

        private float _angle;

        [SerializeField] private float _tolerance = 0.1f;

        private Transform _target;

        [SerializeField] private float _damage = 2f;

        [SerializeField] private float _knockback = 3f;

        private float _nextRotationTime;

        [SerializeField] private float _rotatingTime = 0.1f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            this._target = FindFirstObjectByType<Player>().transform;
            this._line.CurrentCamera = FindFirstObjectByType<Camera>();
        }

        // Update is called once per frame
        private void Update()
        {
            this._angle += this._rotationSpeed * Time.deltaTime;

            if (Time.time > this._nextRotationTime)
            {
                this._nextRotationTime = Time.time + this._rotatingTime;
                this._angle += this._rotationSpeed * this._rotatingTime;
                this._line.Points[0] = new Vector2(Mathf.Cos(this._angle) * this._radius, Mathf.Sin(this._angle) * this._radius);
                this._line.Points[1] = -new Vector2(Mathf.Cos(this._angle) * this._radius, Mathf.Sin(this._angle) * this._radius);
                this._line.ApplyPointPositionChanges();
                this._line.RefreshMaterial();
            }

            this.IsOnLine();
        }

        public void SetAngle(float angle)
        {
            this._angle = angle;
        }

        public void SetSpeed(float speed)
        {
            this._rotationSpeed = speed;
        }

        private void IsOnLine()
        {
            var closestPointOnLine = GetClosestPointOnLine(
                this._line.Points[0] + (Vector2)this.transform.parent.position,
                this._line.Points[1] + (Vector2)this.transform.parent.position,
                this._target.position);
            if (Vector2.Distance(closestPointOnLine, this._target.position) < this._tolerance)
            {
                this._target.GetComponent<Player>().TakeDamage(this._damage);
                this._target.GetComponent<Player>().Knockback.GetKnockedBack(closestPointOnLine, this._knockback);
            }
        }

        private static Vector2 GetClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
        {
            var AB = B - A; // Line direction
            var AP = P - A; // Vector from A to P

            var t = Vector2.Dot(AP, AB) / Vector2.Dot(AB, AB); // Projection factor
            t = Mathf.Clamp01(t); // Clamp to segment

            return A + t * AB; // Closest point on the segment
        }
    }
}
