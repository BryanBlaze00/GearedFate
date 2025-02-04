using UnityEngine;

namespace BTG
{
    public abstract class RMBaseState : BaseState<RustedMarionette.RustedMarionetteState>
    {
        protected RustedMarionette Marionette { get; }

        private static int _phase;

        private int _highAnimId;

        private int _lowAnimId;

        protected RMBaseState(FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette, int highAnimId, int lowAnimId) : base(fsm)
        {
            this.Marionette = marionette;
            this.Marionette.OnHitTaken += this.HandleHitTaken;
            _phase = 0;
            this._lowAnimId = lowAnimId;
            this._highAnimId = highAnimId;
        }

        private void HandleHitTaken()
        {
            if (this.Marionette.CurrentHealth == 0)
            {
                Debug.Log("dead marionette");
                this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.Death]);
                return;
            }

            if (this.Marionette.CurrentHealth / this.Marionette.MaxHealth < 0.7f && _phase == 0)
            {
                Debug.Log("go full maze");
                _phase++;
                this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.StringMaze]);
            }

            if (this.Marionette.CurrentHealth / this.Marionette.MaxHealth < 0.4f && _phase == 1)
            {
                Debug.Log("go full maze");
                _phase++;
                this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.StringMaze]);
            }
        }

        protected void PlayAnimationHighBodyPart()
        {
            this.Marionette.AnimatorHighPart.Play(this._highAnimId);
        }

        protected void PlayAnimationLowBodyPart()
        {
            this.Marionette.AnimatorHighPart.Play(this._lowAnimId);
        }

        public abstract override void OnEnter();


        public abstract override void OnExit();


        public abstract override void OnFrameUpdate();


        public abstract override void OnPhysicsUpdate();
    }
}
