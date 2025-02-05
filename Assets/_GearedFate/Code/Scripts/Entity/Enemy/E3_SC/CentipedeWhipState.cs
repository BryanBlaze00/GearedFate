namespace BTG
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CentipedeWhipState : CentipedeBaseState
    {
        private readonly int _animId;

        // How much it rotates per second
        private float _range = 120;

        // How much it adds to range for each body part piece back
        private float _rangeDelay = 10;

        // how long it takes to whip from left to right
        private float _frequency = 0.5f;

        private readonly List<float> _angles = new ();

        private readonly List<Vector2> _initialPositions = new ();

        private Vector2 _angleVector;

        private float _timer = 0;

        private bool _whipFinished;

        public CentipedeWhipState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId, SteamCentipede steamCentipede)
            : base(fsm, steamCentipede)
        {
            _animId = animationId;
        }

        public override void OnEnter()
        {
            _whipFinished = false;
            Centipede.SetSpeed(Centipede.RegularSpeed);
            Centipede.SetAnimations(_animId, true);
            RegisterInitialPosition();
            _angleVector = new Vector2(-Centipede.HeadDirection().normalized.y, Centipede.HeadDirection().normalized.x);
            _angleVector = Vector2.up;

            ComputeAngles();
            Centipede.StartCoroutine(Whip());
        }

        public override void OnExit()
        {
            ResetPosition();
        }

        public override void OnFrameUpdate()
        {
            if (_whipFinished)
            {
                Fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Chase]);
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        private IEnumerator Whip()
        {
            while (_timer < _frequency)
            {
                _timer += Time.deltaTime;

                for (var i = 0; i < Centipede.BodyPartsCount; i++)
                {
                    Centipede[i].RotateAround(
                        Centipede.HeadPosition,
                        Vector3.forward,
                        -(_angles[i] / _frequency) * Time.deltaTime);
                    Centipede[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            _timer = 0;
            while (_timer < _frequency)
            {
                _timer += Time.deltaTime;
                for (var i = 0; i < Centipede.BodyPartsCount; i++)
                {
                    Centipede[i].RotateAround(
                        Centipede.HeadPosition,
                        Vector3.forward,
                        (_range + (i * _rangeDelay)) / _frequency * Time.deltaTime);

                    Centipede[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            _timer = 0;
            while (_timer < _frequency)
            {
                _timer += Time.deltaTime;
                for (var i = 0; i < Centipede.BodyPartsCount; i++)
                {
                    Centipede[i].RotateAround(
                        Centipede.HeadPosition,
                        Vector3.forward,
                        -((_range + (i * _rangeDelay) - _angles[i]) / _frequency) * Time.deltaTime);

                    Centipede[i].rotation = Quaternion.identity;
                }

                yield return null;
            }

            _whipFinished = true;
        }

        private void RegisterInitialPosition()
        {
            _initialPositions.Clear();
            for (var i = 0; i < Centipede.BodyPartsCount; i++)
            {
                _initialPositions.Add(Centipede[i].position);
            }
        }

        private void ComputeAngles()
        {
            _angles.Clear();
            for (var i = 0; i < Centipede.BodyPartsCount; i++)
            {
                _angles.Add(((_range + (i * _rangeDelay)) / 2) - Vector2.SignedAngle(
                    _angleVector,
                    Centipede.HeadPosition - (Vector2)Centipede[i].position));
            }
        }

        private void ResetPosition()
        {
            for (var i = 0; i < Centipede.BodyPartsCount; i++)
            {
                Centipede[i].position = _initialPositions[i];
            }
        }
    }
}
