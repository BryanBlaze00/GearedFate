using UnityEngine;

namespace BTG
{
    public class CentipedeKnockedState : CentipedeBaseState
    {
        private readonly int _animId;

        private float _knockedTime;

        private float _timeToGetBack = 3f;

        public CentipedeKnockedState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId,
            SteamCentipede steamCentipede)
            : base(fsm, steamCentipede)
        {
            this._animId = animationId;
        }

        public override void OnEnter()
        {
            Debug.Log("Knocked Down !");
            this._knockedTime = Time.time;
            this.Centipede.SetAnimations(this._animId, false);
            this.Centipede.KnockOutAnimate();
            this.Centipede.IsAttacking = false;
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > this._knockedTime + this._timeToGetBack) this.fsm.SwitchState(this.Centipede[SteamCentipede.CentipedeState.Chase]);
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
