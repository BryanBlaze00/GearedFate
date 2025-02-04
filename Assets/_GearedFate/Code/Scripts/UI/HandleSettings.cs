//
// Copyright (c) BTG. All rights reserved.
//

using Unity.VisualScripting;
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
         SFXVolume,
      }

      [SerializeField] private GameObject soundTab;
      [SerializeField] private GameObject controlsTab;
      [SerializeField] private AudioMixer musicMaster;

      [SerializeField] private Slider masterSlider;
      [SerializeField] private Slider musicSlider;
      [SerializeField] private Slider sfxSlider;

		private void Start()
		{
         RefreshSlider();
         Controls();
         gameObject.SetActive(false);
		}

		public void Sound()
      {
         soundTab.SetActive(true);
         controlsTab.SetActive(false);
      }

      public void Controls()
      {
			soundTab.SetActive(false);
			controlsTab.SetActive(true);
		}

      private void RefreshSlider()
      {
			masterSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.MasterVolume), 100);
			musicSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.MusicVolume), 100);
			sfxSlider.value = PlayerPrefs.GetFloat(nameof(SettingsConstant.SFXVolume), 100);
		}

      private float SliderToDB(float value)
      {
         return Mathf.Log10(value/100) * 20f;
		}

      private void HandleVolume(string param, float value)
      {
         if (value < 1) value = 0.001f;
			musicMaster.SetFloat(param, SliderToDB(value));
         PlayerPrefs.SetFloat(param, value);
		}

		public void OnMasterVolumeChange(float value) =>
         HandleVolume(nameof(SettingsConstant.MasterVolume), value);

		public void OnMusicVolumeChange(float value) =>
			HandleVolume(nameof(SettingsConstant.MusicVolume), value);
		
		public void OnSFXVolumeChange(float value) =>
			HandleVolume(nameof(SettingsConstant.SFXVolume), value);
		
	}
}
