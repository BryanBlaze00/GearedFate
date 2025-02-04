using System.Collections;
using System.Linq;
using UnityEngine;

namespace BTG
{
    public class GSBomb : MonoBehaviour
    {
        public float WindupTime;
        public float BlastRadius;
        public float Damage;

        public Animator Animator;
        public Rigidbody2D rb;
        public Collider2D col;
        public AudioSource AudioSource;

        private void OnEnable()
        {
            if (this.rb == null)
            {
                this.rb = this.GetComponent<Rigidbody2D>();
            }

            if (this.col == null)
            {
                this.col = this.GetComponents<Collider2D>().First(x => !x.isTrigger);
            }

            this.col.enabled = false;
            if (this.Animator == null)
            {
                this.Animator = this.GetComponentInChildren<Animator>();
            }

            if (this.AudioSource == null)
            {
                this.AudioSource = this.GetComponent<AudioSource>();
            }

            this.StartCoroutine(this.Explode());
            this.StartCoroutine(this.PhysicsThrow());
        }

        private IEnumerator Explode()
        {
            this.Animator.speed = this.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / this.WindupTime;
            yield return new WaitForSeconds(this.WindupTime - 0.15f); // yeah ok

            var hits = Physics2D.OverlapCircleAll(this.transform.position, this.BlastRadius, LayerMask.GetMask("Player"))
                .Where(x => !x.isTrigger); // the player's feet
            this.col.enabled = false;
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    Debug.Log("Bomb hit player");
                    player.TakeDamage(this.Damage);
                }
            }

            if (this.AudioSource != null)
            {
                this.AudioSource.Play();
            }

            yield return new WaitForSeconds(1.5f); // make sure explosion finished
            this.gameObject.SetActive(false);
        }

        private IEnumerator PhysicsThrow()
        {
            const float baseVY = 4f;
            const int baseNumBounces = 3;
            var vY = baseVY;
            var bouncesLeft = baseNumBounces - 1;
            var time = 0f;
            while (this.gameObject.activeInHierarchy)
            {
                const float grav = -9f;
                vY += grav * Time.fixedDeltaTime;
                this.Animator.transform.localPosition += new Vector3(0, vY * Time.fixedDeltaTime);
                this.col.enabled = this.Animator.transform.localPosition.y < 0.3f &&
                    time > 0.5f; // collide near ground but not at the start (the initial throw)
                if (this.Animator.transform.localPosition.y <= 0f)
                {
                    this.rb.linearVelocity /= 1.3f;
                    if (bouncesLeft == 0)
                    {
                        this.Animator.transform.localPosition *= new Vector2(1, 0);
                        this.col.enabled = true;
                        yield break;
                    }

                    vY = baseVY * (bouncesLeft / (float)baseNumBounces);
                    bouncesLeft--;
                }

                if (bouncesLeft == 0)
                {
                    this.rb.linearVelocity = Vector2.zero;
                }

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
        }
    }
}
