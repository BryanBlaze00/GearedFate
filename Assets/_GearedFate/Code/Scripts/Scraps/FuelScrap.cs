namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Scrap to refuel the player.
    /// </summary>
    public class FuelScrap : Scrap
    {
        [SerializeField]
        private float _fuelAdded;

        public override void ApplyEffect(IAffectable player)
        {
            if (player is IRefuelable refuelable)
            {
                refuelable.Refuel(_fuelAdded);
            }

            var listedPoolObjects = ObjectPool.Instance.GetAllPooledObjects(PooledObjectType.Fuel_Scrap);
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
