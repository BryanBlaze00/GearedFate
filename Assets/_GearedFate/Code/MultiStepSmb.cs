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
            if (this._steps.Count != 0)
            {
                this._nextStepIndex = 0;
                this._nextStep = this._steps[0];
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this._nextStepIndex == this._steps.Count)
            {
                return;
            }

            if (stateInfo.normalizedTime >= this._nextStep)
            {
                this.OnStepReached?.Invoke(this._nextStepIndex);
                this._nextStepIndex++;

                if (this._nextStepIndex == this._steps.Count)
                {
                    return;
                }

                this._nextStep = this._steps[this._nextStepIndex];
            }
        }
    }
}
