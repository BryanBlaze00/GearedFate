//
// Copyright (c) BTG. All rights reserved.
//

using NaughtyAttributes;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Holds All Player Data
    /// </summary>
    [CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity/Player/Player Data")]
    public class PlayerData : ScriptableObject
    {
        //TODO: These are previous Code. Will change according to our needs
        [field: Header("Base Data")]
        [field: SerializeField]
        public float Health { get; private set; } = 50;

        [field: SerializeField] public float MoveSpeed { get; private set; } = 10;
        [field: SerializeField] public float DashForce { get; private set; } = 8;
        [field: SerializeField] public float DashCoolDown { get; private set; } = 1;
        [field: SerializeField] public float DashTime { get; private set; } = 0.1f;
        [field: SerializeField] public float SlashDamage { get; private set; } = 5f;
        [field: SerializeField] public float SlashRadius { get; private set; } = 0.5f;

        [field: SerializeField]
        [field: InfoBox("This is set after calculating the slash animation duration", EInfoBoxType.Normal)]
        public float SlashCoolDown { get; private set; } = 0.25f;

        [field: SerializeField] public float HitSpinChargeUpTime { get; private set; } = 1f;
        [field: SerializeField] public float MaxAttackFuelAmount { get; private set; } = 50;
        [field: SerializeField] public float GearShootCoolDown { get; private set; } = 1f;
        [field: SerializeField] public float GearFuelBurnAmount { get; private set; } = 0.5f;
        [field: SerializeField] public float FireBlazeMoveSpeed { get; private set; } = 3;
        [field: SerializeField] public Vector2 FireBlazeDimension { get; private set; } = new(0.5f, .1f);
        [field: SerializeField] public float FireBlazeDistance { get; private set; } = 2.5f;
        [field: SerializeField] public float FireBlazeDPS { get; private set; } = 30;
        [field: SerializeField] public float FireBlazeBurnRate { get; private set; } = 5;
        [field: SerializeField] public float HitStunTime { get; private set; } = 0.5f;
        [field: SerializeField] public float KnockBackTime { get; private set; } = 0.2f;
        [field: SerializeField] public LayerMask EnemyLayerMask { get; private set; }
    }
}
