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
        public enum State
        {
            Intro,
            Chase,
            Burrow,
            Shoot,
            Bomb,
            Dying,
        }

#if UNITY_EDITOR
        public string debugCurState;
#endif
        private readonly FiniteStateMachine<State> fsm = new();
        public readonly Dictionary<State, GSBaseState> States = new();

        public float CurrentHealth { get; private set; }

        float IBoss.MaxHealth => Data.MaxHealth;

        public int Phase = 0;
        public float CurDistanceGoal;

        public float CurSpeed
        {
            get => curSpeed;
            set
            {
                curSpeed = value;
                NavMeshAgent.speed = curSpeed;
                if (NavMeshAgent.velocity.magnitude > curSpeed)
                {
                    NavMeshAgent.velocity = NavMeshAgent.velocity.normalized * curSpeed;
                }
            }
        }

        private float curSpeed;

        [Header("Assign References")]
        public GSData Data;
        public NavMeshAgent NavMeshAgent;
        public Animator Animator;
        public Player TargetPlayer;
        public GameObject BombPrefab;
        public ProjectileData ProjectileData;
        public GameObject Shadow;
        public HitFlash HitFlash;
        public AudioSource AudioSource;
        public AudioClip AudioBombThrow;
        public AudioClip AudioBurrow;
        public AudioClip AudioBurrowing;
        public AudioClip AudioUnBurrow;

        public AudioClip AudioLaserShot;

        // public AudioClip AudioZapClap;
        public AudioClip AudioDeath;
        public AudioClip AudioMovement;

        public Transform EyeShootUpPos;
        public Transform EyeShootRightPos;
        public Transform EyeShootDownPos;
        public Transform EyeShootLeftPos;
        public Transform BombUpPos;
        public Transform BombRightPos;
        public Transform BombDownPos;
        public Transform BombLeftPos;

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

            AddState(new GSIntroState(fsm, State.Intro, this));
            AddState(new GSChaseState(fsm, State.Chase, this));
            AddState(new GSBurrowState(fsm, State.Burrow, this));
            AddState(new GSShootState(fsm, State.Shoot, this));
            AddState(new GSBombState(fsm, State.Bomb, this));
            AddState(new GSDyingState(fsm, State.Dying, this));
        }

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

        private void AddState(GSBaseState GSstate)
        {
            States.Add(GSstate.State, GSstate);
        }

        private void Start()
        {
            CurDistanceGoal = Data.BaseDistanceGoal;
            fsm.Initialize(States[State.Intro]);
        }

        private void Update()
        {
#if UNITY_EDITOR
            debugCurState = fsm.CurrentState.ToString();
#endif
            fsm.CurrentState.OnFrameUpdate();
        }

        private void FixedUpdate()
        {
            fsm.CurrentState.OnPhysicsUpdate();
            var corners = NavMeshAgent.path?.corners;

            if (corners != null && corners.Length >= 2)
            {
                var movingDir = corners[1] - transform.position;
                Animator.SetFloat("MoveDirX", movingDir.x);
                Animator.SetFloat("MoveDirY", movingDir.y);
            }
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
                fsm.SwitchState(States[State.Dying]);
            }

            if (Phase < Data.StageTransitionHealthPercentage.Count && CurrentHealth < Data.MaxHealth * Data.StageTransitionHealthPercentage[Phase])
            {
                Phase++;
            }
        }
    }
}
