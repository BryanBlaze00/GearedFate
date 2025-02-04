using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Implement this interface for objects that are instantiated at runtime and that need to be persistent.
    /// Scene objects should not implement this.
    /// </summary>
    public interface ISaveableInstance
    {
        /// <summary>
        /// The "type" of the object as used in the Pooling system.
        /// </summary>
        public PooledObjectType PooledObjectType { get; }
    }
}
