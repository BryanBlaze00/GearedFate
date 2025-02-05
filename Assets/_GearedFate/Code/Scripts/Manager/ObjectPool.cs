// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityUtils;

    [System.Serializable]
    public class ObjectToPoolInfo
    {
        [field: SerializeField]
        public GameObject ObjectToPool { get; private set; }

        [field: SerializeField]
        [field: Range(2, 100)]
        public int AmountToPool { get; private set; }

        [field: SerializeField]
        public bool ShouldExpand { get; private set; }

        public ObjectToPoolInfo(GameObject obj, int amt = 2, bool exp = true)
        {
            ObjectToPool = obj;
            AmountToPool = amt;
            ShouldExpand = exp;
        }
    }

    [System.Serializable]
    public class ObjectToPoolWrapper
    {
        [field: SerializeField]
        public ObjectToPoolInfo ObjectToPoolInfo { get; private set; }

        [field: SerializeField]
        public PooledObjectType ObjectType { get; private set; }
    }

    /// <summary>
    /// ObjectPool
    /// </summary>
    public class ObjectPool : Singleton<ObjectPool>
    {
        [SerializeField]
        private List<ObjectToPoolWrapper> objectsToPoolList;
        private readonly Dictionary<PooledObjectType, ObjectToPoolInfo> objectsToPool = new();

        public Dictionary<PooledObjectType, List<GameObject>> PooledObjects { get; } = new();

        protected override void Awake()
        {
            base.Awake();

            foreach (var obj in objectsToPoolList)
            {
                if (objectsToPool.ContainsKey(obj.ObjectType))
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Object skipped. Key already exists in pool!!");
#endif
                    continue;
                }

                if (!obj.ObjectToPoolInfo.ObjectToPool)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Object skipped. Null object Found!!");
#endif
                    continue;
                }

                objectsToPool.Add(obj.ObjectType, obj.ObjectToPoolInfo);
                ObjectPoolItemToPooledObject(obj.ObjectType);
            }

            objectsToPoolList = null;
        }

        public GameObject GetPooledObject(PooledObjectType type)
        {
            if (!PooledObjects.ContainsKey(type))
            {
                return null;
            }

            for (var i = 0; i < PooledObjects[type].Count; i++)
            {
                if (!PooledObjects[type][i].activeInHierarchy)
                {
                    return PooledObjects[type][i];
                }
            }

            if (objectsToPool[type].ShouldExpand)
            {
                var obj = AddObjectToPool(objectsToPool[type]);
                PooledObjects[type].Add(obj);
                return obj;
            }

            return null;
        }

        public List<GameObject> GetAllPooledObjects(PooledObjectType type)
        {
            PooledObjects.TryGetValue(type, out var obj);
            return obj;
        }

        public void AddObject(GameObject GO, int amt, bool exp = true, PooledObjectType type = PooledObjectType.None)
        {
            if (PooledObjects.ContainsKey(type))
            {
#if UNITY_EDITOR
                Debug.LogWarning("Object already exists in pool!");
                return;
#endif
            }

            ObjectToPoolInfo item = new(GO, amt, exp);
            objectsToPool.Add(type, item);
            ObjectPoolItemToPooledObject(type);
        }

        public GameObject AddObjectToPool(ObjectToPoolInfo item)
        {
            var obj = Instantiate(item.ObjectToPool, transform, true);
            obj.SetActive(false);
            return obj;
        }

        public void ReturnPooledObject(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void ObjectPoolItemToPooledObject(PooledObjectType type)
        {
            var item = objectsToPool[type];

            var pooledObjects = new List<GameObject>();
            for (var i = 0; i < item.AmountToPool; i++)
            {
                pooledObjects.Add(AddObjectToPool(item));
            }

            PooledObjects.Add(type, pooledObjects);
        }
    }
}
