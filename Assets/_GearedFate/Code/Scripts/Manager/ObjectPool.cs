//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace BTG
{
    [System.Serializable]
    public class ObjectToPoolInfo
    {
        [field: SerializeField] public GameObject ObjectToPool { get; private set; }

        [field: SerializeField]
        [field: Range(2, 100)]
        public int AmountToPool { get; private set; }

        [field: SerializeField] public bool ShouldExpand { get; private set; } = true;


        public ObjectToPoolInfo(GameObject obj, int amt = 2, bool exp = true)
        {
            this.ObjectToPool = obj;
            this.AmountToPool = amt;
            this.ShouldExpand = exp;
        }
    }

    [System.Serializable]
    public class ObjectToPoolWrapper
    {
        [field: SerializeField] public ObjectToPoolInfo ObjectToPoolInfo { get; private set; }
        [field: SerializeField] public PooledObjectType ObjectType { get; private set; }
    }

    /// <summary>
    /// ObjectPool
    /// </summary>
    public class ObjectPool : Singleton<ObjectPool>
    {
        [SerializeField] private List<ObjectToPoolWrapper> objectsToPoolList;
        private readonly Dictionary<PooledObjectType, ObjectToPoolInfo> objectsToPool = new();

        public Dictionary<PooledObjectType, List<GameObject>> PooledObjects { get; } = new();

        protected override void Awake()
        {
            base.Awake();

            foreach (var obj in this.objectsToPoolList)
            {
                if (this.objectsToPool.ContainsKey(obj.ObjectType))
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

                this.objectsToPool.Add(obj.ObjectType, obj.ObjectToPoolInfo);
                this.ObjectPoolItemToPooledObject(obj.ObjectType);
            }

            this.objectsToPoolList = null;
        }

        public GameObject GetPooledObject(PooledObjectType type)
        {
            if (!this.PooledObjects.ContainsKey(type)) return null;
            for (var i = 0; i < this.PooledObjects[type].Count; i++)
                if (!this.PooledObjects[type][i].activeInHierarchy)
                    return this.PooledObjects[type][i];

            if (this.objectsToPool[type].ShouldExpand)
            {
                var obj = this.AddObjectToPool(this.objectsToPool[type]);
                this.PooledObjects[type].Add(obj);
                return obj;
            }

            return null;
        }

        public List<GameObject> GetAllPooledObjects(PooledObjectType type)
        {
            this.PooledObjects.TryGetValue(type, out var obj);
            return obj;
        }


        public void AddObject(GameObject GO, int amt, bool exp = true, PooledObjectType type = PooledObjectType.None)
        {
            if (this.PooledObjects.ContainsKey(type))
            {
#if UNITY_EDITOR
                Debug.LogWarning("Object already exists in pool!");
                return;
#endif
            }

            ObjectToPoolInfo item = new(GO, amt, exp);
            this.objectsToPool.Add(type, item);
            this.ObjectPoolItemToPooledObject(type);
        }

        public GameObject AddObjectToPool(ObjectToPoolInfo item)
        {
            var obj = Instantiate(item.ObjectToPool);
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            return obj;
        }

        public void ReturnPooledObject(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void ObjectPoolItemToPooledObject(PooledObjectType type)
        {
            var item = this.objectsToPool[type];

            var pooledObjects = new List<GameObject>();
            for (var i = 0; i < item.AmountToPool; i++) pooledObjects.Add(this.AddObjectToPool(item));
            this.PooledObjects.Add(type, pooledObjects);
        }
    }
}
