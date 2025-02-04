using System;
using UnityEngine;

namespace BTG
{
    [RequireComponent(typeof(Animator))]
    public class CentipedeBodyPart : MonoBehaviour, IDamagable
    {
        public event Action OnBodyPartDeath;

        [field: SerializeField] public Animator Animator { get; private set; }

        private int _animMoveX;

        private int _animMoveY;

        private SteamCentipede _steamCentipede;

        [field: SerializeField] public float MaxHealth { get; private set; }

        [SerializeField] private float _damageDealt = 10;

        [SerializeField] private float _knockBack = 10;

        public float CurrentHealth { get; private set; }

        protected void Start()
        {
            _animMoveX = Animator.StringToHash("MoveX");
            _animMoveY = Animator.StringToHash("MoveY");
            _steamCentipede = GetComponentInParent<SteamCentipede>();
            CurrentHealth = MaxHealth;
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                // no damage and a bit less knockback if not attacking
                player.TakeDamage(_steamCentipede.IsAttacking ? _damageDealt : 0);
                player.GetComponent<Knockback>()
                    .GetKnockedBack(transform, _steamCentipede.IsAttacking ? _knockBack : _knockBack / 3);
            }
        }

        public void SetAnimationDirectionParameter(Vector2 tangent)
        {
            Animator.SetFloat(_animMoveX, tangent.normalized.x);
            Animator.SetFloat(_animMoveY, tangent.normalized.y);
        }

        public void SetAnimationSpeed(float speed)
        {
            Animator.speed = speed;
        }

        public void PlayAnimation(int animId, float offset)
        {
            Animator.Play(animId);
            Animator.SetFloat("CycleOffset", offset);
        }

        public void TakeDamage(float damage)
        {
            if (_steamCentipede.Tail == transform)
            {
                CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
                GetComponent<HitFlash>().SetFlashColor(Color.red);
                GetComponent<HitFlash>().HitFlashRoutine();
            }
            else
            {
                GetComponent<HitFlash>().SetFlashColor(Color.blue);
                GetComponent<HitFlash>().HitFlashRoutine();
            }

            if (CurrentHealth == 0) OnBodyPartDeath?.Invoke();
        }

        public void Heal(float addedHealth)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + addedHealth, MaxHealth);
        }
    }
}
