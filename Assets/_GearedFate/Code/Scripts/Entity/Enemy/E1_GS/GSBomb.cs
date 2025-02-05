namespace BTG
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;

    public class GSBomb : MonoBehaviour
    {
        [SerializeField]
        private float WindupTime;

        [SerializeField]
        private float BlastRadius;

        [SerializeField]
        private float Damage;

        [SerializeField]
        private Animator Animator;

        [SerializeField]
        private Rigidbody2D rb;

        [SerializeField]
        private Collider2D col;

        [SerializeField]
        private AudioSource AudioSource;

        private void OnEnable()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }

            if (col == null)
            {
                col = GetComponents<Collider2D>().First(x => !x.isTrigger);
            }

            col.enabled = false;
            if (Animator == null)
            {
                Animator = GetComponentInChildren<Animator>();
            }

            if (AudioSource == null)
            {
                AudioSource = GetComponent<AudioSource>();
            }

            StartCoroutine(Explode());
            StartCoroutine(PhysicsThrow());
        }

        private IEnumerator Explode()
        {
            Animator.speed = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / WindupTime;
            yield return new WaitForSeconds(WindupTime - 0.15f); // yeah ok

            var hits = Physics2D.OverlapCircleAll(transform.position, BlastRadius, LayerMask.GetMask("Player"))
                .Where(x => !x.isTrigger); // the player's feet
            col.enabled = false;
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    Debug.Log("Bomb hit player");
                    player.TakeDamage(Damage);
                }
            }

            if (AudioSource != null)
            {
                AudioSource.Play();
            }

            yield return new WaitForSeconds(1.5f); // make sure explosion finished
            gameObject.SetActive(false);
        }

        private IEnumerator PhysicsThrow()
        {
            const float baseVY = 4f;
            const int baseNumBounces = 3;
            var vY = baseVY;
            var bouncesLeft = baseNumBounces - 1;
            var time = 0f;
            while (gameObject.activeInHierarchy)
            {
                const float grav = -9f;
                vY += grav * Time.fixedDeltaTime;
                Animator.transform.localPosition += new Vector3(0, vY * Time.fixedDeltaTime);
                col.enabled = Animator.transform.localPosition.y < 0.3f &&
                    time > 0.5f; // collide near ground but not at the start (the initial throw)
                if (Animator.transform.localPosition.y <= 0f)
                {
                    rb.linearVelocity /= 1.3f;
                    if (bouncesLeft == 0)
                    {
                        Animator.transform.localPosition *= new Vector2(1, 0);
                        col.enabled = true;
                        yield break;
                    }

                    vY = baseVY * (bouncesLeft / (float)baseNumBounces);
                    bouncesLeft--;
                }

                if (bouncesLeft == 0)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
        }
    }
}
