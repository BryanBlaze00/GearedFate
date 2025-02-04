//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
   /// <summary>
   /// Door
   /// </summary>
   public class Door : MonoBehaviour
   {
      [SerializeField] GameObject closedChild;
      [SerializeField] GameObject openChild;

      private void Start()
      {
         closedChild.SetActive(true);
         openChild.SetActive(false);
      }

      private void OnTriggerEnter2D(Collider2D other)
      {
         if (other.TryGetComponent(out Player _))
         {
            closedChild.SetActive(false);
            openChild.SetActive(true);
         }
      }

      private void OnTriggerExit2D(Collider2D other)
      {
         if (other.TryGetComponent(out Player _))
         {
            closedChild.SetActive(true);
            openChild.SetActive(false);
         }
      }
   }
}