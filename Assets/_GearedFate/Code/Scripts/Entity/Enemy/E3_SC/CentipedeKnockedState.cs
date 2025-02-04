using UnityEngine;

namespace BTG
{
    public class CentipedeKnockedState : CentipedeBaseState
    {
        private readonly int _animId;

        private float _knockedTime;

        private float _timeToGetBack = 3f;

        public CentipedeKnockedState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId, SteamCentipede steamCentipede)
            : base(fsm, steamCentipede)
        {
            _animId = animationId;
        }

        public override void OnEnter()
        {
            Debug.Log("Knocked Down !");
            _knockedTime = Time.time;
            Centipede.SetAnimations(_animId, false);
            Centipede.KnockOutAnimate();
            Centipede.IsAttacking = false;
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > _knockedTime + _timeToGetBack)
            {
                fsm.SwitchState(Centipede[SteamCentipede.CentipedeState.Chase]);
            }
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
