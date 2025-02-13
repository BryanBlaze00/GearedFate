namespace BTG
{
    using UnityEngine;

    [System.Serializable]
    public class ObjectToPoolWrapper
    {
        [field: SerializeField]
        public ObjectToPoolInfo ObjectToPoolInfo { get; private set; }

        [field: SerializeField]
        public PooledObjectType ObjectType { get; private set; }
    }
}
