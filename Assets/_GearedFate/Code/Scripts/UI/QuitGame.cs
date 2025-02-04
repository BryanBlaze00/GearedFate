using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    public class QuitGame : MonoBehaviour
    {
        [SerializeField] private Button _quitButton;

        protected void Awake()
        {
            this._quitButton.onClick.AddListener(this.Quit);
        }

        private void Quit()
        {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
