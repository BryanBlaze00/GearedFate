namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Helper component to provide a root transform from any child transform. More reliable than using transform.root.
    /// </summary>
    public class RootTransformProvider : MonoBehaviour
    {
        [SerializeField]
        private Transform rootTransform; // Make it private for serialization

        public Transform RootTransform
        {
            get
            {
                if (rootTransform == null)
                {
                    // Check if there's a parent
                    if (transform.parent != null)
                    {
                        rootTransform = transform.parent; // Assign the immediate parent
                    }
                    else
                    {
                        Debug.LogWarning("This GameObject has no parent!");
                        rootTransform = transform; // Or rootTransform = null;
                    }
                }

                return rootTransform;
            }
            private set => rootTransform = value;
        }
    }
}
