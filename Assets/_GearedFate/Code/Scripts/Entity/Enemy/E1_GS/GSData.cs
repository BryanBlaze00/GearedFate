// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Geared Sentinel Data
    /// </summary>
    [CreateAssetMenu(fileName = "newGSData", menuName = "Data/Entity/Enemy/GSData", order = 0)]
    public class GSData : ScriptableObject
    {
        [field: SerializeField]
        public float BaseMoveSpeed { get; private set; }

        [field: SerializeField]
        public float BurrowSpeedMultiplier { get; private set; }

        [field: SerializeField]
        public float BurrowDamage { get; private set; }

        [field: SerializeField]
        public float MaxHealth { get; private set; }

        [field: SerializeField]
        public float BaseDistanceGoal { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        public List<float> StageTransitionHealthPercentage { get; private set; }

        [Header("Stage 0 Data")]
        [field: SerializeField]
        [field: Range(0, 360)]
        public float Stage0ShotgunSpreadDegrees { get; private set; }

        [field: SerializeField]
        [field: Range(0, 20)]
        public int Stage0ShotgunProjCount { get; private set; }

        [Header("Stage 1 Data")]
        [field: SerializeField]
        [field: Range(0, 360)]
        public float Stage1ShotgunSpreadDegrees { get; private set; }

        [field: SerializeField]
        [field: Range(0, 20)]
        public int Stage1ShotgunProjCount { get; private set; }

        [Header("Stage 2 Data")]
        [field: SerializeField]
        [field: Range(0, 100)]
        public float Stage2Spray360ProjPerRotation { get; private set; }

        [field: SerializeField]
        [field: Range(0, 10)]
        public float Stage2Spray360RotationsPerSecond { get; private set; }

        [field: SerializeField]
        [field: Range(0, 10)]
        public int Stage2Spray360NumRotations { get; private set; }

        private void OnValidate()
        {
            if (StageTransitionHealthPercentage.Count > 3)
            {
                StageTransitionHealthPercentage.RemoveRange(3, StageTransitionHealthPercentage.Count - 3);
            }
        }
    }
}
