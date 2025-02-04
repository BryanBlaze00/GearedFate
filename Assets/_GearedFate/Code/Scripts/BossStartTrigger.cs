using System.Linq;
using UnityEngine;

namespace BTG
{
    public class BossStartTrigger : MonoBehaviour
    {
        public MonoBehaviour Boss;

        private void Awake()
        {
            if (this.TryGetComponent(out SpriteRenderer sr))
            {
                sr.enabled = false;
            }

            if (this.Boss == null)
            {
                this.Boss = (MonoBehaviour)FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,
                    FindObjectsSortMode.None).OfType<IBoss>().FirstOrDefault();
            }

            if (this.Boss == null)
            {
                Debug.LogError(nameof(BossStartTrigger) + " doesn't have a boss");
            }

            this.Boss.gameObject.SetActive(false);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player _))
            {
                this.Boss.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
    }
}
