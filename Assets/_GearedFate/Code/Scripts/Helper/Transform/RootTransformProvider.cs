using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Helper component to provide a root transform from any child transform. More reliable than using transform.root.
    /// </summary>
    public class RootTransformProvider : MonoBehaviour
    {
        [SerializeField] private Transform rootTransform; // Make it private for serialization

        public Transform RootTransform
        {
            get
            {
                if (this.rootTransform == null)
                {
                    if (this.transform.parent != null) // Check if there's a parent
                    {
                        this.rootTransform = this.transform.parent; // Assign the immediate parent
                    }
                    else
                    {
                        Debug.LogWarning("This GameObject has no parent!");
                        this.rootTransform = this.transform; // Or rootTransform = null;
                    }
                }

                return this.rootTransform;
            }
            private set => this.rootTransform = value;
        }
    }
}
