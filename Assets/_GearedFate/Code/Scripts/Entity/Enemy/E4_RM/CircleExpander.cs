using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BTG
{
    public class CircleExpander : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        [SerializeField] private float _spawnRate = 0.1f;

        [SerializeField] private float _expandSpeed = 1f; // Movement speed

        [SerializeField] private float _lifetime = 5f; // How long before despawning

        [SerializeField] private float _startingRadius = 0.5f;

        [SerializeField] private float _circleTolerance = 0.1f;

        [SerializeField] private int _quickCircleNumber = 25;

        [SerializeField] private float _timeBetweenQuickCircle = 0.3f;

        [SerializeField] private MinMaxFloat _rotationUpdate;

        private bool _alternate;

        private bool _state;

        private List<GameObject> _instances = new();

        public void SetCircleExpand(bool state)
        {
            this._state = state;
            if (state)
            {
                this.StartCoroutine(this.SpawnSlowCircles());
                this.StartCoroutine(this.SpawnQuickCircle());
            }
            else
            {
                for (var i = this._instances.Count - 1; i >= 0; i--) Destroy(this._instances[i].gameObject);
                this._instances.Clear();
            }
        }

        private IEnumerator SpawnQuickCircle()
        {
            for (var i = 0; i < this._quickCircleNumber; i++)
            {
                this.SpawnPrefab(this._rotationUpdate.Min, Random.Range(0, 2 * Mathf.PI), 3f, 2f);
                yield return new WaitForSeconds(this._timeBetweenQuickCircle);
            }
        }

        private IEnumerator SpawnSlowCircles()
        {
            while (this._state)
            {
                this.SpawnPrefab(Random.Range(this._rotationUpdate.Min, this._rotationUpdate.Max), Random.Range(0, 2 * Mathf.PI),
                    this._expandSpeed,
                    this._lifetime);
                yield return new WaitForSeconds(this._spawnRate);
            }
        }

        private void SpawnPrefab(float rotationSpeed, float angle, float expandingSpeed, float lifeTime)
        {
            this._alternate = !this._alternate;
            var instance = Instantiate(this._prefab, this.transform);
            this._instances.Add(instance);
            instance.GetComponent<OpenRotatingCircleStrings>().SetAngle(angle);
            instance.GetComponent<OpenRotatingCircleStrings>().SetDirection(this._alternate);
            instance.GetComponent<OpenRotatingCircleStrings>().SetRotationSpeed(rotationSpeed);
            instance.GetComponent<OpenRotatingCircleStrings>().SetTolerance(this._circleTolerance);
            this.StartCoroutine(this.ExpandAndDestroy(instance, expandingSpeed, lifeTime));
        }

        private IEnumerator ExpandAndDestroy(GameObject obj, float expandingSpeed, float lifeTime)
        {
            var timer = 0f;
            while (timer < lifeTime)
            {
                obj.GetComponent<OpenRotatingCircleStrings>().SetRadius(this._startingRadius + timer * expandingSpeed);
                timer += Time.deltaTime;
                yield return null;
            }

            this._instances.Remove(obj);
            Destroy(obj);
        }
    }
}
