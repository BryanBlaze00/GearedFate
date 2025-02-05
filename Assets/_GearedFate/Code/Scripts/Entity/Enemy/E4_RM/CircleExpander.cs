namespace BTG
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CircleExpander : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private float _spawnRate = 0.1f;

        [SerializeField]
        private float _expandSpeed = 1f; // Movement speed

        [SerializeField]
        private float _lifetime = 5f; // How long before despawning

        [SerializeField]
        private float _startingRadius = 0.5f;

        [SerializeField]
        private float _circleTolerance = 0.1f;

        [SerializeField]
        private int _quickCircleNumber = 25;

        [SerializeField]
        private float _timeBetweenQuickCircle = 0.3f;

        [SerializeField]
        private MinMaxFloat _rotationUpdate;

        private bool _alternate;

        private bool _state;

        private List<GameObject> _instances = new();

        public void SetCircleExpand(bool state)
        {
            _state = state;
            if (state)
            {
                StartCoroutine(SpawnSlowCircles());
                StartCoroutine(SpawnQuickCircle());
            }
            else
            {
                for (var i = _instances.Count - 1; i >= 0; i--)
                {
                    Destroy(_instances[i].gameObject);
                }

                _instances.Clear();
            }
        }

        private IEnumerator SpawnQuickCircle()
        {
            for (var i = 0; i < _quickCircleNumber; i++)
            {
                SpawnPrefab(_rotationUpdate.Min, Random.Range(0, 2 * Mathf.PI), 3f, 2f);
                yield return new WaitForSeconds(_timeBetweenQuickCircle);
            }
        }

        private IEnumerator SpawnSlowCircles()
        {
            while (_state)
            {
                SpawnPrefab(Random.Range(_rotationUpdate.Min, _rotationUpdate.Max), Random.Range(0, 2 * Mathf.PI),
                    _expandSpeed,
                    _lifetime);
                yield return new WaitForSeconds(_spawnRate);
            }
        }

        private void SpawnPrefab(float rotationSpeed, float angle, float expandingSpeed, float lifeTime)
        {
            _alternate = !_alternate;
            var instance = Instantiate(_prefab, transform);
            _instances.Add(instance);
            instance.GetComponent<OpenRotatingCircleStrings>().SetAngle(angle);
            instance.GetComponent<OpenRotatingCircleStrings>().SetDirection(_alternate);
            instance.GetComponent<OpenRotatingCircleStrings>().SetRotationSpeed(rotationSpeed);
            instance.GetComponent<OpenRotatingCircleStrings>().SetTolerance(_circleTolerance);
            StartCoroutine(ExpandAndDestroy(instance, expandingSpeed, lifeTime));
        }

        private IEnumerator ExpandAndDestroy(GameObject obj, float expandingSpeed, float lifeTime)
        {
            var timer = 0f;
            while (timer < lifeTime)
            {
                obj.GetComponent<OpenRotatingCircleStrings>().SetRadius(_startingRadius + (timer * expandingSpeed));
                timer += Time.deltaTime;
                yield return null;
            }

            _instances.Remove(obj);
            Destroy(obj);
        }
    }
}
