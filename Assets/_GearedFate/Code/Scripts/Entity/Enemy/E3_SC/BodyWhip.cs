using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            RegisterInitialPosition();
            StartCoroutine(Whip());
            AngleFromHead();
        }

        private IEnumerator Whip()
        {
            _timer = 0;
            ComputeAngles();
            while (_timer < _frequency)
            {
                _timer += Time.deltaTime;

                for (var i = 0; i < _bodyParts.Count; i++)
                {
                    _bodyParts[i].RotateAround(_head.transform.position, Vector3.forward,
                        _angles[i] / _frequency * Time.deltaTime);
                    _bodyParts[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            _timer = 0;
            while (_timer < _frequency)
            {
                _timer += Time.deltaTime;
                for (var i = 0; i < _bodyParts.Count; i++)
                {
                    _bodyParts[i].RotateAround(_head.transform.position, Vector3.forward,
                        -((_range + i * _rangeDelay) / _frequency) * Time.deltaTime);
                    _bodyParts[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            _timer = 0;
            while (_timer < _frequency)
            {
                _timer += Time.deltaTime;
                for (var i = 0; i < _bodyParts.Count; i++)
                {
                    _bodyParts[i].RotateAround(_head.transform.position, Vector3.forward,
                        (_range + i * _rangeDelay - _angles[i]) / _frequency * Time.deltaTime);
                    _bodyParts[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            DifferenceWithInitial();
        }

        private void AngleFromHead()
        {
            for (var i = 0; i < _bodyParts.Count; i++)
                Debug.Log(
                    $"angle bodyPart {i} : {Vector2.SignedAngle(Vector2.down, _head.position - _bodyParts[i].position)}");
        }

        private void DifferenceWithInitial()
        {
            float sum = 0;
            for (var i = 0; i < _bodyParts.Count; i++)
                sum += Vector2.Distance(_bodyParts[i].position, _initialPositions[i]);
            Debug.Log($"difference initial bodyPart : {sum}");
        }


        private void ComputeAngles()
        {
            _angles.Clear();
            for (var i = 0; i < _bodyParts.Count; i++)
                _angles.Add((_range + i * _rangeDelay) / 2 -
                            Vector2.SignedAngle(Vector2.down, _head.position - _bodyParts[i].position));
        }

        private void RegisterInitialPosition()
        {
            _initialPositions.Clear();
            for (var i = 0; i < _bodyParts.Count; i++) _initialPositions.Add(_bodyParts[i].position);
        }
    }
}
