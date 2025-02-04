//
// Copyright (c) BTG. All rights reserved.
//

using System;
using UnityEngine;

namespace BTG
{
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
            this.OnChargeUpFinishedEvent.Invoke();
        }

        #endregion FireBlaze

        #region FireSpin

        public event Action OnSpinChargeUpEvent;
        public event Action OnSpinFinishedEvent;

        [SerializeField]
        private void OnSpinChargeUp()
        {
            this.OnSpinChargeUpEvent.Invoke();
        }

        [SerializeField]
        private void OnSpinFinished()
        {
            this.OnSpinFinishedEvent.Invoke();
        }

        #endregion FireSpin

        #region GearToss

        public event Action OnGearTossEvent;
        public event Action OnGearTossFinishedEvent;

        [SerializeField]
        private void OnGearToss()
        {
            this.OnGearTossEvent?.Invoke();
        }

        [SerializeField]
        private void OnGearTossFinished()
        {
            this.OnGearTossFinishedEvent.Invoke();
        }

        #endregion GearToss
    }
}
