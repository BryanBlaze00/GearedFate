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
            _settingsButton.onClick.AddListener(GoSettings);
        }

        private void GoSettings()
        {
            Sequence.Create(1, CycleMode.Restart, 0f, true)
                .Group(Tween.Alpha(_fadingPanel, 1f, 1f))
                .ChainCallback(SetScreens)
                .Chain(Tween.Alpha(_fadingPanel, 0f, 1f));
        }

        private void SetScreens()
        {
            _settingScreen.SetActive(true);
            _mainMenuScreen.SetActive(false);

            if (_title) _title.SetActive(false);
        }
    }
}
