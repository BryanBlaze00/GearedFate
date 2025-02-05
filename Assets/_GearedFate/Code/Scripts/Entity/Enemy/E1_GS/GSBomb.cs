namespace BTG
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class GSBomb : MonoBehaviour
    {
        [FormerlySerializedAs("WindupTime")]
        [SerializeField]
        private float _windupTime;

        [FormerlySerializedAs("BlastRadius")]
        [SerializeField]
        private float _blastRadius;

        [FormerlySerializedAs("Damage")]
        [SerializeField]
        private float _damage;

        [FormerlySerializedAs("Animator")]
        [SerializeField]
        private Animator _animator;

        [FormerlySerializedAs("rb")]
        [SerializeField]
        private Rigidbody2D _rb;

        [FormerlySerializedAs("col")]
        [SerializeField]
        private Collider2D _col;

        [FormerlySerializedAs("AudioSource")]
        [SerializeField]
        private AudioSource _audioSource;

        private void OnEnable()
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody2D>();
            }

            if (_col == null)
            {
                _col = GetComponents<Collider2D>().First(x => !x.isTrigger);
            }

            _col.enabled = false;
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            StartCoroutine(Explode());
            StartCoroutine(PhysicsThrow());
        }

        private IEnumerator Explode()
        {
            _animator.speed = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / _windupTime;
            yield return new WaitForSeconds(_windupTime - 0.15f); // yeah ok

            var hits = Physics2D.OverlapCircleAll(transform.position, _blastRadius, LayerMask.GetMask("Player"))
                .Where(x => !x.isTrigger); // the player's feet
            _col.enabled = false;
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    Debug.Log("Bomb hit player");
                    player.TakeDamage(_damage);
                }
            }

            if (_audioSource != null)
            {
                _audioSource.Play();
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
                _animator.transform.localPosition += new Vector3(0, vY * Time.fixedDeltaTime);
                _col.enabled = _animator.transform.localPosition.y < 0.3f &&
                    time > 0.5f; // collide near ground but not at the start (the initial throw)
                if (_animator.transform.localPosition.y <= 0f)
                {
                    _rb.linearVelocity /= 1.3f;
                    if (bouncesLeft == 0)
                    {
                        _animator.transform.localPosition *= new Vector2(1, 0);
                        _col.enabled = true;
                        yield break;
                    }

                    vY = baseVY * (bouncesLeft / (float)baseNumBounces);
                    bouncesLeft--;
                }

                if (bouncesLeft == 0)
                {
                    _rb.linearVelocity = Vector2.zero;
                }

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
        }
    }
}
