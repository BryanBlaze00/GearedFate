using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Scrap to heal the player.
    /// </summary>
    public class HealthScrap : Scrap
    {
        [SerializeField]
        private float _healthAdded;
        public override void ApplyEffect(IAffectable affectable)
        {
            if (affectable is IHealable healable)
            {
                healable.Heal(_healthAdded);
            }

            var listedPoolObjects = ObjectPool.Instance.GetAllPooledObjects(PooledObjectType.Health_Scrap);
            foreach (var pooledObject in listedPoolObjects)
            {
                if (pooledObject.activeSelf)
                {
                    ObjectPool.Instance.ReturnPooledObject(gameObject);
                    break;
                }
                else
                {
                    Destroy(gameObject); // Destroy the scrap when it reaches the player if not in the pool.
                    break;
                }
            }
        }
    }
}
