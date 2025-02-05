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

        protected override void Awake() // Use protected override for Singleton's Awake
        {
            base.Awake(); // Important: Call the base Singleton Awake!

            currentScene = SceneManager.GetActiveScene().name;
        }

        public void LoadMainMenu()
        {
            AudioManager.instance.PlayMenuClip();
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
            /*
         int currentLevelIndex = -1;

         for (int i = 0; i < bossLevelScenes.Length; i++)
         {
            if (currentScene == bossLevelScenes[i] || currentScene == cutsceneScenes[i])
            {
               currentLevelIndex = i;
               break;
            }
         }

         if (currentLevelIndex == -1 && currentScene == introScene)
         {
            LoadCutscene(0);
            return;
         }

         if (currentLevelIndex < bossLevelScenes.Length - 1)
         {
            LoadCutscene(currentLevelIndex + 1);
         }
         else if (currentLevelIndex == bossLevelScenes.Length - 1)
         {
            Debug.Log("Game Completed!");
            LoadMainMenu();
         }
         else
         {
            Debug.LogWarning("No next level defined or invalid current scene: " + currentScene);
         }
      }

      private void LoadCutscene(int index)
      {
         if (index < cutsceneScenes.Length)
         {
            LoadScene(cutsceneScenes[index]);
         }
         else
         {
            Debug.LogError("Cutscene index out of range: " + index);
         }
      }

      public void LoadBossLevel(int levelIndex)
      {
         if (levelIndex >= 0 && levelIndex < bossLevelScenes.Length)
         {
            LoadScene(bossLevelScenes[levelIndex]);
         }
         else
         {
            Debug.LogError("Boss level index out of range: " + levelIndex);
         }
         */
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
                    AudioManager.instance.PlayCorrrectClip(currentBuildScene);
                }
            }
        }

        private void LoadScene(string sceneName)
        {
            currentScene = sceneName;
            SceneManager.LoadScene(sceneName);
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
    }
}
