//
// Copyright (c) BTG. All rights reserved.
//

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace BTG
{
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
            Dying
        }
#if UNITY_EDITOR
        public string debugCurState;
#endif
        private readonly FiniteStateMachine<State> fsm = new();
        public readonly Dictionary<State, GSBaseState> States = new();
        public float CurrentHealth { get; private set; }
        float IBoss.MaxHealth => this.Data.MaxHealth;
        public int Phase = 0;
        public float CurDistanceGoal;

        public float CurSpeed
        {
            get => this.curSpeed;
            set
            {
                this.curSpeed = value;
                this.NavMeshAgent.speed = this.curSpeed;
                if (this.NavMeshAgent.velocity.magnitude > this.curSpeed) this.NavMeshAgent.velocity = this.NavMeshAgent.velocity.normalized * this.curSpeed;
            }
        }

        private float curSpeed;

        [Header("Assign References")] public GSData Data;
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

        //public AudioClip AudioZapClap;
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
            if (this.NavMeshAgent == null) this.NavMeshAgent = this.GetComponent<NavMeshAgent>();
            this.NavMeshAgent.updateRotation = false;
            this.NavMeshAgent.updateUpAxis = false;
            this.ResetMoveSpeed();
            this.CurrentHealth = this.Data.MaxHealth;
            this.Phase = 0;

            if (this.Animator == null) this.Animator = this.GetComponentInChildren<Animator>();
            if (this.AudioSource == null) this.AudioSource = this.GetComponent<AudioSource>();

            this.AddState(new GSIntroState(this.fsm, State.Intro, this));
            this.AddState(new GSChaseState(this.fsm, State.Chase, this));
            this.AddState(new GSBurrowState(this.fsm, State.Burrow, this));
            this.AddState(new GSShootState(this.fsm, State.Shoot, this));
            this.AddState(new GSBombState(this.fsm, State.Bomb, this));
            this.AddState(new GSDyingState(this.fsm, State.Dying, this));
        }

        public void EnableColliders()
        {
            this.GetComponents<Collider2D>().ForEach(collider => collider.enabled = true);
        }

        public void DisableColliders()
        {
            this.GetComponents<Collider2D>().ForEach(collider => collider.enabled = false);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<Knockback>(out var knockback))
                knockback.GetKnockedBack(this.transform, 5f);
        }

        public void ResetMoveSpeed()
        {
            this.CurSpeed = this.Data.BaseMoveSpeed * (1 + this.Phase * 0.3f); // 1, 1.3, 1.6
        }

        public float CalculateVolume(float delayBetweenSounds)
        {
            return Mathf.Min(-0.05f / (delayBetweenSounds + 0.05f) + 10f / 9f,
                1f); // quick formula to make fast repeated sounds not too loud
        }

        private void AddState(GSBaseState GSstate)
        {
            this.States.Add(GSstate.State, GSstate);
        }

        private void Start()
        {
            this.CurDistanceGoal = this.Data.BaseDistanceGoal;
            this.fsm.Initialize(this.States[State.Intro]);
        }

        private void Update()
        {
#if UNITY_EDITOR
            this.debugCurState = this.fsm.CurrentState.ToString();
#endif
            this.fsm.CurrentState.OnFrameUpdate();
        }

        private void FixedUpdate()
        {
            this.fsm.CurrentState.OnPhysicsUpdate();
            var corners = this.NavMeshAgent.path?.corners;

            if (corners != null && corners.Length >= 2)
            {
                var movingDir = corners[1] - this.transform.position;
                this.Animator.SetFloat("MoveDirX", movingDir.x);
                this.Animator.SetFloat("MoveDirY", movingDir.y);
            }
        }

        public void TakeDamage(float damage)
        {
            if (this.CurrentHealth <= 0)
                return; // don't repeatedly die
            this.CurrentHealth -= damage;
            this.HitFlash.HitFlashRoutine();
            if (this.CurrentHealth <= 0)
            {
                this.CurrentHealth = 0;
                this.fsm.SwitchState(this.States[State.Dying]);
            }

            if (this.Phase < this.Data.StageTransitionHealthPercentage.Count && this.CurrentHealth < this.Data.MaxHealth * this.Data.StageTransitionHealthPercentage[this.Phase]) this.Phase++;
        }
    }
}
