namespace BTG
{
    using UnityEngine;

    [System.Serializable]
    public class ObjectToPoolInfo
    {
        public ObjectToPoolInfo(GameObject obj, int amt = 2, bool exp = true)
        {
            ObjectToPool = obj;
            AmountToPool = amt;
            ShouldExpand = exp;
        }

        [field: SerializeField]
        public GameObject ObjectToPool { get; private set; }

        [field: SerializeField]
        [field: Range(2, 100)]
        public int AmountToPool { get; private set; }

        [field: SerializeField]
        public bool ShouldExpand { get; private set; }
    }
}
