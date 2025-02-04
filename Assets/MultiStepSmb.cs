using System;
using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// A State machine behaviour that will call events at different normalized times in the state
    /// </summary>
    public class MultiStepSmb : StateMachineBehaviour
    {
        public event Action<int> OnStepReached;

        private int _nextStepIndex;

        private float _nextStep;

        [SerializeField]
        private List<float> _steps;

        [field:SerializeField]
        public string Id { get; private set; }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_steps.Count != 0)
            {
                _nextStepIndex = 0;
                _nextStep = _steps[0];
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_nextStepIndex == _steps.Count)
            {
                return;
            }

            if (stateInfo.normalizedTime >= _nextStep)
            {
                OnStepReached?.Invoke(_nextStepIndex);
                _nextStepIndex++;

                if (_nextStepIndex == _steps.Count)
                {
                    return;
                }

                _nextStep = _steps[_nextStepIndex];
            }
        }
    }
}
