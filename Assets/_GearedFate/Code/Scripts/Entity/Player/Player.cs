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
            RB = GetComponent<Rigidbody2D>();
            Input = GetComponent<PlayerInputHandler>();
            var body = transform.Find("Graphics").Find("Body");
            Anim = body.GetComponent<Animator>();
            AnimEvent = body.GetComponent<PlayerAnimationEventHandler>();
            hitFlash = GetComponentInChildren<HitFlash>();
            Knockback = GetComponentInChildren<Knockback>();

            states.Add(State.Idle, new PlayerIdleState(fsm, this, Data, Animator.StringToHash(nameof(State.Idle))));
            states.Add(State.Move, new PlayerMoveState(fsm, this, Data, Animator.StringToHash(nameof(State.Move))));
            states.Add(State.Dash, new PlayerDashState(fsm, this, Data, Animator.StringToHash(nameof(State.Dash))));
            states.Add(State.Slash, new PlayerSlashState(fsm, this, Data, Animator.StringToHash(nameof(State.Slash))));
            states.Add(State.HeatWave,
                new PlayerHeatWave(fsm, this, Data, Animator.StringToHash(nameof(State.HeatWave))));
            states.Add(State.GearToss,
                new PlayerGearTossState(fsm, this, Data, Animator.StringToHash(nameof(State.GearToss))));
            states.Add(State.FireBlaze,
                new PlayerFireBlazeState(fsm, this, Data,
                    Animator.StringToHash("ChargeUp"))); ///Charges up before plays fireblaze Animation
            states.Add(State.Hit,
                new PlayerHitState(fsm, this, Data,
                    Animator.StringToHash(nameof(State.Idle)))); //TODO: change to hit (No animation yet)
            states.Add(State.Dead, new PlayerDeadState(fsm, this, Data, Animator.StringToHash(nameof(State.Dead))));

            abilities.Add(State.Slash);
            abilities.Add(State.FireBlaze);
            abilities.Add(State.HeatWave);
            abilities.Add(State.GearToss);
        }

        private void Start()
        {
            CurrentHealth = Data.Health;
            CurrentAttackFuelAmount = Data.MaxAttackFuelAmount;
            CurrentDirection = Vector2.down;
            currentAbilityIndex = 0;
            CurrentAbility = abilities[currentAbilityIndex];
            Blaze.SetActive(false);
            AnimMoveX = Animator.StringToHash("MoveX");
            AnimMoveY = Animator.StringToHash("MoveY");
            fsm.Initialize(states[State.Idle]);
        }

        private void Update()
        {
            fsm.CurrentState.OnFrameUpdate();
            if (_lastHit + 1f < Time.time) isInvulnerable = false;
        }

        private void FixedUpdate()
        {
            fsm.CurrentState.OnPhysicsUpdate();
            // make sure the player isn't stuck out of bounds
            var walkMask = 1 << NavMesh.GetAreaFromName("Walkable");

            var inBounds = NavMesh.SamplePosition(transform.position, out var hit, 0.1f, walkMask);
            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName.Contains("Boss") || sceneName.Contains("boss")) // it's getting hackier and hackier
            {
                if (inBounds)
                {
                    OutOfBoundsTime--;
                    if (OutOfBoundsTime < 0f)
                        OutOfBoundsTime = 0f;
                }
                else
                {
                    OutOfBoundsTime += Time.fixedDeltaTime;
                    if (OutOfBoundsTime >= 6f)
                    {
                        //Debug.LogError("Player out of bounds!");
                        var found = false;
                        var searchRadius = 0.5f;
                        while (!found && searchRadius < 10f)
                        {
                            found = NavMesh.SamplePosition(transform.position, out hit, searchRadius, walkMask);
                            searchRadius += 0.5f;
                        }

                        if (found) transform.position = hit.position;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            MyDebug();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isInvulnerable) return;
        }

        #endregion Unity CallBacks

        #region Other Methods

        public void SetLookDir()
        {
            if (Input.MoveInput == Vector2.zero) return;
            CurrentDirection = Input.MoveInput;
            Anim.SetFloat(AnimMoveX, Input.MoveInput.x);
            Anim.SetFloat(AnimMoveY, Input.MoveInput.y);
            CheckIfShouldFlip();
        }

        public void CheckIfShouldFlip()
        {
            renderer.flipX = Input.MoveInput.x == -1;
        }

        public void BurnAttackFuel(float fuel)
        {
            CurrentAttackFuelAmount -= fuel;
            if (CurrentAttackFuelAmount < 0)
                CurrentAttackFuelAmount = 0;
        }

        public void ShootGear()
        {
            var obj = ObjectPool.Instance.GetPooledObject(GearData.PooledObjectType);
            obj.transform.position = ShootPos.position;

            if (obj.TryGetComponent(out Projectile projectile)) projectile.SetUnaffectedLayer(gameObject.layer);

            AudioManager.Instance.PlaySFX(ShootGearAudio);
            obj.SetActive(true);
            obj.GetComponent<Rigidbody2D>().linearVelocity = CurrentDirection * GearData.Speed;

            BurnAttackFuel(Data.GearFuelBurnAmount);
        }

        public void BlastHeatWave()
        {
            Instantiate(HeatWave, ShootPos.position, Quaternion.identity);
        }

        public void TakeDamage(float damage)
        {
            if (isInvulnerable) return;

            if (CurrentHealth > 0)
            {
                CurrentHealth -= damage;
                if (damage > 0)
                {
                    AudioManager.Instance.PlaySFX(HurtAudio);
                    StartCoroutine(SetInvulnerable());
                    hitFlash.HitFlashRoutine();
                    _lastHit = Time.time;
                    Debug.Log("Health: " + CurrentHealth);
                }


                fsm.SwitchState(
                    CurrentHealth <= 0 ? states[State.Dead] : states[State.Hit]
                );
            }
        }

        private IEnumerator SetInvulnerable()
        {
            yield return null;
            isInvulnerable = true;
        }

        public void Heal(float heal)
        {
            AudioManager.Instance.PlaySFX(HealAudio);
            CurrentHealth = Mathf.Min(CurrentHealth + heal, Data.Health);
        }

        public void Refuel(float fuel)
        {
            AudioManager.Instance.PlaySFX(HealAudio);
            CurrentAttackFuelAmount = Mathf.Min(CurrentAttackFuelAmount + fuel, Data.MaxAttackFuelAmount);
        }

        public void ChangeCurrentAbility(int value)
        {
            currentAbilityIndex += value + 4; /// if cA = 0 & value = -1 then cA = 0 - 1 + 4 => cA = 3
            currentAbilityIndex %= 4; /// if cA = 3 & value = 1 then cA = (3 + 1)%4 = 0
            CurrentAbility = abilities[currentAbilityIndex];
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
            transform.position = data.PlayerData.Position;
            CurrentHealth = data.PlayerData.Health;
            CurrentAttackFuelAmount = data.PlayerData.Fuel;
        }

        public void SaveData(ref GameData data)
        {
            data.PlayerData = new PlayerSaveStruct(transform.position, CurrentAttackFuelAmount, CurrentHealth);
        }
    }
}
