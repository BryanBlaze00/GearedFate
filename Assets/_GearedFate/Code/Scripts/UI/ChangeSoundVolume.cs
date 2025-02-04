namespace BTG
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ChangeSoundVolume : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private Slider _soundSlider;

        protected void Start()
        {
            _soundSlider.onValueChanged.AddListener(ChangeVolume);
        }

        private void ChangeVolume(float volume)
        {
            _audioSource.volume = volume;
        }
    }
}
