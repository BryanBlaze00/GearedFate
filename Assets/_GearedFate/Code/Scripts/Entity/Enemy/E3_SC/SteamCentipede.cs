//
// Copyright (c) BTG. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

namespace BTG
{
    /// <summary>
    /// GearboundSentinel
    /// </summary>
    public class SteamCentipede : MonoBehaviour, IHealable, IBoss
    {
        public enum CentipedeState
        {
            Idle,
            Chase,
            Circle,
            Charge,
            DeathCircle,
            Whip,
            Knocked,
            Dead
        }

        public float Speed { get; private set; }

        [field: SerializeField] public float RegularSpeed { get; set; }

        [field: SerializeField] public float ChargeSpeed { get; set; }

        [field: SerializeField] public float DeathCircleSpeed { get; set; }

        [field: SerializeField] public float MinimumTimeBeforeCharge { get; set; }

        [field: SerializeField] public float MaximumTimeBeforeCharge { get; set; }

        [field: SerializeField] public float AccelerationWhenLosingBodyPart { get; set; }

        [field: SerializeField] public float CirclingDistance { get; set; }

        [field: SerializeField] public float ChasingDistance { get; set; }

        [field: SerializeField] public float ChargeBoundMultiplier { get; set; }

        [field: SerializeField] public Rigidbody2D RigidBody { get; set; }

        [SerializeField] private float _bodyPartDistance;

        [SerializeField] private float _initialBodyPartsCount;

        [SerializeField] private float _headDistance;

        [SerializeField] private Transform _head;

        [SerializeField] private GameObject _trajectoryPrefab;

        [SerializeField] private GameObject _eggPrefab;

        [SerializeField] private GameObject _bodyPartPrefab;

        [SerializeField] private GameObject _explosion;

        private SplineContainer _trajectory;

        private Transform _target;

        private readonly List<float> _currentPositionsOnSpline = new();

        private readonly FiniteStateMachine<CentipedeState> _finiteStateMachine = new();

        private readonly Dictionary<CentipedeState, CentipedeBaseState> _states = new();

        private readonly List<Transform> _bodyParts = new();

        private float _maxHealth;


        private float _totalSplineLength;

        public float DistanceToTarget => Vector3.Distance(_bodyParts.Last().position, _target.position);

        public Vector2 VectorToTarget => _bodyParts.Last().position - _target.position;

        public Vector2 HeadPosition => _bodyParts.Last().position;

        private float HeadPositionOnSpline => _currentPositionsOnSpline.Last();

        public CentipedeBaseState this[CentipedeState key] => _states[key];

        public Transform this[int key] => _bodyParts[key];

        public Transform Head => _bodyParts.Last();

        public Transform Tail => _bodyParts.First();

        public int BodyPartsCount => _bodyParts.Count;

        public Transform Target => _target;

        public bool IsAttacking { get; set; }

