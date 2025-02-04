using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    public class BackToMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _settingScreen;

        [SerializeField] private GameObject _mainMenuScreen;

        [SerializeField] private GameObject _title;

        [SerializeField] private Image _fadingPanel;

        [SerializeField] private Button _backToMenuButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            this._backToMenuButton.onClick.AddListener(this.GoMenu);
        }

        private void GoMenu()
        {
            Sequence.Create(1, CycleMode.Restart, 0f, true)
                .Group(Tween.Alpha(this._fadingPanel, 1f, 1f))
                .ChainCallback(this.SetScreens)
                .Chain(Tween.Alpha(this._fadingPanel, 0f, 1f));
        }

        private void SetScreens()
        {
            this._settingScreen.SetActive(false);
            this._mainMenuScreen.SetActive(true);

            if (this._title) this._title.SetActive(true);
        }
    }
}
