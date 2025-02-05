namespace BTG
{
    using UnityEngine;

    public class HeatWave : MonoBehaviour
    {
        [field: SerializeField]
        public int Damage { get; private set; }

        public void OnAnimationFinished()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(Damage);
            }
        }
    }
}
