//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace BTG
{
   /// <summary>
   /// AudioManager
   /// </summary>
   public class AudioManager : PersistentSingleton<AudioManager>
   {
      [SerializeField] private AudioSource audioSource;

      [SerializeField] private AudioClip[] clips;

      [SerializeField] private AudioSource _sfxSource;
      public void PlaySFX(AudioClip clip)
      {
         _sfxSource.clip = clip;  /// 3 -> 1, 5 -> 2 etc.
         _sfxSource.Play();
      }

      public void PlayCorrrectClip(int index)
      {
         audioSource.clip = clips[(index - 1) / 2];  /// 3 -> 1, 5 -> 2 etc.
         audioSource.Play();
		}

      public void PlayMenuClip()
      {
         audioSource.clip = clips[0];
         audioSource.Play();
      }
   }
}
