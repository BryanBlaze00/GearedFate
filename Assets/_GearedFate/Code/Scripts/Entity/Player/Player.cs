//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace BTG
{
    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, IDamagable, IDataPersistence, IHealable, IRefuelable
    {
        public enum State
        {
            Idle,
            Move,
            Dash,
            HeatWave,
            Slash,
            GearToss,
            FireBlaze,
            Hit,
            Dead
        }

        //TODO: if hit, immobilize/ invulnerabity for a moment and continue flashing (done from main script)

        #region Player Control Fields

        [field: SerializeField] public PlayerData Data { get; private set; }
        public PlayerInputHandler Input { get; private set; }
        private readonly FiniteStateMachine<State> fsm = new();
        public readonly Dictionary<State, PlayerBaseState> states = new();
        [HideInInspector] public bool isInvulnerable;
        public float CurrentHealth { get; private set; }
        public float CurrentAttackFuelAmount { get; private set; }
        public Vector2 CurrentDirection { get; private set; }

        public float OutOfBoundsTime = 0f;

        private readonly List<State> abilities = new();
        private int currentAbilityIndex;
        public State CurrentAbility { get; private set; }

        #endregion Player Control Fields

        #region Reference Fields

        [field: SerializeField] public PlayerInfoSO PlayerInfo { get; private set; }
        [field: SerializeField] public ProjectileData GearData { get; private set; }
        [field: SerializeField] public GameObject HeatWave { get; private set; }
        [field: SerializeField] public GameObject Blaze { get; private set; }
        [field: SerializeField] public Transform ShootPos { get; private set; }
        [field: SerializeField] public Transform SlashPos { get; private set; }
        [SerializeField] private new SpriteRenderer renderer;
        public PlayerAnimationEventHandler AnimEvent { get; private set; }
        public Knockback Knockback { get; private set; }
        public Rigidbody2D RB { get; private set; }
        public Animator Anim { get; private set; }
        public int AnimMoveX { get; private set; }
        public int AnimMoveY { get; private set; }
        private HitFlash hitFlash;

        private float _lastHit;

        #endregion Reference Fields

        public AudioClip HurtAudio;

        public AudioClip ShootGearAudio;

        public AudioClip HealAudio;

        public AudioClip DashAudio;

        public AudioClip SlashAudio;

        public AudioClip DeathAudio;

        public AudioClip FlameBeam;

        #region Unity CallBacks

        private void Awake()
        {
            this.RB = this.GetComponent<Rigidbody2D>();
            this.Input = this.GetComponent<PlayerInputHandler>();
            var body = this.transform.Find("Graphics").Find("Body");
            this.Anim = body.GetComponent<Animator>();
            this.AnimEvent = body.GetComponent<PlayerAnimationEventHandler>();
            this.hitFlash = this.GetComponentInChildren<HitFlash>();
            this.Knockback = this.GetComponentInChildren<Knockback>();

            this.states.Add(State.Idle, new PlayerIdleState(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.Idle))));
            this.states.Add(State.Move, new PlayerMoveState(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.Move))));
            this.states.Add(State.Dash, new PlayerDashState(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.Dash))));
            this.states.Add(State.Slash, new PlayerSlashState(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.Slash))));
            this.states.Add(State.HeatWave,
                new PlayerHeatWave(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.HeatWave))));
            this.states.Add(State.GearToss,
                new PlayerGearTossState(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.GearToss))));
            this.states.Add(State.FireBlaze,
                new PlayerFireBlazeState(
                    this.fsm, this,
                    this.Data,
                    Animator.StringToHash("ChargeUp"))); ///Charges up before plays fireblaze Animation
            this.states.Add(State.Hit,
                new PlayerHitState(
                    this.fsm, this,
                    this.Data,
                    Animator.StringToHash(nameof(State.Idle)))); //TODO: change to hit (No animation yet)
            this.states.Add(State.Dead, new PlayerDeadState(this.fsm, this, this.Data, Animator.StringToHash(nameof(State.Dead))));

            this.abilities.Add(State.Slash);
            this.abilities.Add(State.FireBlaze);
            this.abilities.Add(State.HeatWave);
            this.abilities.Add(State.GearToss);
        }

        private void Start()
        {
            this.CurrentHealth = this.Data.Health;
            this.CurrentAttackFuelAmount = this.Data.MaxAttackFuelAmount;
            this.CurrentDirection = Vector2.down;
            this.currentAbilityIndex = 0;
            this.CurrentAbility = this.abilities[this.currentAbilityIndex];
            this.Blaze.SetActive(false);
            this.AnimMoveX = Animator.StringToHash("MoveX");
            this.AnimMoveY = Animator.StringToHash("MoveY");
            this.fsm.Initialize(this.states[State.Idle]);
        }

        private void Update()
        {
            this.fsm.CurrentState.OnFrameUpdate();
            if (this._lastHit + 1f < Time.time)
            {
                this.isInvulnerable = false;
            }
        }

        private void FixedUpdate()
        {
            this.fsm.CurrentState.OnPhysicsUpdate();
            // make sure the player isn't stuck out of bounds
            var walkMask = 1 << NavMesh.GetAreaFromName("Walkable");

            var inBounds = NavMesh.SamplePosition(this.transform.position, out var hit, 0.1f, walkMask);
            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName.Contains("Boss") || sceneName.Contains("boss")) // it's getting hackier and hackier
            {
                if (inBounds)
                {
                    this.OutOfBoundsTime--;
                    if (this.OutOfBoundsTime < 0f)
                    {
                        this.OutOfBoundsTime = 0f;
                    }
                }
                else
                {
                    this.OutOfBoundsTime += Time.fixedDeltaTime;
                    if (this.OutOfBoundsTime >= 6f)
                    {
                        //Debug.LogError("Player out of bounds!");
                        var found = false;
                        var searchRadius = 0.5f;
                        while (!found && searchRadius < 10f)
                        {
                            found = NavMesh.SamplePosition(this.transform.position, out hit, searchRadius, walkMask);
                            searchRadius += 0.5f;
                        }

                        if (found)
                        {
                            this.transform.position = hit.position;
                        }
                    }
                }
            }
        }

        private void LateUpdate()
        {
            this.MyDebug();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (this.isInvulnerable)
            {
                return;
            }
        }

        #endregion Unity CallBacks

        #region Other Methods

        public void SetLookDir()
        {
            if (this.Input.MoveInput == Vector2.zero)
            {
                return;
            }

            this.CurrentDirection = this.Input.MoveInput;
            this.Anim.SetFloat(this.AnimMoveX, this.Input.MoveInput.x);
            this.Anim.SetFloat(this.AnimMoveY, this.Input.MoveInput.y);
            this.CheckIfShouldFlip();
        }

        public void CheckIfShouldFlip()
        {
            this.renderer.flipX = this.Input.MoveInput.x == -1;
        }

        public void BurnAttackFuel(float fuel)
        {
            this.CurrentAttackFuelAmount -= fuel;
            if (this.CurrentAttackFuelAmount < 0)
            {
                this.CurrentAttackFuelAmount = 0;
            }
        }

        public void ShootGear()
        {
            var obj = ObjectPool.Instance.GetPooledObject(this.GearData.PooledObjectType);
            obj.transform.position = this.ShootPos.position;

            if (obj.TryGetComponent(out Projectile projectile))
            {
                projectile.SetUnaffectedLayer(this.gameObject.layer);
            }

            AudioManager.Instance.PlaySFX(this.ShootGearAudio);
            obj.SetActive(true);
            obj.GetComponent<Rigidbody2D>().linearVelocity = this.CurrentDirection * this.GearData.Speed;

            this.BurnAttackFuel(this.Data.GearFuelBurnAmount);
        }

        public void BlastHeatWave()
        {
            Instantiate(this.HeatWave, this.ShootPos.position, Quaternion.identity);
        }

        public void TakeDamage(float damage)
        {
            if (this.isInvulnerable)
            {
                return;
            }

            if (this.CurrentHealth > 0)
            {
                this.CurrentHealth -= damage;
                if (damage > 0)
                {
                    AudioManager.Instance.PlaySFX(this.HurtAudio);
                    this.StartCoroutine(this.SetInvulnerable());
                    this.hitFlash.HitFlashRoutine();
                    this._lastHit = Time.time;
                    Debug.Log("Health: " + this.CurrentHealth);
                }

                this.fsm.SwitchState(
                    this.CurrentHealth <= 0 ? this.states[State.Dead] : this.states[State.Hit]
                );
            }
        }

        private IEnumerator SetInvulnerable()
        {
            yield return null;
            this.isInvulnerable = true;
        }

        public void Heal(float heal)
        {
            AudioManager.Instance.PlaySFX(this.HealAudio);
            this.CurrentHealth = Mathf.Min(this.CurrentHealth + heal, this.Data.Health);
        }

        public void Refuel(float fuel)
        {
            AudioManager.Instance.PlaySFX(this.HealAudio);
            this.CurrentAttackFuelAmount = Mathf.Min(this.CurrentAttackFuelAmount + fuel, this.Data.MaxAttackFuelAmount);
        }

        public void ChangeCurrentAbility(int value)
        {
            this.currentAbilityIndex += value + 4; /// if cA = 0 & value = -1 then cA = 0 - 1 + 4 => cA = 3
            this.currentAbilityIndex %= 4; /// if cA = 3 & value = 1 then cA = (3 + 1)%4 = 0
            this.CurrentAbility = this.abilities[this.currentAbilityIndex];
        }

        private void MyDebug()
        {
        }

        private void OnDrawGizmos()
        {
        }

        #endregion Other Methods

        public void LoadData(GameData data)
        {
            this.transform.position = data.PlayerData.Position;
            this.CurrentHealth = data.PlayerData.Health;
            this.CurrentAttackFuelAmount = data.PlayerData.Fuel;
        }

        public void SaveData(ref GameData data)
        {
            data.PlayerData = new PlayerSaveStruct(this.transform.position, this.CurrentAttackFuelAmount, this.CurrentHealth);
        }
    }
}
