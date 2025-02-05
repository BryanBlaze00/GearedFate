// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// ConveyerBelt class to provide conveyer belt functionality.
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D))]
    public class ConveyerBelt : AbstractMovingMachinery
    {
        [FormerlySerializedAs("speedX")]
        [Header("Conveyer Belt Settings")]
        [Tooltip(
            "Speed in the x-axis. Negative values move the objects to the left. Positive values move the objects to the right.")]
        [SerializeField]
        [Range(-5f, 5f)]
        private float _speedX = 1f; // Speed in the x-axis

        [FormerlySerializedAs("speedY")]
        [Tooltip("Speed in the y-axis. Negative values move the objects down. Positive values move the objects up.")]
        [SerializeField]
        [Range(-5f, 5f)]
        private float _speedY = 0f; // Speed in the y-axis

        [FormerlySerializedAs("blendVelocityFactor")]
        [Tooltip("Blend factor for the velocity. 0 means no blending, 1 means full blending.")]
        [SerializeField]
        private float _blendVelocityFactor = 0.5f; // Blend factor for the velocity

        private PolygonCollider2D _polygonCollider;

        protected override void MoveEntity(Transform entity)
        {
            var rb = entity.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                var conveyorBeltVelocity = new Vector2(_speedX, _speedY); // The *desired* velocity

                // Blend the conveyor belt velocity with the player's current velocity
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, conveyorBeltVelocity, _blendVelocityFactor);
            }
        }

        private void Start()
        {
            _polygonCollider = GetComponent<PolygonCollider2D>();
            _polygonCollider.isTrigger = true; // Make the collider a trigger

            if (_polygonCollider.points.Length != 4)
            {
                Debug.LogError("Polygon Collider 2D must have exactly 4 points! Use for corners of the conveyer belt.");
                enabled = false; // Disable the script to prevent further execution
                return;
            }
        }

        private void FixedUpdate()
        {
            MoveEntities(); // Call the MoveEntities method from the base class
        }
    }
}
