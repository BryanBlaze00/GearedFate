namespace BTG
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Abstract base class for machinery that can move around entities.
    /// </summary>
    public abstract class AbstractMovingMachinery : MonoBehaviour
    {
        /// <summary>
        /// Keep track of the entity this machinery should move.
        /// </summary>
        private readonly HashSet<Transform> _entitiesToMove = new ();

        /// <summary>
        /// Method called for each entity that should be moved. The movement differs depending on the machinery.
        /// </summary>
        /// <param name="entity"> The entity transform to move</param>
        protected abstract void MoveEntity(Transform entity);

        protected void OnTriggerEnter2D(Collider2D other)
        {
            // For a transform to be moved, it needs to have a collider tagged Movable Collider, and it needs to provide a root transform.
            if (other.CompareTag("MovableCollider") &&
                other.TryGetComponent(out RootTransformProvider rootTransformProvider))
            {
                if (rootTransformProvider.RootTransform)
                {
                    _entitiesToMove.Add(rootTransformProvider.RootTransform);
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("MovableCollider") &&
                other.TryGetComponent(out RootTransformProvider rootTransformProvider))
            {
                if (rootTransformProvider.RootTransform)
                {
                    _entitiesToMove.Remove(rootTransformProvider.RootTransform);
                }
            }
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("MovableCollider") &&
                other.TryGetComponent(out RootTransformProvider rootTransformProvider))
            {
                if (rootTransformProvider.RootTransform)
                {
                    _entitiesToMove.Add(rootTransformProvider.RootTransform);
                }
            }
        }

        protected void MoveEntities()
        {
            foreach (var entity in _entitiesToMove)
            {
                MoveEntity(entity);
            }
        }
    }
}
