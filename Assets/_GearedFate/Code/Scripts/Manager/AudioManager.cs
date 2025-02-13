// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityUtils;

    /// <summary>
    /// AudioManager
    /// </summary>
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip[] musicClips;

        [SerializeField]
        private AudioSource _sfxSource;

        /// <summary>
        /// Play an SFX audio clip.
        /// </summary>
        /// <param name="clip"> The AudioClip to play. </param>
        public void PlaySFX(AudioClip clip)
        {
            _sfxSource.clip = clip;
            _sfxSource.Play();
        }

        /// <summary>
        /// Plays the corresponding music clip based on the given index.
        /// The index is expected to be an odd number (1, 3, 5, etc.).
        /// The actual music clip index is calculated as (index - 1) / 2.
        /// e.g. 3 -> 1, 5 -> 2 etc.
        /// </summary>
        /// <param name="index">The index of the music clip to play.</param>
        public void PlayCorrrectMusicClip(int index)
        {
            audioSource.clip = musicClips[(index - 1) / 2];
            audioSource.Play();
        }

        /// <summary>
        /// Plays the first music clip in the `musicClips` array.
        /// This is typically used to play the background music for the main menu.
        /// </summary>
        public void PlayMenuMusicClip()
        {
            audioSource.clip = musicClips[0];
            audioSource.Play();
        }

        /// <summary>
        /// Stops all music playback.
        /// </summary>
        public void StopMusic()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// Stops all SFX playback.
        /// </summary>
        public void StopSFX()
        {
            _sfxSource.Stop();
        }
    }
}
