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
            this._animMoveX = Animator.StringToHash("MoveX");
            this._animMoveY = Animator.StringToHash("MoveY");
            this._steamCentipede = this.GetComponentInParent<SteamCentipede>();
            this.CurrentHealth = this.MaxHealth;
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                // no damage and a bit less knockback if not attacking
                player.TakeDamage(this._steamCentipede.IsAttacking ? this._damageDealt : 0);
                player.GetComponent<Knockback>()
                    .GetKnockedBack(this.transform, this._steamCentipede.IsAttacking ? this._knockBack : this._knockBack / 3);
            }
        }

        public void SetAnimationDirectionParameter(Vector2 tangent)
        {
            this.Animator.SetFloat(this._animMoveX, tangent.normalized.x);
            this.Animator.SetFloat(this._animMoveY, tangent.normalized.y);
        }

        public void SetAnimationSpeed(float speed)
        {
            this.Animator.speed = speed;
        }

        public void PlayAnimation(int animId, float offset)
        {
            this.Animator.Play(animId);
            this.Animator.SetFloat("CycleOffset", offset);
        }

        public void TakeDamage(float damage)
        {
            if (this._steamCentipede.Tail == this.transform)
            {
                this.CurrentHealth = Mathf.Max(this.CurrentHealth - damage, 0);
                this.GetComponent<HitFlash>().SetFlashColor(Color.red);
                this.GetComponent<HitFlash>().HitFlashRoutine();
            }
            else
            {
                this.GetComponent<HitFlash>().SetFlashColor(Color.blue);
                this.GetComponent<HitFlash>().HitFlashRoutine();
            }

            if (this.CurrentHealth == 0)
            {
                this.OnBodyPartDeath?.Invoke();
            }
        }

        public void Heal(float addedHealth)
        {
            this.CurrentHealth = Mathf.Min(this.CurrentHealth + addedHealth, this.MaxHealth);
        }
    }
}
