using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BTG
{
    public class PlayGame : MonoBehaviour
    {
        [SerializeField] private GameObject _loadScreen;

        [SerializeField] private GameObject _menuScreen;

        [SerializeField] private Image _progressBarFiller;

        [SerializeField] private Button _playButton;

        private float _targetFill;

        /// <summary>
        /// Start the game asynchronously.
        /// </summary>
        // private async void StartLoadGame()
        // {
        // AsyncOperation scene = SceneManager.LoadSceneAsync("Cutscene 0");
        // _loadScreen.SetActive(true);
        // _menuScreen.SetActive(false);
        // scene.allowSceneActivation = false;

        // while (scene.progress < 0.89)
        // {
        //     _targetFill = scene.progress;
        //     await Awaitable.NextFrameAsync();
        // }

        // _targetFill = scene.progress;
        // await Awaitable.WaitForSecondsAsync(1f);
        // _loadScreen.SetActive(false);
        // scene.allowSceneActivation = true;
        // }
        protected void Start()
        {
            this._playButton.onClick.AddListener(this.StartLoadGame);
        }

        protected void Update()
        {
            // AsyncOperation
            // Debug.Log(_progressBarFiller.fillAmount);

            // _progressBarFiller.fillAmount =
            //     Mathf.MoveTowards(_progressBarFiller.fillAmount, _targetFill, Time.deltaTime);
        }

        private void StartLoadGame()
        {
            GameManager.Instance.StartGame();
        }
    }
}
