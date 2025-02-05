namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Helper component to put on entities roots, that provide a transform representing the contact point with the ground.
    /// </summary>
    public class ContactTransformProvider : MonoBehaviour
    {
        [field: SerializeField]
        public Transform ContactTransform { get; private set; }
    }
}
