using System;
using UnityEngine;

namespace BTG
{
    public class TgcDashSmb : StateMachineBehaviour
    {
        public event Action OnStartDash;

        public event Action OnDashEnd;

        public event Action OnAnimationEnd;

        private bool _dashStartReached;

        private bool _dashEndReached;

        private bool _animationEndReached;

        [SerializeField]
        private float _frameStartTarget;

        [SerializeField]
        private float _frameEndTarget;

        [SerializeField]
        private float _animationEndTarget;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            this._dashStartReached = false;
            this._dashEndReached = false;
            this._animationEndReached = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime >= this._frameStartTarget && !this._dashStartReached)
            {
                this._dashStartReached = true;
                this.OnStartDash?.Invoke();
            }

            if (stateInfo.normalizedTime >= this._frameEndTarget && !this._dashEndReached)
            {
                this._dashEndReached = true;
                this.OnDashEnd?.Invoke();
            }

            if (stateInfo.normalizedTime >= this._animationEndTarget && !this._animationEndReached)
            {
                this._animationEndReached = true;
                this.OnAnimationEnd?.Invoke();
            }
        }
    }
}
