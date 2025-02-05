// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityUtils;

    /// <summary>
    /// GameManager class to manage game scenes and transitions. Also other possible Settings (define when needed).
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        // ... other game manager code ...
        [Header("Scene Settings")]
        [SerializeField]
        private string mainMenuScene = "MainMenu";

        [SerializeField]
        private string gameOver = "GameOver";

        [SerializeField]
        private string credits = "Credits";

        // [SerializeField] string introScene = "IntroScene";
        [SerializeField]
        private string[] bossLevelScenes =
        {
            "BossLevel 1", "BossLevel 2", "BossLevel 3",
            "BossLevel 4", "BossLevel 5",
        };

        [SerializeField]
        private string[] cutsceneScenes =
        {
            "Cutscene 0", "Cutscene 1", "Cutscene 2",
            "Cutscene 3", "Cutscene 4", "Cutscene 5",
        };

        private string currentScene;

        public void LoadMainMenu()
        {
            AudioManager.Instance.PlayMenuClip();
            LoadScene(mainMenuScene);
        }

        public void StartGame()
        {
            LoadScene("Cutscene 0");
        }

        public string GetCurrentScene()
        {
            return currentScene;
        }

        public string[] GetCutsceneScenes()
        {
            return cutsceneScenes;
        }

        public string[] GetBossLevelScenes()
        {
            return bossLevelScenes;
        }

        public void LoadNextLevel()
        {
            var currentBuildScene = SceneManager.GetActiveScene().buildIndex;
            if (currentBuildScene >= 10)
            {
                LoadMainMenu();
            }
            else
            {
                SceneManager.LoadScene(++currentBuildScene);
                if (currentBuildScene % 2 != 0)
                {
                    AudioManager.Instance.PlayCorrrectClip(currentBuildScene);
                }
            }
        }

        public void RestartLevel()
        {
            LoadScene(currentScene);
        }

        public void LoadGameOver()
        {
            LoadScene(gameOver);
        }

        public void LoadCredits()
        {
            LoadScene(credits);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        protected override void Awake() // Use protected override for Singleton's Awake
        {
            base.Awake(); // Important: Call the base Singleton Awake!

            currentScene = SceneManager.GetActiveScene().name;
        }

        private void LoadScene(string sceneName)
        {
            currentScene = sceneName;
            SceneManager.LoadScene(sceneName);
        }
    }
}
