// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.IO;
    using System.Linq;
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
        private string _mainMenuScene = "MainMenu";

        [SerializeField]
        private string _gameOver = "Gameover";

        [SerializeField]
        private string _credits = "Credits";

        [SerializeField]
        private string[] _bossLevelScenes =
        {
            "BossLevel 1", "BossLevel 2", "BossLevel 3",
            "BossLevel 4", "BossLevel 5",
        };

        [SerializeField]
        private string[] _cutsceneScenes =
        {
            "Cutscene 0", "Cutscene 1", "Cutscene 2",
            "Cutscene 3", "Cutscene 4", "Cutscene 5",
        };

        private string _currentScene;
        private string _previousScene;

        /// <summary>
        /// Gets the build index of a scene given its name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The build index of the scene, or -1 if the scene is not found.</returns>
        public static int GetBuildIndexFromSceneName(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneFileName = Path.GetFileNameWithoutExtension(scenePath);

                if (sceneFileName == sceneName)
                {
                    return i;
                }
            }

            return -1; // Scene not found
        }

        /// <summary>
        /// Gets the name of a scene given its build index.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene.</param>
        /// <returns>The name of the scene, or an empty string if the index is invalid.</returns>
        public static string GetSceneNameFromBuildIndex(int buildIndex)
        {
            if (buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings)
            {
                return SceneUtility.GetScenePathByBuildIndex(buildIndex).Split('/').Last().Split('.')[0];
            }
            else
            {
                return string.Empty; // Return an empty string for invalid indices
            }
        }

        /// <summary>
        /// Loads the Main Menu.
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene(_mainMenuScene);
            AudioManager.Instance.PlayMenuMusicClip();
        }

        /// <summary>
        /// Starts the game on Cutscene 0.
        /// </summary>
        public void StartGame()
        {
            LoadScene("Cutscene 0");
        }

        /// <summary>
        /// Gets the string of the current scene.
        /// </summary>
        /// <returns> Current Scene string. </returns>
        public string GetCurrentScene()
        {
            return _currentScene;
        }

        /// <summary>
        /// Gets the string of the previous scene.
        /// </summary>
        /// <returns> Previous Scene string. </returns>
        public string GetPreviousScene()
        {
            return _previousScene;
        }

        /// <summary>
        /// Gets the strings of all Cutscenes.
        /// </summary>
        /// <returns> Array of Cutscene strings. </returns>
        public string[] GetCutsceneScenes()
        {
            return _cutsceneScenes;
        }

        /// <summary>
        /// Gets the strings of all Boss Level scenes.
        /// </summary>
        /// <returns> Array of Boss Level strings. </returns>
        public string[] GetBossLevelScenes()
        {
            return _bossLevelScenes;
        }

        /// <summary>
        /// Loads the next level in the Build index.
        /// </summary>
        public void LoadNextLevel()
        {
            _previousScene = GetCurrentScene();

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
                    AudioManager.Instance.PlayCorrrectMusicClip(currentBuildScene);
                }
            }

            _currentScene = GetSceneNameFromBuildIndex(currentBuildScene);

#if UNITY_EDITOR
            Debug.Log("From LoadNextLevel:");
            Debug.Log("Current Scene = " + _currentScene);
            Debug.Log("Previous Scene = " + _previousScene);
#endif
        }

        /// <summary>
        /// Loads the previous scene from the Gameover scene or restarts current scene.
        /// </summary>
        public void RestartLevel()
        {
            if (GetCurrentScene().Equals("Gameover"))
            {
                int previousSceneIndex = GetBuildIndexFromSceneName(_previousScene);
                int musicIndex = previousSceneIndex % 2 == 0 ? previousSceneIndex : previousSceneIndex - 1;

                AudioManager.Instance.PlayCorrrectMusicClip(musicIndex);
                LoadScene(_previousScene);

                Debug.Log("MusicSceneIndex = " + musicIndex);
            }
            else
            {
                LoadScene(_currentScene);
            }
        }

        /// <summary>
        /// Loads the game over scene.
        /// </summary>
        public void LoadGameOver()
        {
            LoadScene(_gameOver);
        }

        /// <summary>
        /// Loads the credits scene.
        /// </summary>
        public void LoadCredits()
        {
            LoadScene(_credits);
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }

        protected override void Awake() // Use protected override for Singleton's Awake
        {
            base.Awake(); // Important: Call the base Singleton Awake!

            _currentScene = SceneManager.GetActiveScene().name;
        }

        private void LoadScene(string sceneName)
        {
            _previousScene = GetCurrentScene();
            _currentScene = sceneName;
            SceneManager.LoadScene(sceneName);

#if UNITY_EDITOR
            Debug.Log("From LoadScene:");
            Debug.Log("Current Scene = " + _currentScene);
            Debug.Log("Previous Scene = " + _previousScene);
#endif
        }
    }
}
