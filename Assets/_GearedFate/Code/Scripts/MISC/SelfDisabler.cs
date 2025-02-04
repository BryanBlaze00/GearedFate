//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
   /// <summary>
   /// SelfDisabler
   /// </summary>
   public class SelfDisabler : MonoBehaviour
   {
      public void DisableSelf() => gameObject.SetActive(false);
   }
}
