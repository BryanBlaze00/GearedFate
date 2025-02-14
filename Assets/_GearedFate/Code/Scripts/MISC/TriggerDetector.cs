// Copyright (c) BTG. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// TriggerDetector -
/// This component listens for 2D collision trigger events (Enter, Stay, Exit)
/// and dispatches UnityEvents with the colliding Collider2D as an argument.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TriggerDetector : MonoBehaviour
{
   [SerializeField]
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
   public UnityEvent<Collider2D> onTriggerEnter2D;
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
#pragma warning restore SA1401 // Fields should be private

   [SerializeField]
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
   public UnityEvent<Collider2D> onTriggerStay2D;
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
#pragma warning restore SA1401 // Fields should be private

   [SerializeField]
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
   public UnityEvent<Collider2D> onTriggerExit2D;
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
#pragma warning restore SA1401 // Fields should be private

   private void OnTriggerEnter2D(Collider2D collision)
   {
      onTriggerEnter2D?.Invoke(collision);
   }

   private void OnTriggerStay2D(Collider2D collision)
   {
      onTriggerStay2D?.Invoke(collision);
   }

   private void OnTriggerExit2D(Collider2D collision)
   {
      onTriggerExit2D?.Invoke(collision);
   }
}
