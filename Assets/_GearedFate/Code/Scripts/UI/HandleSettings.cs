//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace BTG
{
    /// <summary>
    /// HandleSettings
    /// </summary>
    public class HandleSettings : MonoBehaviour
    {
        public enum SettingsConstant
        {
            MasterVolume,
            MusicVolume,
            SFXVolume
        }

        [SerializeField] private GameObject soundTab;
        [SerializeField] private GameObject controlsTab;
        [SerializeField] private AudioMixer musicMaster;

        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            this.RefreshSlider();
            this.Controls();
            this.gameObject.SetActive(false);
        }

        public void Sound()
        {
            this.soundTab.SetActive(true);
            this.controlsTab.SetActive(false);
        }

        public void Controls()
        {
            this.soundTab.SetActive(false);
            this.controlsTab.SetActive(true);
        }

        private void RefreshSlider()
        {
            this.masterSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.MasterVolume), 100);
            this.musicSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.MusicVolume), 100);
            this.sfxSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.SFXVolume), 100);
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

            this.musicMaster.SetFloat(param, this.SliderToDB(value));
            PlayerPrefs.SetFloat(param, value);
        }

        public void OnMasterVolumeChange(float value)
        {
            this.HandleVolume(nameof(SettingsConstant.MasterVolume), value);
        }

        public void OnMusicVolumeChange(float value)
        {
            this.HandleVolume(nameof(SettingsConstant.MusicVolume), value);
        }

        public void OnSFXVolumeChange(float value)
        {
            this.HandleVolume(nameof(SettingsConstant.SFXVolume), value);
        }
    }
}
