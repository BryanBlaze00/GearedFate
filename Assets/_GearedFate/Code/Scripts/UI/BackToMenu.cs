using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    public class BackToMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _settingScreen;

        [SerializeField]
        private GameObject _mainMenuScreen;

        [SerializeField]
        private GameObject _title;

        [SerializeField]
        private Image _fadingPanel;

        [SerializeField]
        private Button _backToMenuButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _backToMenuButton.onClick.AddListener(GoMenu);
        }

        private void GoMenu()
        {
            Sequence.Create(1, CycleMode.Restart, 0f, true)
                .Group(Tween.Alpha(_fadingPanel, 1f, 1f))
                .ChainCallback(SetScreens)
                .Chain(Tween.Alpha(_fadingPanel, 0f, 1f));
        }

        private void SetScreens()
        {
            _settingScreen.SetActive(false);
            _mainMenuScreen.SetActive(true);

            if (_title)
            {
                _title.SetActive(true);
            }
        }
    }
}
