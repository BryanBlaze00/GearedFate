//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
   /// <summary>
   /// PSSelfDestroy is a script that will destroy the object it is attached to after the particle system has finished playing.
   /// </summary>
   public class PSSelfDestroy : MonoBehaviour
   {
      private ParticleSystem ps;

      private void Awake()
      {
         ps = GetComponent<ParticleSystem>();
      }

      private void Update()
      {
         if (ps && !ps.IsAlive())
            DestroySelfAnimEvent();
      }

      private void DestroySelfAnimEvent()
      {
         Destroy(gameObject);
      }
   }
}
