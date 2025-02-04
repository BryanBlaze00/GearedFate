namespace BTG
{
    using System.Collections.Generic;
    using UnityEngine;

    public class LineRotater : MonoBehaviour
    {
        [SerializeField]
        private GameObject _linePrefab;

        [SerializeField]
        private GameObject _protectionPrefab;

        [SerializeField]
        private float _spawnRateProtection = 0.1f;

        [SerializeField]
        private float _protectionExpandSpeed = 1f;

        [SerializeField]
        private float _protectionPeriod = 2f; // Movement speed

        [SerializeField]
        private float _lifetimeProtection = 5f; // How long before despawning

        [SerializeField]
        private float _startingRadiusProtection = 0.5f;

        [SerializeField]
        private float _circleTolerance = 0.1f;

        [SerializeField]
        private int _lineNumber = 1;

        [SerializeField]
        private float _lineSpeed = 1f;

        [SerializeField]

        // private bool _rotateClockwise = true;
        private GameObject _protectionInstance;

        private List<RotatingLine> _rotatingLines = new ();

        private bool _isSpawning;

        public void SetLineState(bool isSpawning)
        {
            _isSpawning = isSpawning;
            if (isSpawning)
            {
                var angles = GetEvenlySpacedAngles(_lineNumber, 0);

                for (var i = 0; i < _lineNumber; i++)
                {
                    var instance = ObjectPool.Instance.GetPooledObject(PooledObjectType.LineString);
                    instance.transform.SetParent(transform);
                    instance.SetActive(true);

                    var rotatingLine = instance.GetComponent<RotatingLine>();
                    _rotatingLines.Add(rotatingLine);
                    rotatingLine.SetAngle(angles[i]);
                    rotatingLine.SetSpeed(_lineSpeed);
                }
            }
            else
            {
                for (var i = _rotatingLines.Count - 1; i >= 0; i--)
                {
                    _rotatingLines[i].gameObject.SetActive(false);
                }

                _rotatingLines.Clear();

                if (_protectionInstance != null)
                {
                    _protectionInstance.SetActive(false);
                }
            }
        }

        private static List<float> GetEvenlySpacedAngles(int count, float offset)
        {
            var directions = new List<float>();

            for (var i = 0; i < count; i++)
            {
                var angle = (2 * Mathf.PI / count * i) + offset; // Evenly spaced angle
                directions.Add(angle);
            }

            return directions;
        }
    }
}
