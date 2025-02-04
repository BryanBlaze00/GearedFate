// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System;
    using UnityEngine;

    /// <summary>
    /// PlayerAnimationEventHandler
    /// </summary>
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        #region FireBlaze

        public event Action OnChargeUpFinishedEvent;

        [SerializeField]
        private void OnChargeUpFinished()
        {
            OnChargeUpFinishedEvent.Invoke();
        }

        #endregion FireBlaze

        #region FireSpin

        public event Action OnSpinChargeUpEvent;

        public event Action OnSpinFinishedEvent;

        [SerializeField]
        private void OnSpinChargeUp()
        {
            OnSpinChargeUpEvent.Invoke();
        }

        [SerializeField]
        private void OnSpinFinished()
        {
            OnSpinFinishedEvent.Invoke();
        }

        #endregion FireSpin

        public event Action OnGearTossEvent;

        public event Action OnGearTossFinishedEvent;

        [SerializeField]
        private void OnGearToss()
        {
            OnGearTossEvent?.Invoke();
        }

        [SerializeField]
        private void OnGearTossFinished()
        {
            OnGearTossFinishedEvent.Invoke();
        }
    }
}
