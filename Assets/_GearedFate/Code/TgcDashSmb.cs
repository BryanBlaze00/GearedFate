namespace BTG
{
    using System;
    using UnityEngine;

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
            _dashStartReached = false;
            _dashEndReached = false;
            _animationEndReached = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime >= _frameStartTarget && !_dashStartReached)
            {
                _dashStartReached = true;
                OnStartDash?.Invoke();
            }

            if (stateInfo.normalizedTime >= _frameEndTarget && !_dashEndReached)
            {
                _dashEndReached = true;
                OnDashEnd?.Invoke();
            }

            if (stateInfo.normalizedTime >= _animationEndTarget && !_animationEndReached)
            {
                _animationEndReached = true;
                OnAnimationEnd?.Invoke();
            }
        }
    }
}
