using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    public class CentipedeWhipState : CentipedeBaseState
    {
        private readonly int _animId;

        // How much it rotates per second
        private float _range = 120;

        // How much it adds to range for each body part piece back
        private float _rangeDelay = 10;

        // how long it takes to whip from left to right
        private float _frequency = 0.5f;

        private readonly List<float> _angles = new();

        private readonly List<Vector2> _initialPositions = new();

        private Vector2 _angleVector;

        private float _timer = 0;

        private bool _whipFinished;

        public CentipedeWhipState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId,
            SteamCentipede steamCentipede) : base(fsm, steamCentipede)
        {
            this._animId = animationId;
        }

        public override void OnEnter()
        {
            this._whipFinished = false;
            this.Centipede.SetSpeed(this.Centipede.RegularSpeed);
            this.Centipede.SetAnimations(this._animId, true);
            this.RegisterInitialPosition();
            this._angleVector = new Vector2(-this.Centipede.HeadDirection().normalized.y, this.Centipede.HeadDirection().normalized.x);
            this._angleVector = Vector2.up;

            this.ComputeAngles();
            this.Centipede.StartCoroutine(this.Whip());
        }

        public override void OnExit()
        {
            this.ResetPosition();
        }

        public override void OnFrameUpdate()
        {
            if (this._whipFinished) this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Chase]);
        }

        public override void OnPhysicsUpdate()
        {
        }

        private IEnumerator Whip()
        {
            while (this._timer < this._frequency)
            {
                this._timer += Time.deltaTime;

                for (var i = 0; i < this.Centipede.BodyPartsCount; i++)
                {
                    this.Centipede[i].RotateAround(
                        this.Centipede.HeadPosition, Vector3.forward,
                        -(this._angles[i] / this._frequency) * Time.deltaTime);
                    this.Centipede[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            this._timer = 0;
            while (this._timer < this._frequency)
            {
                this._timer += Time.deltaTime;
                for (var i = 0; i < this.Centipede.BodyPartsCount; i++)
                {
                    this.Centipede[i].RotateAround(
                        this.Centipede.HeadPosition, Vector3.forward,
                        (this._range + i * this._rangeDelay) / this._frequency * Time.deltaTime);
                    this.Centipede[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            this._timer = 0;
            while (this._timer < this._frequency)
            {
                this._timer += Time.deltaTime;
                for (var i = 0; i < this.Centipede.BodyPartsCount; i++)
                {
                    this.Centipede[i].RotateAround(
                        this.Centipede.HeadPosition, Vector3.forward,
                        -((this._range + i * this._rangeDelay - this._angles[i]) / this._frequency) * Time.deltaTime);
                    this.Centipede[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            this._whipFinished = true;
        }


        private void RegisterInitialPosition()
        {
            this._initialPositions.Clear();
            for (var i = 0; i < this.Centipede.BodyPartsCount; i++) this._initialPositions.Add(this.Centipede[i].position);
        }

        private void ComputeAngles()
        {
            this._angles.Clear();
            for (var i = 0; i < this.Centipede.BodyPartsCount; i++)
                this._angles.Add((this._range + i * this._rangeDelay) / 2 - Vector2.SignedAngle(
                    this._angleVector,
                    this.Centipede.HeadPosition - (Vector2)this.Centipede[i].position));
        }

        private void ResetPosition()
        {
            for (var i = 0; i < this.Centipede.BodyPartsCount; i++) this.Centipede[i].position = this._initialPositions[i];
        }
    }
}
