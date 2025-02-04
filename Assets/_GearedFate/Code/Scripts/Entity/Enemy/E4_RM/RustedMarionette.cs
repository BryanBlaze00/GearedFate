namespace BTG
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// GearboundSentinel
    /// </summary>
    public class RustedMarionette : MonoBehaviour, IDamagable, IBoss
    {
        public event Action OnHitTaken;

        [field: SerializeField] public CircleSpawner CircleSpawner { get; private set; }

        [field: SerializeField] public LineRotater LineRotater { get; private set; }

        [field: SerializeField] public CircleExpander CircleExpander { get; private set; }

        [field: SerializeField] public Animator AnimatorLowPart { get; private set; }

        [field: SerializeField] public Animator AnimatorHighPart { get; private set; }

        [field: SerializeField] public float MaxHealth { get; private set; }


        private Player _player;

        private readonly FiniteStateMachine<RustedMarionetteState> _fsm = new();

        public readonly Dictionary<RustedMarionetteState, RMBaseState> _states = new();

        public float CurrentHealth { get; private set; }

        public enum RustedMarionetteState
        {
            Death = 0,
            CircleStorm = 1,
            Idle = 2,
            CirclingLines = 3,
            StringMaze = 4,
        }

        private void Start()
        {
            this._player = FindFirstObjectByType<Player>();
            var death = Animator.StringToHash(nameof(RustedMarionetteState.Death));
            var spin = Animator.StringToHash("Spin");
            this.CurrentHealth = this.MaxHealth;

            this._states.Add(RustedMarionetteState.Idle, new RMIdleState(this._fsm, this, spin, spin));
            this._states.Add(RustedMarionetteState.Death, new RMDeathState(this._fsm, this, death, death));
            this._states.Add(RustedMarionetteState.CircleStorm, new RMCircleStormState(this._fsm, this, spin, spin));
            this._states.Add(RustedMarionetteState.CirclingLines, new RMSpinningLinesState(this._fsm, this, spin, spin));
            this._states.Add(RustedMarionetteState.StringMaze, new RMStringMazeState(this._fsm, this, spin, spin));

            this._fsm.Initialize(this._states[RustedMarionetteState.CirclingLines]);
        }

        public void TakeDamage(float damage)
        {
            // Ignore hit if in maze state and player far from target
            if (this._fsm.CurrentState.GetType() == typeof(RMStringMazeState) &&
                Vector2.Distance(this._player.transform.position, this.transform.position) > 3f)
            {
                foreach (var hitFlash in this.GetComponentsInChildren<HitFlash>())
                {
                    hitFlash.SetFlashColor(Color.blue);
                    hitFlash.HitFlashRoutine();
                }

                return;
            }

            foreach (var hitFlash in this.GetComponentsInChildren<HitFlash>())
            {
                hitFlash.SetFlashColor(Color.red);
                hitFlash.HitFlashRoutine();
            }

            this.CurrentHealth = Mathf.Max(0f, this.CurrentHealth - damage);

            if (this.CurrentHealth == 0)
            {
                Elevator.Instance.ActivateElevator();
            }

            this.OnHitTaken?.Invoke();
        }

        protected void Update()
        {
            this._fsm.CurrentState.OnFrameUpdate();
        }
    }
}
