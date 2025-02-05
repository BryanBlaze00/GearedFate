// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// GearboundSentinel
    /// Phase 0: Shoots bullets and throws bombs while avoiding player
    ///  - Random between bombs and narrow shotgun spray
    ///  - Trying to keep to a target distance
    /// Phase 1: Starts to Burrow, but become more agressive
    ///  - Shotgun spray when in close-mid range
    ///  - Occasionally switch target distance to closer values, including 0?
    /// Phase 2: More chaotic movement and attack flurries, perhaps combo them
    ///  - Spinning 360 spray right before burrowing?
    ///  - Throw bombs when pop up from burrow?
    ///  - Switch target distance constantly? Increase movement speed?
    ///
    /// Different bullet patterns:
    ///  - shotgun spray (narrow or wide)
    ///  - spinning 360 degree spray
    ///  - direct shot at the player (or predicting player)
    /// </summary>
    public class GearboundSentinel : MonoBehaviour, IDamagable, IBoss
    {
        private readonly FiniteStateMachine<State> _fsm = new ();

        private readonly Dictionary<State, GSBaseState> _states = new ();

        private float _curSpeed;

        public enum State
        {
            Intro,
            Chase,
            Burrow,
            Shoot,
            Bomb,
            Dying,
        }

        public int Phase { get; set; }

        public float CurDistanceGoal { get; set; }

        public float CurrentHealth { get; private set; }

        float IBoss.MaxHealth => Data.MaxHealth;

        public float CurSpeed
        {
            get => _curSpeed;
            set
            {
                _curSpeed = value;
                NavMeshAgent.speed = _curSpeed;
                if (NavMeshAgent.velocity.magnitude > _curSpeed)
                {
                    NavMeshAgent.velocity = NavMeshAgent.velocity.normalized * _curSpeed;
                }
            }
        }

        [Header("Assign References")]
        [field:SerializeField]
        public GSData Data { get; private set; }

        [field:SerializeField]
        public NavMeshAgent NavMeshAgent { get; private set; }

        [field:SerializeField]
        public Animator Animator { get; private set; }

        [field:SerializeField]
        public Player TargetPlayer { get; set; }

        [field:SerializeField]
        public GameObject BombPrefab { get; private set; }

        [field:SerializeField]
        public ProjectileData ProjectileData { get; private set; }

        [field:SerializeField]
        public GameObject Shadow { get; private set; }

        [field:SerializeField]
        public HitFlash HitFlash { get; private set; }

        [field:SerializeField]
        public AudioSource AudioSource { get; private set; }

        [field:SerializeField]
        public AudioClip AudioBombThrow { get; private set; }

        [field:SerializeField]
        public AudioClip AudioBurrow { get; private set; }

        [field:SerializeField]
        public AudioClip AudioBurrowing { get; private set; }

        [field:SerializeField]
        public AudioClip AudioUnBurrow { get; private set; }

        [field:SerializeField]
        public AudioClip AudioLaserShot { get; private set; }

        [field:SerializeField]
        public AudioClip AudioDeath { get; private set; }

        [field:SerializeField]
        public AudioClip AudioMovement { get; private set; }

        [field:SerializeField]
        public Transform EyeShootUpPos { get; private set; }

        [field:SerializeField]
        public Transform EyeShootRightPos { get; private set; }

        [field:SerializeField]
        public Transform EyeShootDownPos { get; private set; }

        [field:SerializeField]
        public Transform EyeShootLeftPos { get; private set; }

        [field:SerializeField]
        public Transform BombUpPos { get; private set; }

        [field:SerializeField]
        public Transform BombRightPos { get; private set; }

        [field:SerializeField]
        public Transform BombDownPos { get; private set; }

        [field:SerializeField]
        public Transform BombLeftPos { get; private set; }

        public GSBaseState this[State key] => _states[key];

        public void EnableColliders()
        {
            GetComponents<Collider2D>().ForEach(collider => collider.enabled = true);
        }

        public void DisableColliders()
        {
            GetComponents<Collider2D>().ForEach(collider => collider.enabled = false);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<Knockback>(out var knockback))
            {
                knockback.GetKnockedBack(transform, 5f);
            }
        }

        public void ResetMoveSpeed()
        {
            CurSpeed = Data.BaseMoveSpeed * (1 + (Phase * 0.3f)); // 1, 1.3, 1.6
        }

        public float CalculateVolume(float delayBetweenSounds)
        {
            return Mathf.Min(
                (-0.05f / (delayBetweenSounds + 0.05f)) + (10f / 9f),
                1f); // quick formula to make fast repeated sounds not too loud
        }

        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0)
            {
                return; // don't repeatedly die
            }

            CurrentHealth -= damage;
            HitFlash.HitFlashRoutine();
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                _fsm.SwitchState(_states[State.Dying]);
            }

            if (Phase < Data.StageTransitionHealthPercentage.Count && CurrentHealth < Data.MaxHealth * Data.StageTransitionHealthPercentage[Phase])
            {
                Phase++;
            }
        }

        private void Awake()
        {
            if (NavMeshAgent == null)
            {
                NavMeshAgent = GetComponent<NavMeshAgent>();
            }

            NavMeshAgent.updateRotation = false;
            NavMeshAgent.updateUpAxis = false;
            ResetMoveSpeed();
            CurrentHealth = Data.MaxHealth;
            Phase = 0;

            if (Animator == null)
            {
                Animator = GetComponentInChildren<Animator>();
            }

            if (AudioSource == null)
            {
                AudioSource = GetComponent<AudioSource>();
            }

            AddState(new GSIntroState(_fsm, State.Intro, this));
            AddState(new GSChaseState(_fsm, State.Chase, this));
            AddState(new GSBurrowState(_fsm, State.Burrow, this));
            AddState(new GSShootState(_fsm, State.Shoot, this));
            AddState(new GSBombState(_fsm, State.Bomb, this));
            AddState(new GSDyingState(_fsm, State.Dying, this));
        }

        private void AddState(GSBaseState gsState)
        {
            _states.Add(gsState.State, gsState);
        }

        private void Start()
        {
            CurDistanceGoal = Data.BaseDistanceGoal;
            _fsm.Initialize(_states[State.Intro]);
        }

        private void Update()
        {
            _fsm.CurrentState.OnFrameUpdate();
        }

        private void FixedUpdate()
        {
            _fsm.CurrentState.OnPhysicsUpdate();
            var corners = NavMeshAgent.path?.corners;

            if (corners != null && corners.Length >= 2)
            {
                var movingDir = corners[1] - transform.position;
                Animator.SetFloat("MoveDirX", movingDir.x);
                Animator.SetFloat("MoveDirY", movingDir.y);
            }
        }
    }
}
