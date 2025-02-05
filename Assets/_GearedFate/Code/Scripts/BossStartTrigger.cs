namespace BTG
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class BossStartTrigger : MonoBehaviour
    {
        [FormerlySerializedAs("Boss")]
        [SerializeField]
        private MonoBehaviour _boss;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player _))
            {
                _boss.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            if (TryGetComponent(out SpriteRenderer sr))
            {
                sr.enabled = false;
            }

            if (_boss == null)
            {
                _boss = (MonoBehaviour)FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None).OfType<IBoss>().FirstOrDefault();
            }

            if (_boss == null)
            {
                Debug.LogError(nameof(BossStartTrigger) + " doesn't have a boss");
            }

            _boss.gameObject.SetActive(false);
        }
    }
}
