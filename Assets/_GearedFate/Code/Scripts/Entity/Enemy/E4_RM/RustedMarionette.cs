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
        private readonly FiniteStateMachine<RustedMarionetteState> _fsm = new ();

        private readonly Dictionary<RustedMarionetteState, RMBaseState> _states = new ();

        private Player _player;

        public event Action OnHitTaken;

        public enum RustedMarionetteState
        {
            Death = 0,
            CircleStorm = 1,
            Idle = 2,
            CirclingLines = 3,
            StringMaze = 4,
        }

        [field: SerializeField]
        public CircleSpawner CircleSpawner { get; private set; }

        [field: SerializeField]
        public LineRotater LineRotater { get; private set; }

        [field: SerializeField]
        public CircleExpander CircleExpander { get; private set; }

        [field: SerializeField]
        public Animator AnimatorLowPart { get; private set; }

        [field: SerializeField]
        public Animator AnimatorHighPart { get; private set; }

        [field: SerializeField]
        public float MaxHealth { get; private set; }

        public float CurrentHealth { get; private set; }

        public RMBaseState this[RustedMarionetteState key] => _states[key];

        public void TakeDamage(float damage)
        {
            // Ignore hit if in maze state and player far from target
            if (_fsm.CurrentState.GetType() == typeof(RMStringMazeState) &&
                Vector2.Distance(_player.transform.position, transform.position) > 3f)
            {
                foreach (var hitFlash in GetComponentsInChildren<HitFlash>())
                {
                    hitFlash.SetFlashColor(Color.blue);
                    hitFlash.HitFlashRoutine();
                }

                return;
            }

            foreach (var hitFlash in GetComponentsInChildren<HitFlash>())
            {
                hitFlash.SetFlashColor(Color.red);
                hitFlash.HitFlashRoutine();
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

            if (CurrentHealth == 0)
            {
                Elevator.Instance.ActivateElevator();
            }

            OnHitTaken?.Invoke();
        }

        protected void Update()
        {
            _fsm.CurrentState.OnFrameUpdate();
        }

        protected void Start()
        {
            _player = FindFirstObjectByType<Player>();
            var death = Animator.StringToHash(nameof(RustedMarionetteState.Death));
            var spin = Animator.StringToHash("Spin");
            CurrentHealth = MaxHealth;

            _states.Add(RustedMarionetteState.Idle, new RMIdleState(_fsm, this, spin, spin));
            _states.Add(RustedMarionetteState.Death, new RMDeathState(_fsm, this, death, death));
            _states.Add(RustedMarionetteState.CircleStorm, new RMCircleStormState(_fsm, this, spin, spin));
            _states.Add(RustedMarionetteState.CirclingLines, new RMSpinningLinesState(_fsm, this, spin, spin));
            _states.Add(RustedMarionetteState.StringMaze, new RMStringMazeState(_fsm, this, spin, spin));

            _fsm.Initialize(_states[RustedMarionetteState.CirclingLines]);
        }
    }
}
