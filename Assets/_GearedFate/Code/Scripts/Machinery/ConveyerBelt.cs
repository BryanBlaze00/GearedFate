//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
   /// <summary>
   /// ConveyerBelt class to provide conveyer belt functionality.
   /// </summary>
   [RequireComponent(typeof(PolygonCollider2D))]
   public class ConveyerBelt : AbstractMovingMachinery
   {
      [Header("Conveyer Belt Settings")]
      [Tooltip("Speed in the x-axis. Negative values move the objects to the left. Positive values move the objects to the right.")]
      [SerializeField][Range(-5f, 5f)] float speedX = 1f; // Speed in the x-axis

      [Tooltip("Speed in the y-axis. Negative values move the objects down. Positive values move the objects up.")]
      [SerializeField][Range(-5f, 5f)] float speedY = 0f; // Speed in the y-axis
      
      [Tooltip("Blend factor for the velocity. 0 means no blending, 1 means full blending.")]
      [SerializeField] float blendVelocityFactor = 0.5f; // Blend factor for the velocity

      private PolygonCollider2D polygonCollider;


      private void Start()
      {
         polygonCollider = GetComponent<PolygonCollider2D>();
         polygonCollider.isTrigger = true; // Make the collider a trigger

         if (polygonCollider.points.Length != 4)
         {
            Debug.LogError("Polygon Collider 2D must have exactly 4 points! Use for corners of the conveyer belt.");
            enabled = false; // Disable the script to prevent further execution
            return;
         }
      }

      protected override void MoveEntity(Transform entity)
      {
         Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();

         if (rb != null)
         {
            Vector2 conveyorBeltVelocity = new Vector2(speedX, speedY); // The *desired* velocity

            // Blend the conveyor belt velocity with the player's current velocity
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, conveyorBeltVelocity, blendVelocityFactor);
         }
      }

      private void FixedUpdate()
      {
         MoveEntities(); // Call the MoveEntities method from the base class
      }
   }
}
