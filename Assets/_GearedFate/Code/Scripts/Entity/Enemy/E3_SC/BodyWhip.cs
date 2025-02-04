using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    public class BodyWhip : MonoBehaviour
    {
        [SerializeField] private Transform _head;

        [SerializeField] private List<Transform> _bodyParts;

        // How much it rotates per second
        [SerializeField] private float _range;

        [SerializeField] private float _rangeDelay;

        // how long it takes to whip from left to right
        [SerializeField] private float _frequency;

        private float _timer = 0;

        private List<Vector2> _initialPositions = new();

        private List<float> _angles = new();

        private void Start()
        {
            this.RegisterInitialPosition();
            this.StartCoroutine(this.Whip());
            this.AngleFromHead();
        }

        private IEnumerator Whip()
        {
            this._timer = 0;
            this.ComputeAngles();
            while (this._timer < this._frequency)
            {
                this._timer += Time.deltaTime;

                for (var i = 0; i < this._bodyParts.Count; i++)
                {
                    this._bodyParts[i].RotateAround(
                        this._head.transform.position, Vector3.forward,
                        this._angles[i] / this._frequency * Time.deltaTime);
                    this._bodyParts[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            this._timer = 0;
            while (this._timer < this._frequency)
            {
                this._timer += Time.deltaTime;
                for (var i = 0; i < this._bodyParts.Count; i++)
                {
                    this._bodyParts[i].RotateAround(
                        this._head.transform.position, Vector3.forward,
                        -((this._range + i * this._rangeDelay) / this._frequency) * Time.deltaTime);
                    this._bodyParts[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            this._timer = 0;
            while (this._timer < this._frequency)
            {
                this._timer += Time.deltaTime;
                for (var i = 0; i < this._bodyParts.Count; i++)
                {
                    this._bodyParts[i].RotateAround(
                        this._head.transform.position, Vector3.forward,
                        (this._range + i * this._rangeDelay - this._angles[i]) / this._frequency * Time.deltaTime);
                    this._bodyParts[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            this.DifferenceWithInitial();
        }

        private void AngleFromHead()
        {
            for (var i = 0; i < this._bodyParts.Count; i++)
            {
                Debug.Log(
                    $"angle bodyPart {i} : {Vector2.SignedAngle(Vector2.down, this._head.position - this._bodyParts[i].position)}");
            }
        }

        private void DifferenceWithInitial()
        {
            float sum = 0;
            for (var i = 0; i < this._bodyParts.Count; i++)
            {
                sum += Vector2.Distance(this._bodyParts[i].position, this._initialPositions[i]);
            }

            Debug.Log($"difference initial bodyPart : {sum}");
        }


        private void ComputeAngles()
        {
            this._angles.Clear();
            for (var i = 0; i < this._bodyParts.Count; i++)
            {
                this._angles.Add((this._range + i * this._rangeDelay) / 2 -
                                 Vector2.SignedAngle(Vector2.down, this._head.position - this._bodyParts[i].position));
            }
        }

        private void RegisterInitialPosition()
        {
            this._initialPositions.Clear();
            for (var i = 0; i < this._bodyParts.Count; i++)
            {
                this._initialPositions.Add(this._bodyParts[i].position);
            }
        }
    }
}
