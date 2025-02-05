namespace BTG
{
    using System.Linq;
    using UnityEngine;

    public class BossStartTrigger : MonoBehaviour
    {
        public MonoBehaviour Boss;

        private void Awake()
        {
            if (TryGetComponent(out SpriteRenderer sr))
            {
                sr.enabled = false;
            }

            if (Boss == null)
            {
                Boss = (MonoBehaviour)FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None).OfType<IBoss>().FirstOrDefault();
            }

            if (Boss == null)
            {
                Debug.LogError(nameof(BossStartTrigger) + " doesn't have a boss");
            }

            Boss.gameObject.SetActive(false);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player _))
            {
                Boss.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
