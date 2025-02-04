using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BTG
{
    public class CircleSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private float _spawnRate = 0.1f;

        [SerializeField]
        private float _moveSpeed = 5f; // Movement speed

        [SerializeField]
        private float _lifetime = 5f; // How long before despawning

        [SerializeField]
        private float _startingRadius = 0.5f;

        [SerializeField]
        private float _endRadiusTime = 3f;

        [SerializeField]
        private int _atSameTime = 6;

        [SerializeField]
        private float _endRadius = 4f;

        private float _offset = 0;

        private bool _isSpawning;

        public void SetSpawningState(bool isSpawning)
        {
            _isSpawning = isSpawning;
            if (isSpawning)
            {
                StartCoroutine(SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (_isSpawning)
            {
                SpawnCircles();
                yield return new WaitForSeconds(_spawnRate);
                _offset += 40f;
            }
        }

        private void SpawnCircles()
        {
            List<Vector2> directions = GetEvenlySpacedDirections(_atSameTime, _offset);
            for (int i = 0; i < _atSameTime; i++)
            {
                GameObject instance = ObjectPool.Instance.GetPooledObject(PooledObjectType.CircleString);
                instance.SetActive(true);
                instance.transform.SetParent(transform);
                instance.GetComponent<OpenRotatingCircleStrings>().SetAngle(Random.Range(0, 2*Mathf.PI));
                StartCoroutine(MoveCircleAndDestroy(instance, directions[i]));
            }

        }

        private IEnumerator MoveCircleAndDestroy(GameObject obj, Vector2 direction)
        {
            float timer = 0f;
            while (timer < _lifetime)
            {
                float t = Mathf.Min(1f,timer / _endRadiusTime);
                obj.transform.position = (Vector2) obj.transform.position + direction * _moveSpeed * Time.deltaTime;
                obj.GetComponent<OpenRotatingCircleStrings>().SetRadius(_startingRadius * (1-t) + _endRadius * t);
                timer += Time.deltaTime;
                yield return null;
            }

            obj.SetActive(false);
        }

        private static List<Vector2> GetEvenlySpacedDirections(int count, float offset)
        {
            List<Vector2> directions = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i + offset; // Evenly spaced angle
                float radians = angle * Mathf.Deg2Rad; // Convert to radians

                Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                directions.Add(direction);
            }

            return directions;
        }
    }
}
