// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// NoBossSub when there is no boss, this will activate the elevator
    /// </summary>
    public class NoBossSub : MonoBehaviour
    {
        private void Start()
        {
            Elevator.Instance.ActivateElevator();
        }
    }
}
