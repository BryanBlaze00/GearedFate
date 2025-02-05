// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.SceneManagement;
    using UnityEngine.Serialization;

    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, IDamagable, IDataPersistence, IHealable, IRefuelable
    {
        private readonly Dictionary<State, PlayerBaseState> _states = new ();

        private readonly List<State> _abilities = new ();

        private readonly FiniteStateMachine<State> _fsm = new ();

        private float _outOfBoundsTime = 0f;

        private int _currentAbilityIndex;

        private HitFlash _hitFlash;

        private float _lastHit;

        [FormerlySerializedAs("renderer")]
        [SerializeField]
        private SpriteRenderer _renderer;

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
            Dead,
        }

        public bool IsInvulnerable { get; set; }

        [field: SerializeField]
        public PlayerData Data { get; private set; }

        public PlayerInputHandler Input { get; private set; }

        public float CurrentHealth { get; private set; }

        public float CurrentAttackFuelAmount { get; private set; }

        public Vector2 CurrentDirection { get; private set; }

        public State CurrentAbility { get; private set; }

        [field: SerializeField]
        public PlayerInfoSO PlayerInfo { get; private set; }

        [field: SerializeField]
        public ProjectileData GearData { get; private set; }

        [field: SerializeField]
        public GameObject HeatWave { get; private set; }

        [field: SerializeField]
        public GameObject Blaze { get; private set; }

        [field: SerializeField]
        public Transform ShootPos { get; private set; }

        [field: SerializeField]
        public Transform SlashPos { get; private set; }

        public PlayerAnimationEventHandler AnimEvent { get; private set; }

        public Knockback Knockback { get; private set; }

        public Rigidbody2D RB { get; private set; }

        public Animator Anim { get; private set; }

        public int AnimMoveX { get; private set; }

        public int AnimMoveY { get; private set; }

        [field:SerializeField]
        public AudioClip HurtAudio { get; private set; }

        [field:SerializeField]
        public AudioClip ShootGearAudio { get; private set; }

        [field:SerializeField]
        public AudioClip HealAudio { get; private set; }

        [field:SerializeField]
        public AudioClip DashAudio { get; private set; }

        [field:SerializeField]
        public AudioClip SlashAudio { get; private set; }

        [field:SerializeField]
        public AudioClip DeathAudio { get; private set; }

        [field:SerializeField]
        public AudioClip FlameBeam { get; private set; }

        public PlayerBaseState this[State key] => _states[key];

        public void SetLookDir()
        {
            if (Input.MoveInput == Vector2.zero)
            {
                return;
            }

            CurrentDirection = Input.MoveInput;
            Anim.SetFloat(AnimMoveX, Input.MoveInput.x);
            Anim.SetFloat(AnimMoveY, Input.MoveInput.y);
            CheckIfShouldFlip();
        }

        public void CheckIfShouldFlip()
        {
            _renderer.flipX = Input.MoveInput.x == -1;
        }

        public void BurnAttackFuel(float fuel)
        {
            CurrentAttackFuelAmount -= fuel;
            if (CurrentAttackFuelAmount < 0)
            {
                CurrentAttackFuelAmount = 0;
            }
        }

        public void ShootGear()
        {
            var obj = ObjectPool.Instance.GetPooledObject(GearData.PooledObjectType);
            obj.transform.position = ShootPos.position;

            if (obj.TryGetComponent(out Projectile projectile))
            {
                projectile.SetUnaffectedLayer(gameObject.layer);
            }

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
            if (IsInvulnerable)
            {
                return;
            }

            if (CurrentHealth > 0)
            {
                CurrentHealth -= damage;
                if (damage > 0)
                {
                    AudioManager.Instance.PlaySFX(HurtAudio);
                    StartCoroutine(SetInvulnerable());
                    _hitFlash.HitFlashRoutine();
                    _lastHit = Time.time;
                    Debug.Log("Health: " + CurrentHealth);
                }

                _fsm.SwitchState(
                    CurrentHealth <= 0 ? _states[State.Dead] : _states[State.Hit]);
            }
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
            _currentAbilityIndex += value + 4; /// if cA = 0 & value = -1 then cA = 0 - 1 + 4 => cA = 3
            _currentAbilityIndex %= 4; /// if cA = 3 & value = 1 then cA = (3 + 1)%4 = 0
            CurrentAbility = _abilities[_currentAbilityIndex];
        }

        public void LoadData(GameData data)
        {
            transform.position = data.PlayerData.Position;
            CurrentHealth = data.PlayerData.Health;
            CurrentAttackFuelAmount = data.PlayerData.Fuel;
        }

        public void SaveData(ref GameData data)
        {
            data.SavePlayerData(new PlayerSaveStruct(transform.position, CurrentAttackFuelAmount, CurrentHealth));
        }

        private void LateUpdate()
        {
            MyDebug();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsInvulnerable)
            {
                return;
            }
        }

        private IEnumerator SetInvulnerable()
        {
            yield return null;
            IsInvulnerable = true;
        }

        private void MyDebug()
        {
        }

        private void OnDrawGizmos()
        {
        }

        private void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
            Input = GetComponent<PlayerInputHandler>();
            var body = transform.Find("Graphics").Find("Body");
            Anim = body.GetComponent<Animator>();
            AnimEvent = body.GetComponent<PlayerAnimationEventHandler>();
            _hitFlash = GetComponentInChildren<HitFlash>();
            Knockback = GetComponentInChildren<Knockback>();

            _states.Add(State.Idle, new PlayerIdleState(_fsm, this, Data, Animator.StringToHash(nameof(State.Idle))));
            _states.Add(State.Move, new PlayerMoveState(_fsm, this, Data, Animator.StringToHash(nameof(State.Move))));
            _states.Add(State.Dash, new PlayerDashState(_fsm, this, Data, Animator.StringToHash(nameof(State.Dash))));
            _states.Add(State.Slash, new PlayerSlashState(_fsm, this, Data, Animator.StringToHash(nameof(State.Slash))));
            _states.Add(
                State.HeatWave,
                new PlayerHeatWave(_fsm, this, Data, Animator.StringToHash(nameof(State.HeatWave))));

            _states.Add(
                State.GearToss,
                new PlayerGearTossState(_fsm, this, Data, Animator.StringToHash(nameof(State.GearToss))));

            _states.Add(
                State.FireBlaze,
                new PlayerFireBlazeState(
                    _fsm,
                    this,
                    Data,
                    Animator.StringToHash("ChargeUp"))); ///Charges up before plays fireblaze Animation

            _states.Add(
                State.Hit,
                new PlayerHitState(
                    _fsm,
                    this,
                    Data,
                    Animator.StringToHash(nameof(State.Idle)))); // TODO: change to hit (No animation yet)

            _states.Add(State.Dead, new PlayerDeadState(_fsm, this, Data, Animator.StringToHash(nameof(State.Dead))));

            _abilities.Add(State.Slash);
            _abilities.Add(State.FireBlaze);
            _abilities.Add(State.HeatWave);
            _abilities.Add(State.GearToss);
        }

        private void Start()
        {
            CurrentHealth = Data.Health;
            CurrentAttackFuelAmount = Data.MaxAttackFuelAmount;
            CurrentDirection = Vector2.down;
            _currentAbilityIndex = 0;
            CurrentAbility = _abilities[_currentAbilityIndex];
            Blaze.SetActive(false);
            AnimMoveX = Animator.StringToHash("MoveX");
            AnimMoveY = Animator.StringToHash("MoveY");
            _fsm.Initialize(_states[State.Idle]);
        }

        private void Update()
        {
            _fsm.CurrentState.OnFrameUpdate();
            if (_lastHit + 1f < Time.time)
            {
                IsInvulnerable = false;
            }
        }

        private void FixedUpdate()
        {
            _fsm.CurrentState.OnPhysicsUpdate();

            // make sure the player isn't stuck out of bounds
            var walkMask = 1 << NavMesh.GetAreaFromName("Walkable");

            var inBounds = NavMesh.SamplePosition(transform.position, out var hit, 0.1f, walkMask);
            var sceneName = SceneManager.GetActiveScene().name;

            // it's getting hackier and hackier
            if (sceneName.Contains("Boss") || sceneName.Contains("boss"))
            {
                if (inBounds)
                {
                    _outOfBoundsTime--;
                    if (_outOfBoundsTime < 0f)
                    {
                        _outOfBoundsTime = 0f;
                    }
                }
                else
                {
                    _outOfBoundsTime += Time.fixedDeltaTime;
                    if (_outOfBoundsTime >= 6f)
                    {
                        // Debug.LogError("Player out of bounds!");
                        var found = false;
                        var searchRadius = 0.5f;
                        while (!found && searchRadius < 10f)
                        {
                            found = NavMesh.SamplePosition(transform.position, out hit, searchRadius, walkMask);
                            searchRadius += 0.5f;
                        }

                        if (found)
                        {
                            transform.position = hit.position;
                        }
                    }
                }
            }
        }
    }
}
