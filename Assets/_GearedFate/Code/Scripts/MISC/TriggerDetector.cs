// Copyright (c) BTG. All rights reserved.

using System;
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
   public event Action<Collider2D> OnTriggerDetectorEnter2D;

   [SerializeField]
   public event Action<Collider2D> OnTriggerDetectorStay2D;

   [SerializeField]
   public event Action<Collider2D> OnTriggerDetectorExit2D;

   private void OnTriggerEnter2D(Collider2D collision)
   {
      OnTriggerDetectorEnter2D?.Invoke(collision);
   }

   private void OnTriggerStay2D(Collider2D collision)
   {
      OnTriggerDetectorStay2D?.Invoke(collision);
   }

   private void OnTriggerExit2D(Collider2D collision)
   {
      OnTriggerDetectorExit2D?.Invoke(collision);
   }
}
