using System;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Script for gear machinery that moves movable entity in circle, following the gear animation.
    /// </summary>
    public class MovingGearMachinery : AbstractMovingMachinery
    {
        // Reference to the gear animation script
        [SerializeField] private GearAnim _gearAnim;
        private SpriteRenderer parentSpriteRenderer;

        private void Awake()
        {
            parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        }

        protected void Start()
        {
            // Each time the animation of the gear moves, we should move entities with it.
            _gearAnim.OnGearMoved += MoveEntities;
        }

        protected override void MoveEntity(Transform entity)
        {
            var angle = 1f / _gearAnim.FullCircleRotationNumbers * 360f;

            if (entity.TryGetComponent(out ContactTransformProvider contactTransformProvider))
            {
                // If the feet are not the 0,0 position of the entity we move, we must be careful and not directly rotate the entity.
                // Instead, we rotate a gameObject at the position of the feet, to know where the feet should end up.
                var temp = new GameObject();
                temp.transform.position = contactTransformProvider.ContactTransform.position;
                temp.transform.RotateAround(transform.position, Vector3.forward, -angle);

                // When figuring out where the feet should be, it's straightforward :
                // The entity should end up at the feet position + the distance between the entity center and its feet.
                var offset = entity.position - contactTransformProvider.ContactTransform.position;
                entity.transform.position = temp.transform.position + offset;

                // TODO use pooling instead
                Destroy(temp);
            }
            else
            {
                if (parentSpriteRenderer.flipX == false)
                    entity.RotateAround(transform.position, Vector3.forward, -angle);
                else
                    entity.RotateAround(transform.position, Vector3.forward, +angle);
            }
        }
    }
}
