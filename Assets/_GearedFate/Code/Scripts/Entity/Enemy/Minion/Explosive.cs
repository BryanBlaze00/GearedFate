using System.Threading.Tasks;
using UnityEngine;

namespace BTG
{
    public class Explosive : MonoBehaviour
    {
        [SerializeField] private float explosionDamage = 10.0f;

        [SerializeField] private float explodeKnockBack = 5.0f;

        [SerializeField] private float _explosionRadius = 5.0f;

        [SerializeField] private GameObject _explosionEffect;

        [SerializeField] private float _destroyWaitTime = 2f;

        public async void Explode(float afterTime)
        {
            await Task.Delay((int)afterTime * 1000);
            Instantiate(this._explosionEffect, this.transform.position, Quaternion.identity);
            this.gameObject.SetInactive(this._destroyWaitTime);
            var results = Physics2D.CircleCastAll(this.transform.position, this._explosionRadius, Vector2.up, 0f);
            foreach (var result in results)
            {
                if (result.collider.TryGetComponent(out Player player))
                {
                    player.GetComponent<Knockback>().GetKnockedBack(this.transform, this.explodeKnockBack);
                    player.TakeDamage(this.explosionDamage);
                }
            }
        }
    }
}
