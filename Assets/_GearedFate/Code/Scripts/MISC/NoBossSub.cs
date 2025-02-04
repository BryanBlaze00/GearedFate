//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
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