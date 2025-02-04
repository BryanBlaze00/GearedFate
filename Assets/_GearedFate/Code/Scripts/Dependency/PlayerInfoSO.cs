//
// Copyright (c) BTG. All rights reserved.
//

using NaughtyAttributes;
using UnityEngine;

namespace BTG
{
   /// <summary>
   /// Carries and transfers player state information to other scripts
   /// This is used to remove any direct dependencies among player and any other scripts
   /// Necessary Player information is updated so other scripts can retrieve such data
   /// </summary>

   [CreateAssetMenu(fileName = "newPlayerInfoSO", menuName = "Data/Entity/Player/Player Info SO", order = 0)]
   public class PlayerInfoSO : ScriptableObject
   {
      [ReadOnly] public Vector2 position;
   }
}
