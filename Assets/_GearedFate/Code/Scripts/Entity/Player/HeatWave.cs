using UnityEngine;

namespace BTG
{
    public class HeatWave : MonoBehaviour
    {
        [field: SerializeField] public int Damage { get; private set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamagable damagable)) damagable.TakeDamage(Damage);
        }

        public void OnAnimationFinished()
        {
            Destroy(gameObject);
        }
    }
}
