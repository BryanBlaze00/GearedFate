using Game.Core.Rendering;
using UnityEngine;

namespace BTG
{
    [RequireComponent(typeof(MultiLineRenderer2D))]
    public class CircleString : MonoBehaviour
    {
        private MultiLineRenderer2D _multiLine;

        [SerializeField] private float _radius;

        private float _angle;

        [SerializeField] private int _pointsNumber;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            this._multiLine = this.GetComponent<MultiLineRenderer2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            this._multiLine.Points.Clear();
            this._angle = 2 * Mathf.PI / (this._pointsNumber - 1);

            for (var i = 0; i < this._pointsNumber; i++)
            {
                this._multiLine.Points.Add(new Vector2(Mathf.Sin(this._angle * i) * this._radius, Mathf.Cos(this._angle * i) * this._radius));
            }

            this._multiLine.SetMaxPoints(this._pointsNumber + 2);
            this._multiLine.ApplyPointPositionChanges();
            this._multiLine.RefreshMaterial();
        }
    }
}
