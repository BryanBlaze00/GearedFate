using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    public class CircleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        [SerializeField] private float _spawnRate = 0.1f;

        [SerializeField] private float _moveSpeed = 5f; // Movement speed

        [SerializeField] private float _lifetime = 5f; // How long before despawning

        [SerializeField] private float _startingRadius = 0.5f;

        [SerializeField] private float _endRadiusTime = 3f;

        [SerializeField] private int _atSameTime = 6;

        [SerializeField] private float _endRadius = 4f;

        private float _offset = 0;

        private bool _isSpawning;

        public void SetSpawningState(bool isSpawning)
        {
            this._isSpawning = isSpawning;
            if (isSpawning)
            {
                this.StartCoroutine(this.SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (this._isSpawning)
            {
                this.SpawnCircles();
                yield return new WaitForSeconds(this._spawnRate);
                this._offset += 40f;
            }
        }

        private void SpawnCircles()
        {
            var directions = GetEvenlySpacedDirections(this._atSameTime, this._offset);
            for (var i = 0; i < this._atSameTime; i++)
            {
                var instance = ObjectPool.Instance.GetPooledObject(PooledObjectType.CircleString);
                instance.SetActive(true);
                instance.transform.SetParent(this.transform);
                instance.GetComponent<OpenRotatingCircleStrings>().SetAngle(Random.Range(0, 2 * Mathf.PI));
                this.StartCoroutine(this.MoveCircleAndDestroy(instance, directions[i]));
            }
        }

        private IEnumerator MoveCircleAndDestroy(GameObject obj, Vector2 direction)
        {
            var timer = 0f;
            while (timer < this._lifetime)
            {
                var t = Mathf.Min(1f, timer / this._endRadiusTime);
                obj.transform.position = (Vector2)obj.transform.position + direction * this._moveSpeed * Time.deltaTime;
                obj.GetComponent<OpenRotatingCircleStrings>().SetRadius(this._startingRadius * (1 - t) + this._endRadius * t);
                timer += Time.deltaTime;
                yield return null;
            }

            obj.SetActive(false);
        }

        private static List<Vector2> GetEvenlySpacedDirections(int count, float offset)
        {
            var directions = new List<Vector2>();

            for (var i = 0; i < count; i++)
            {
                var angle = 360f / count * i + offset; // Evenly spaced angle
                var radians = angle * Mathf.Deg2Rad; // Convert to radians

                var direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                directions.Add(direction);
            }

            return directions;
        }
    }
}
