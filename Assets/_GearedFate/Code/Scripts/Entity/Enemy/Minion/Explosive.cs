using System.Threading.Tasks;
using UnityEngine;

namespace BTG
{
    public class Explosive : MonoBehaviour
    {
        [SerializeField]
        private float explosionDamage = 10.0f;

        [SerializeField]
        private float explodeKnockBack = 5.0f;

        [SerializeField]
        private float _explosionRadius = 5.0f;

        [SerializeField]
        private GameObject _explosionEffect;

        [SerializeField]
        private float _destroyWaitTime = 2f;

        public async void Explode(float afterTime)
        {
            await Task.Delay((int) afterTime * 1000);
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            gameObject.SetInactive(_destroyWaitTime);
            RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, _explosionRadius, Vector2.up, 0f);
            foreach (RaycastHit2D result in results)
            {
                if (result.collider.TryGetComponent(out Player player))
                {
                    player.GetComponent<Knockback>().GetKnockedBack(transform, explodeKnockBack);
                    player.TakeDamage(explosionDamage);
                }
            }
        }


    }
}
