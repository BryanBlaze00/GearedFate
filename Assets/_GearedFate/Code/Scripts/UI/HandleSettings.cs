// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    /// <summary>
    /// HandleSettings
    /// </summary>
    public class HandleSettings : MonoBehaviour
    {
        [FormerlySerializedAs("soundTab")]
        [SerializeField]
        private GameObject _soundTab;

        [FormerlySerializedAs("controlsTab")]
        [SerializeField]
        private GameObject _controlsTab;

        [FormerlySerializedAs("musicMaster")]
        [SerializeField]
        private AudioMixer _musicMaster;

        [FormerlySerializedAs("masterSlider")]
        [SerializeField]
        private Slider _masterSlider;

        [FormerlySerializedAs("musicSlider")]
        [SerializeField]
        private Slider _musicSlider;

        [FormerlySerializedAs("sfxSlider")]
        [SerializeField]
        private Slider _sfxSlider;

        public enum SettingsConstant
        {
            MasterVolume,
            MusicVolume,
            SFXVolume,
        }

        public void Sound()
        {
            _soundTab.SetActive(true);
            _controlsTab.SetActive(false);
        }

        public void Controls()
        {
            _soundTab.SetActive(false);
            _controlsTab.SetActive(true);
        }

        public void OnMasterVolumeChange(float value)
        {
            HandleVolume(nameof(SettingsConstant.MasterVolume), value);
        }

        public void OnMusicVolumeChange(float value)
        {
            HandleVolume(nameof(SettingsConstant.MusicVolume), value);
        }

        public void OnSFXVolumeChange(float value)
        {
            HandleVolume(nameof(SettingsConstant.SFXVolume), value);
        }

        private void Start()
        {
            RefreshSlider();
            Controls();
            gameObject.SetActive(false);
        }

        private void RefreshSlider()
        {
            _masterSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.MasterVolume), 100);
            _musicSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.MusicVolume), 100);
            _sfxSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.SFXVolume), 100);
        }

        private float SliderToDB(float value)
        {
            return Mathf.Log10(value / 100) * 20f;
        }

        private void HandleVolume(string param, float value)
        {
            if (value < 1)
            {
                value = 0.001f;
            }

            _musicMaster.SetFloat(param, SliderToDB(value));
            PlayerPrefs.SetFloat(param, value);
        }
    }
}
