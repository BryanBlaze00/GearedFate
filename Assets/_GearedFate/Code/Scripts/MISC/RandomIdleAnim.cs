// -
// Copyright (c) BTG. All rights reserved.
// -

using BTG;
using UnityEngine;

/// <summary>
/// RandomIdleAnim - Plays idle animations on random.
/// </summary>
public class RandomIdleAnim : MonoBehaviour
{
   private Animator animator;

   private void Awake()
   {
      animator = GetComponent<Animator>();
   }

   private void Start()
   {
      AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
      animator.Play(animatorStateInfo.fullPathHash, -1, RandomUtility.RandomFloat(0f, 1f));
   }
}
