//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// DestructableObject class to provide destructible object functionality.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DestructableObject : MonoBehaviour, IDamagable
    {
        [SerializeField] private float _maxHealth = 10f;
        [SerializeField] private GameObject _destroyEffect;

        [Header("Randomization Settings")] [SerializeField] [Range(0, 3)]
        private int _minAmountToDrop = 1;

        [SerializeField] [Range(0, 3)] private int _maxAmountToDrop = 3;
        [SerializeField] [Range(1, 100)] private int _chanceToDrop = 30;

        // ObjectBreakAnim _objectBreakAnim; TODO: FIX ME
        private float _currentHealth;

        private void Awake()
        {
            // _objectBreakAnim = GetComponent<ObjectBreakAnim>(); TODO: FIX ME
            this._currentHealth = this._maxHealth;
        }

        public void TakeDamage(float damage)
        {
            this._currentHealth -= damage;

            if (this._currentHealth <= 0)
            {
                if (this._destroyEffect != null)
                {
                    Instantiate(
                        this._destroyEffect,
                        this.transform.position,
                        Quaternion.identity); // Instantiate the destroy effect
                }

                this.RandomizedItemDrop();
                // doesn't work to update navmesh:
                //GameObject.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None).ForEach(x => x.UpdateNavMesh(x.navMeshData));
                Destroy(this.gameObject); // Destroy the object
            }
        }

        /// <summary>
        /// Randomized scrap amount and type to drop when destructible object is destroyed.
        /// </summary>
        private void RandomizedItemDrop()
        {
            var randAmount = RandomUtilily.RandomInt(this._minAmountToDrop, this._maxAmountToDrop); /// Randomized scrap amount.

            for (var i = 0; i < randAmount; i++)
            {
                var randObj = RandomUtilily.RandomInt(1, 2); // Randomized object to drop
                var randChance = RandomUtilily.Chance(this._chanceToDrop); // Randomized chance to drop object

                if (randObj == 1 && randChance)
                {
                    var obj = ObjectPool.Instance.GetPooledObject(PooledObjectType.Fuel_Scrap);
                    obj.transform.position = this.transform.position;
                    obj.SetActive(true);
                }
                else if (randObj == 2 && randChance)
                {
                    var obj = ObjectPool.Instance.GetPooledObject(PooledObjectType.Health_Scrap);
                    obj.transform.position = this.transform.position;
                    obj.SetActive(true);
                }
            }
        }
    }
}
