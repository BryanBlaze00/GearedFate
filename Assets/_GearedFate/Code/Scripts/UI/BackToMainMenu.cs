namespace BTG
{
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class BackToMainMenu : MonoBehaviour
    {
        [SerializeField]
        private Image _fadingPanel;

        [SerializeField]
        private Button _backToMenuButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _backToMenuButton.onClick.AddListener(GoMenu);
        }

        private void GoMenu()
        {
            Sequence.Create(1, CycleMode.Restart, 0f, true)
                .Group(Tween.Alpha(_fadingPanel, 1f, 1f))
                .ChainCallback(LoadMainMenu);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
