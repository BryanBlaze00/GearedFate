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

        public float DistanceToTarget => Vector3.Distance(this._bodyParts.Last().position, this._target.position);

        public Vector2 VectorToTarget => this._bodyParts.Last().position - this._target.position;

        public Vector2 HeadPosition => this._bodyParts.Last().position;

        private float HeadPositionOnSpline => this._currentPositionsOnSpline.Last();

        public CentipedeBaseState this[CentipedeState key] => this._states[key];

        public Transform this[int key] => this._bodyParts[key];

        public Transform Head => this._bodyParts.Last();

        public Transform Tail => this._bodyParts.First();

        public int BodyPartsCount => this._bodyParts.Count;

        public Transform Target => this._target;

        public bool IsAttacking { get; set; }

        protected void Start()
        {
            // Create all initial body parts
            for (var i = 0; i < this._initialBodyPartsCount; i++)
            {
                var bodypartGo = Instantiate(this._bodyPartPrefab, this.transform, true);

                // Register body death events for each body parts
                bodypartGo.GetComponent<CentipedeBodyPart>().OnBodyPartDeath += this.HandleBodyPartDeath;
                this._bodyParts.Add(bodypartGo.transform);
            }

            this._head.GetComponent<CentipedeBodyPart>().OnBodyPartDeath += this.HandleBodyPartDeath;
            this._bodyParts.Add(this._head);

            // Initialise Trajectory and position on it.
            var trajectoryGo = Instantiate(this._trajectoryPrefab);
            this._trajectory = trajectoryGo.GetComponent<SplineContainer>();
            this._totalSplineLength = this._trajectory.CalculateLength();
            this.InitializeBodyPosition();

            this.Speed = this.RegularSpeed;


            // Add the fsm states of the centipede
            this._states.Add(CentipedeState.Chase,
                new CentipedeChaseState(
                    this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)), this,
                    this.ChasingDistance));

            this._states.Add(CentipedeState.Circle,
                new CentipedeCircleState(
                    this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)),
                    this,
                    this.ChasingDistance,
                    this.CirclingDistance,
                    this.MinimumTimeBeforeCharge,
                    this.MaximumTimeBeforeCharge));

            this._states.Add(CentipedeState.Charge,
                new CentipedeChargeState(
                    this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)),
                    this));
            this._states.Add(CentipedeState.DeathCircle,
                new CentipedeDeathCircleState(
                    this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)),
                    this));
            this._states.Add(CentipedeState.Whip,
                new CentipedeWhipState(this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Chase)), this));
            this._states.Add(CentipedeState.Knocked,
                new CentipedeKnockedState(
                    this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Knocked)),
                    this));
            this._states.Add(CentipedeState.Dead,
                new CentipedeDeadState(this._finiteStateMachine, Animator.StringToHash(nameof(CentipedeState.Dead)), this));
            this._finiteStateMachine.Initialize(this._states[CentipedeState.Chase]);


            // Find it's target
            this._target = FindFirstObjectByType<Player>().transform;

            this._maxHealth = this._bodyParts.Sum(x => x.GetComponent<CentipedeBodyPart>().MaxHealth);
            ;
        }

        private void HandleBodyPartDeath()
        {
            var deadBodyPart = this._bodyParts.First();
            this.UnregisterBodyPart();


            var explosion = Instantiate(this._explosion);
            explosion.transform.position = this.Tail.position;

            if (this._bodyParts.Count == 1)
            {
                this._finiteStateMachine.SwitchState(this._states[CentipedeState.Dead]);

                Elevator.Instance.ActivateElevator(); // Blaze added this line for level transition
            }

            if (this._bodyParts.Count > 1) Destroy(deadBodyPart.gameObject);

            this.RegularSpeed *= this.AccelerationWhenLosingBodyPart;
            this.ChargeSpeed *= this.AccelerationWhenLosingBodyPart;
            this.DeathCircleSpeed *= this.AccelerationWhenLosingBodyPart;
            this.Speed *= this.AccelerationWhenLosingBodyPart;

            this.MinimumTimeBeforeCharge *= this.ChargeBoundMultiplier;
            this.MaximumTimeBeforeCharge *= this.ChargeBoundMultiplier;

            ((CentipedeCircleState)this._states[CentipedeState.Circle]).SetChargeTimeBounds(
                this.MinimumTimeBeforeCharge,
                this.MaximumTimeBeforeCharge);
        }

        public Vector2 HeadDirection()
        {
            var tangent = this._trajectory.EvaluateTangent(this._currentPositionsOnSpline.Last());
            var closestCardinal = VectorHelper2D.ClosestCardinal(new Vector2(tangent.x, tangent.y));
            return VectorHelper2D.VectorFromDirection(closestCardinal);
        }

        public void MoveAlongTrajectory()
        {
            var moved = Time.deltaTime * this.Speed;

            // Move all following transform along the spline
            for (var i = 0; i < this._bodyParts.Count; i++)
            {
                this._currentPositionsOnSpline[i] += moved / this._totalSplineLength;
                this._bodyParts[i].position = this._trajectory.EvaluatePosition(this._currentPositionsOnSpline[i]);
                Vector3 tangent = this._trajectory.EvaluateTangent(this._currentPositionsOnSpline[i]);
                this.UpdateAnimationDirectionParameter(i, new Vector2(tangent.x, tangent.y));
            }

            this.transform.position = this.HeadPosition;
        }

        public void UpdateAnimationDirectionParameter(int bodyIndex, Vector2 lookingDirection)
        {
            this._bodyParts[bodyIndex].GetComponent<CentipedeBodyPart>().SetAnimationDirectionParameter(lookingDirection);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                this.ShortenTrajectory();
                this._finiteStateMachine.SwitchState(this._states[CentipedeState.Charge]);
            }

            if (other.TryGetComponent(out HealthScrap healthScrap)) healthScrap.ApplyEffect(this);

            if (other.gameObject.GetComponent<DestructableObject>() && this.IsAttacking)
            {
                other.gameObject.GetComponent<DestructableObject>().TakeDamage(50000f);
                this._finiteStateMachine.SwitchState(this._states[CentipedeState.Knocked]);
            }
        }


        public void ShortenTrajectory()
        {
            var lengthBeforeShorteningNode = this._totalSplineLength;

            var last = this._trajectory.Spline.Knots.Last();
            last.Position = new float3(this.HeadPosition.x, this.HeadPosition.y, 0);
            this._trajectory.Spline.SetKnot(this._trajectory.Spline.Count - 1, last);
            this._totalSplineLength = this._trajectory.CalculateLength();

            // Gotta recompute the correct spline position for each body parts since the spline length changed.
            for (var i = 0; i < this._bodyParts.Count; i++) this._currentPositionsOnSpline[i] *= lengthBeforeShorteningNode / this._totalSplineLength;
        }

        public void SetSpeed(float value)
        {
            this.Speed = value;
            for (var i = 0; i < this._bodyParts.Count; i++) this._bodyParts[i].GetComponent<CentipedeBodyPart>().SetAnimationSpeed(value / 3);
        }

        public void KnockOutAnimate()
        {
            for (var i = 0; i < this._bodyParts.Count - 1; i++) this._bodyParts[i].GetComponent<CentipedeBodyPart>().SetAnimationSpeed(0);
            this._bodyParts.Last().GetComponent<CentipedeBodyPart>().SetAnimationSpeed(1f);
        }

        public void ExpandTrajectory(Vector2 aimingPosition)
        {
            var aimingPosition3D = new Vector3(aimingPosition.x, aimingPosition.y);

            var lengthBeforeAddingNode = this._totalSplineLength;

            this._trajectory.Spline.Add(aimingPosition3D);

            this._totalSplineLength = this._trajectory.CalculateLength();

            // Gotta recompute the correct spline position for each body parts since the spline length changed.
            for (var i = 0; i < this._bodyParts.Count; i++) this._currentPositionsOnSpline[i] *= lengthBeforeAddingNode / this._totalSplineLength;
        }

        public void SetAnimations(int animId, bool isWalking)
        {
            for (var i = 0; i < this._bodyParts.Count; i++)
            {
                var offset = isWalking ? i % 2 / 2f : 0f;
                this._bodyParts[i].GetComponent<CentipedeBodyPart>().PlayAnimation(animId, offset);
            }
        }

        // Check if a given normalized spline position is after the spline end.
        public bool IsReachingTrajectoryEndNextStep()
        {
            var moved = Time.deltaTime * this.Speed;
            return this.HeadPositionOnSpline + moved / this._totalSplineLength >= 1;
        }

        protected void Update()
        {
            this._finiteStateMachine.CurrentState.OnFrameUpdate();
        }

        protected void FixedUpdate()
        {
            this._finiteStateMachine.CurrentState.OnPhysicsUpdate();
        }

        private void InitializeBodyPosition()
        {
            var normalizedDistance = this._bodyPartDistance / this._totalSplineLength;
            for (var i = 0; i < this._bodyParts.Count - 1; i++)
            {
                this._currentPositionsOnSpline.Add(normalizedDistance * i);
                this._bodyParts[i].position = this._trajectory.EvaluatePosition(normalizedDistance * i);
            }

            // position head
            var headNormalizedDistance = this._headDistance / this._totalSplineLength;
            this._bodyParts.Last().position = this._trajectory.EvaluatePosition(normalizedDistance * (this._bodyParts.Count() - 2) + headNormalizedDistance);
            this._currentPositionsOnSpline.Add(normalizedDistance * (this._bodyParts.Count() - 2) + headNormalizedDistance);
        }

        private void UnregisterBodyPart()
        {
            this._bodyParts.Remove(this._bodyParts.First());
            this._currentPositionsOnSpline.Remove(this._currentPositionsOnSpline.First());
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
            var egg = Instantiate(this._eggPrefab);
            egg.transform.position = this.Tail.position;
            egg.GetComponent<Explosive>().Explode(2f);
        }

        public float MaxHealth => this._maxHealth;

        public float CurrentHealth => this._bodyParts.Sum(x => x.GetComponent<CentipedeBodyPart>().CurrentHealth);
    }
}