        protected void Start()
        {
            // Create all initial body parts
            for (var i = 0; i < _initialBodyPartsCount; i++)
            {
                var bodypartGo = Instantiate(_bodyPartPrefab, transform, true);

                // Register body death events for each body parts
                bodypartGo.GetComponent<CentipedeBodyPart>().OnBodyPartDeath += HandleBodyPartDeath;
                _bodyParts.Add(bodypartGo.transform);
            }

            _head.GetComponent<CentipedeBodyPart>().OnBodyPartDeath += HandleBodyPartDeath;
            _bodyParts.Add(_head);

            // Initialise Trajectory and position on it.
            var trajectoryGo = Instantiate(_trajectoryPrefab);
            _trajectory = trajectoryGo.GetComponent<SplineContainer>();
            _totalSplineLength = _trajectory.CalculateLength();
            InitializeBodyPosition();

            Speed = RegularSpeed;


            // Add the fsm states of the centipede
            _states.Add(CentipedeState.Chase,
                new CentipedeChaseState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)), this,
                    ChasingDistance));

            _states.Add(CentipedeState.Circle,
                new CentipedeCircleState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)),
                    this,
                    ChasingDistance,
                    CirclingDistance,
                    MinimumTimeBeforeCharge,
                    MaximumTimeBeforeCharge));

            _states.Add(CentipedeState.Charge,
                new CentipedeChargeState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)),
                    this));
            _states.Add(CentipedeState.DeathCircle,
                new CentipedeDeathCircleState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)),
                    this));
            _states.Add(CentipedeState.Whip,
                new CentipedeWhipState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)), this));
            _states.Add(CentipedeState.Knocked,
                new CentipedeKnockedState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Knocked)),
                    this));
            _states.Add(CentipedeState.Dead,
                new CentipedeDeadState(_finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Dead)), this));
            _finiteStateMachine.Initialize(_states[CentipedeState.Chase]);


            // Find it's target
            _target = FindFirstObjectByType<Player>().transform;

            _maxHealth = _bodyParts.Sum(x => x.GetComponent<CentipedeBodyPart>().MaxHealth);
            ;
        }

        private void HandleBodyPartDeath()
        {
            var deadBodyPart = _bodyParts.First();
            UnregisterBodyPart();


            var explosion = Instantiate(_explosion);
            explosion.transform.position = Tail.position;

            if (_bodyParts.Count == 1)
            {
                _finiteStateMachine.SwitchState(_states[CentipedeState.Dead]);

                Elevator.Instance.ActivateElevator(); // Blaze added this line for level transition
            }

            if (_bodyParts.Count > 1) Destroy(deadBodyPart.gameObject);

            RegularSpeed *= AccelerationWhenLosingBodyPart;
            ChargeSpeed *= AccelerationWhenLosingBodyPart;
            DeathCircleSpeed *= AccelerationWhenLosingBodyPart;
            Speed *= AccelerationWhenLosingBodyPart;

            MinimumTimeBeforeCharge *= ChargeBoundMultiplier;
            MaximumTimeBeforeCharge *= ChargeBoundMultiplier;

            ((CentipedeCircleState)_states[CentipedeState.Circle]).SetChargeTimeBounds(MinimumTimeBeforeCharge,
                MaximumTimeBeforeCharge);
        }

        public Vector2 HeadDirection()
        {
            var tangent = _trajectory.EvaluateTangent(_currentPositionsOnSpline.Last());
            var closestCardinal = VectorHelper2D.ClosestCardinal(new Vector2(tangent.x, tangent.y));
            return VectorHelper2D.VectorFromDirection(closestCardinal);
        }

        public void MoveAlongTrajectory()
        {
            var moved = Time.deltaTime * Speed;

            // Move all following transform along the spline
            for (var i = 0; i < _bodyParts.Count; i++)
            {
                _currentPositionsOnSpline[i] += moved / _totalSplineLength;
                _bodyParts[i].position = _trajectory.EvaluatePosition(_currentPositionsOnSpline[i]);
                Vector3 tangent = _trajectory.EvaluateTangent(_currentPositionsOnSpline[i]);
                UpdateAnimationDirectionParameter(i, new Vector2(tangent.x, tangent.y));
            }


            transform.position = HeadPosition;
        }

        public void UpdateAnimationDirectionParameter(int bodyIndex, Vector2 lookingDirection)
        {
            _bodyParts[bodyIndex].GetComponent<CentipedeBodyPart>().SetAnimationDirectionParameter(lookingDirection);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                ShortenTrajectory();
                _finiteStateMachine.SwitchState(_states[CentipedeState.Charge]);
            }

            if (other.TryGetComponent(out HealthScrap healthScrap)) healthScrap.ApplyEffect(this);

            if (other.gameObject.GetComponent<DestructableObject>() && IsAttacking)
            {
                other.gameObject.GetComponent<DestructableObject>().TakeDamage(50000f);
                _finiteStateMachine.SwitchState(_states[CentipedeState.Knocked]);
            }
        }


        public void ShortenTrajectory()
        {
            var lengthBeforeShorteningNode = _totalSplineLength;

            var last = _trajectory.Spline.Knots.Last();
            last.Position = new float3(HeadPosition.x, HeadPosition.y, 0);
            _trajectory.Spline.SetKnot(_trajectory.Spline.Count - 1, last);
            _totalSplineLength = _trajectory.CalculateLength();

            // Gotta recompute the correct spline position for each body parts since the spline length changed.
            for (var i = 0; i < _bodyParts.Count; i++)
                _currentPositionsOnSpline[i] *= lengthBeforeShorteningNode / _totalSplineLength;
        }

        public void SetSpeed(float value)
        {
            Speed = value;
            for (var i = 0; i < _bodyParts.Count; i++)
                _bodyParts[i].GetComponent<CentipedeBodyPart>().SetAnimationSpeed(value / 3);
        }

        public void KnockOutAnimate()
        {
            for (var i = 0; i < _bodyParts.Count - 1; i++)
                _bodyParts[i].GetComponent<CentipedeBodyPart>().SetAnimationSpeed(0);
            _bodyParts.Last().GetComponent<CentipedeBodyPart>().SetAnimationSpeed(1f);
        }

        public void ExpandTrajectory(Vector2 aimingPosition)
        {
            var aimingPosition3D = new Vector3(aimingPosition.x, aimingPosition.y);

            var lengthBeforeAddingNode = _totalSplineLength;

            _trajectory.Spline.Add(aimingPosition3D);

            _totalSplineLength = _trajectory.CalculateLength();

            // Gotta recompute the correct spline position for each body parts since the spline length changed.
            for (var i = 0; i < _bodyParts.Count; i++)
                _currentPositionsOnSpline[i] *= lengthBeforeAddingNode / _totalSplineLength;
        }

        public void SetAnimations(int animId, bool isWalking)
        {
            for (var i = 0; i < _bodyParts.Count; i++)
            {
                var offset = isWalking ? i % 2 / 2f : 0f;
                _bodyParts[i].GetComponent<CentipedeBodyPart>().PlayAnimation(animId, offset);
            }
        }

        // Check if a given normalized spline position is after the spline end.
        public bool IsReachingTrajectoryEndNextStep()
        {
            var moved = Time.deltaTime * Speed;
            return HeadPositionOnSpline + moved / _totalSplineLength >= 1;
        }

        protected void Update()
        {
            _finiteStateMachine.CurrentState.OnFrameUpdate();
        }

        protected void FixedUpdate()
        {
            _finiteStateMachine.CurrentState.OnPhysicsUpdate();
        }

        private void InitializeBodyPosition()
        {
            var normalizedDistance = _bodyPartDistance / _totalSplineLength;
            for (var i = 0; i < _bodyParts.Count - 1; i++)
            {
                _currentPositionsOnSpline.Add(normalizedDistance * i);
                _bodyParts[i].position = _trajectory.EvaluatePosition(normalizedDistance * i);
            }

            // position head
            var headNormalizedDistance = _headDistance / _totalSplineLength;
            _bodyParts.Last().position =
                _trajectory.EvaluatePosition(normalizedDistance * (_bodyParts.Count() - 2) + headNormalizedDistance);
            _currentPositionsOnSpline.Add(normalizedDistance * (_bodyParts.Count() - 2) + headNormalizedDistance);
        }

        private void UnregisterBodyPart()
        {
            _bodyParts.Remove(_bodyParts.First());
            _currentPositionsOnSpline.Remove(_currentPositionsOnSpline.First());
        }


        public void Heal(float healthAdded)
        {
            /*CentipedeBodyPart tail = Tail.GetComponent<CentipedeBodyPart>();

            // Don't create more body parts than its initial amount
            if (_bodyParts.Count <= _initialBodyPartsCount)
            {
                // create new body part and set it up
                GameObject newBodyPart = Instantiate(_bodyPartPrefab, transform, true);
                _bodyParts.Insert(0, newBodyPart.transform);

                float normalizedDistance = _bodyPartDistance / _totalSplineLength;
                _currentPositionsOnSpline.Add(_currentPositionsOnSpline.Last() + normalizedDistance);
                _bodyParts.First().position = _trajectory.EvaluatePosition(_currentPositionsOnSpline.Last());


                RegularSpeed /= AccelerationWhenLosingBodyPart;
                ChargeSpeed /= AccelerationWhenLosingBodyPart;
                DeathCircleSpeed /= AccelerationWhenLosingBodyPart;
                Speed /= AccelerationWhenLosingBodyPart;

                MinimumTimeBeforeCharge /= ChargeBoundMultiplier;
                MaximumTimeBeforeCharge /= ChargeBoundMultiplier;

                ((CentipedeCircleState)_states[CentipedeState.Circle]).SetChargeTimeBounds(MinimumTimeBeforeCharge, MaximumTimeBeforeCharge);
            }*/
        }

        public void LayEgg()
        {
            var egg = Instantiate(_eggPrefab);
            egg.transform.position = Tail.position;
            egg.GetComponent<Explosive>().Explode(2f);
        }

        public float MaxHealth => _maxHealth;

        public float CurrentHealth => _bodyParts.Sum(x => x.GetComponent<CentipedeBodyPart>().CurrentHealth);
    }
}
