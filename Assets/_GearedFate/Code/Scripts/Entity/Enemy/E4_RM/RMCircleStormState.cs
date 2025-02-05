namespace BTG
{
    using UnityEngine;

    public class RMCircleStormState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 10f;

        private bool _isAttacking;

        private float _timeBeforeAttack = 1f;

        public RMCircleStormState(
            FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette,
            int highAnimId,
            int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("enter circle storm");
            _enterTime = Time.time;
            PlayAnimationHighBodyPart();
            PlayAnimationLowBodyPart();
            _isAttacking = false;
        }

        public override void OnExit()
        {
            Marionette.CircleSpawner.SetSpawningState(false);
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > _enterTime + _timeBeforeAttack && !_isAttacking)
            {
                _isAttacking = true;
                Marionette.CircleSpawner.SetSpawningState(true);
            }

            if (Time.time > _enterTime + _timeBeforeStateChange)
            {
                Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.Idle]);
            }
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
