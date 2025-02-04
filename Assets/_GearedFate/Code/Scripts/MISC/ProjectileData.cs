// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Projectile Data
    /// </summary>
    public enum ProjectileEffect
    {
        None,
        DOT,
        Freeze,
        Slow,
    }

    [CreateAssetMenu(fileName = "newProjectileData", menuName = "Data/ProjectileData", order = 0)]
    public class ProjectileData : ScriptableObject
    {
        [field: SerializeField]
        public int Damage { get; private set; }

        [field: SerializeField]
        public float Speed { get; private set; }

        [field: SerializeField]
        public PooledObjectType PooledObjectType { get; private set; }

        [field: SerializeField]
        public ProjectileEffect Effect { get; private set; } //TODO: Might be implemented later

        [field: SerializeField]
        public float EffectData { get; private set; } //TODO: Might be implemented later
    }
}
