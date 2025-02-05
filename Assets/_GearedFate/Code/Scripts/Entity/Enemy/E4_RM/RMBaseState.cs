namespace BTG
{
    using UnityEngine;

    public abstract class RMBaseState : BaseState<RustedMarionette.RustedMarionetteState>
    {
        private static int _phase;

        private int _highAnimId;

        private int _lowAnimId;

        protected RMBaseState(
            FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette,
            int highAnimId,
            int lowAnimId)
            : base(fsm)
        {
            Marionette = marionette;
            Marionette.OnHitTaken += HandleHitTaken;
            _phase = 0;
            _lowAnimId = lowAnimId;
            _highAnimId = highAnimId;
        }

        protected RustedMarionette Marionette { get; }

        public abstract override void OnEnter();

        public abstract override void OnExit();

        public abstract override void OnFrameUpdate();

        public abstract override void OnPhysicsUpdate();

        protected void PlayAnimationHighBodyPart()
        {
            Marionette.AnimatorHighPart.Play(_highAnimId);
        }

        protected void PlayAnimationLowBodyPart()
        {
            Marionette.AnimatorHighPart.Play(_lowAnimId);
        }

        private void HandleHitTaken()
        {
            if (Marionette.CurrentHealth == 0)
            {
                Debug.Log("dead marionette");
                Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.Death]);
                return;
            }

            if (Marionette.CurrentHealth / Marionette.MaxHealth < 0.7f && _phase == 0)
            {
                Debug.Log("go full maze");
                _phase++;
                Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.StringMaze]);
            }

            if (Marionette.CurrentHealth / Marionette.MaxHealth < 0.4f && _phase == 1)
            {
                Debug.Log("go full maze");
                _phase++;
                Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.StringMaze]);
            }
        }
    }
}
