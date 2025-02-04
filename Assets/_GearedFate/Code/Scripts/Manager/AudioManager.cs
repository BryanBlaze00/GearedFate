//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
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
            this._sfxSource.clip = clip; /// 3 -> 1, 5 -> 2 etc.
            this._sfxSource.Play();
        }

        public void PlayCorrrectClip(int index)
        {
            this.audioSource.clip = this.clips[(index - 1) / 2]; /// 3 -> 1, 5 -> 2 etc.
            this.audioSource.Play();
        }

        public void PlayMenuClip()
        {
            this.audioSource.clip = this.clips[0];
            this.audioSource.Play();
        }
    }
}
