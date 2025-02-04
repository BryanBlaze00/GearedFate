using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    public class GoToSettings : MonoBehaviour
    {
        [SerializeField] private GameObject _settingScreen;

        [SerializeField] private GameObject _mainMenuScreen;

        [SerializeField] private GameObject _title;

        [SerializeField] private Image _fadingPanel;

        [SerializeField] private Button _settingsButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            this._settingsButton.onClick.AddListener(this.GoSettings);
        }

        private void GoSettings()
        {
            Sequence.Create(1, CycleMode.Restart, 0f, true)
                .Group(Tween.Alpha(this._fadingPanel, 1f, 1f))
                .ChainCallback(this.SetScreens)
                .Chain(Tween.Alpha(this._fadingPanel, 0f, 1f));
        }

        private void SetScreens()
        {
            this._settingScreen.SetActive(true);
            this._mainMenuScreen.SetActive(false);

            if (this._title) this._title.SetActive(false);
        }
    }
}
