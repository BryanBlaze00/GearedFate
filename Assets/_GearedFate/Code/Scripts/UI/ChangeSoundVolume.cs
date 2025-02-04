using System;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    public class ChangeSoundVolume : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private Slider _soundSlider;

        protected void Start()
        {
            this._soundSlider.onValueChanged.AddListener(this.ChangeVolume);
        }

        private void ChangeVolume(float volume)
        {
            this._audioSource.volume = volume;
        }
    }
}
