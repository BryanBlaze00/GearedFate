namespace BTG
{
    using Game.Core.Rendering;
    using UnityEngine;

    [RequireComponent(typeof(MultiLineRenderer2D))]
    public class CircleString : MonoBehaviour
    {
        private MultiLineRenderer2D _multiLine;

        [SerializeField]
        private float _radius;

        private float _angle;

        [SerializeField]
        private int _pointsNumber;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _multiLine = GetComponent<MultiLineRenderer2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            _multiLine.Points.Clear();
            _angle = 2 * Mathf.PI / (_pointsNumber - 1);

            for (var i = 0; i < _pointsNumber; i++)
            {
                _multiLine.Points.Add(new Vector2(Mathf.Sin(_angle * i) * _radius, Mathf.Cos(_angle * i) * _radius));
            }

            _multiLine.SetMaxPoints(_pointsNumber + 2);
            _multiLine.ApplyPointPositionChanges();
            _multiLine.RefreshMaterial();
        }
    }
}
